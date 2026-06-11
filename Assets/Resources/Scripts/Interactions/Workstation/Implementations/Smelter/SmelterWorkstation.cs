using System.Collections.Generic;
using UnityEngine;

public class SmelterWorkstation : MonoBehaviour, IActivatable
{
    [Header("Framework References")]
    [Tooltip("Isi dengan AreaItemScanner, atau biarkan kosong kalau scanner ada di child.")]
    [SerializeField]
    private MonoBehaviour inputProviderBehaviour;

    [SerializeField]
    private WorkstationRecipeMatcher matcher;

    [SerializeField]
    private WorkstationInputSelector inputSelector;

    [SerializeField]
    private WorkstationInputConsumer inputConsumer;

    [SerializeField]
    private WorkstationOutputSpawner outputSpawner;

    [Header("Smelter Requirements")]
    [SerializeField]
    private SmelterFuelTank fuelTank;

    [Tooltip("Single lid sensor lama. Boleh dikosongkan kalau pakai All Lid Sensors.")]
    [SerializeField]
    private SmelterLidSensor lidSensor;

    [Tooltip("Isi dengan semua lid sensor yang wajib tertutup. Contoh: Sensor Socket_A dan Sensor Socket_B.")]
    [SerializeField]
    private SmelterLidSensor[] allLidSensors;

    [Header("Behaviour")]
    [SerializeField]
    private bool autoStartWhenClosed = true;

    [SerializeField]
    private float autoStartInterval = 0.25f;

    [SerializeField]
    private bool cancelWhenLidOpened = true;

    [SerializeField]
    private bool cancelWhenFuelEmpty = true;

    [Tooltip("Kalau true, item yang sedang diproses harus tetap ada di area scanner sampai selesai.")]
    [SerializeField]
    private bool requireActiveItemsRemainInScanner = true;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    private IWorkstationInputProvider inputProvider;
    private WorkstationRecipeData activeRecipe;
    private List<PhysicalItem> activeItems = new List<PhysicalItem>();
    private WorkstationInputSnapshot activeSnapshot;
    private float processTimer;
    private float nextAutoStartTime;

    public bool IsBusy { get; private set; }

    public bool HasFuel => fuelTank != null && fuelTank.HasFuel;

    public bool IsLidClosed
    {
        get
        {
            bool hasAnySensor = false;

            if (allLidSensors != null && allLidSensors.Length > 0)
            {
                for (int i = 0; i < allLidSensors.Length; i++)
                {
                    SmelterLidSensor sensor = allLidSensors[i];

                    if (sensor == null)
                        return false;

                    hasAnySensor = true;

                    if (!sensor.IsClosed)
                        return false;
                }
            }
            else if (lidSensor != null)
            {
                hasAnySensor = true;

                if (!lidSensor.IsClosed)
                    return false;
            }

            return hasAnySensor;
        }
    }

    void Awake()
    {
        ResolveReferences();
    }

    void Update()
    {
        if (IsBusy)
        {
            TickProcess();
            return;
        }

        if (!autoStartWhenClosed)
            return;

        if (Time.time < nextAutoStartTime)
            return;

        nextAutoStartTime = Time.time + autoStartInterval;

        if (!CanSmeltNow())
            return;

        TryStart();
    }

    void ResolveReferences()
    {
        if (inputProviderBehaviour != null)
            inputProvider = inputProviderBehaviour as IWorkstationInputProvider;

        if (inputProvider == null)
            inputProvider = GetComponentInChildren<IWorkstationInputProvider>();

        if (matcher == null)
            matcher = GetComponent<WorkstationRecipeMatcher>();

        if (inputSelector == null)
            inputSelector = GetComponent<WorkstationInputSelector>();

        if (inputConsumer == null)
            inputConsumer = GetComponent<WorkstationInputConsumer>();

        if (outputSpawner == null)
            outputSpawner = GetComponentInChildren<WorkstationOutputSpawner>();

        if (fuelTank == null)
            fuelTank = GetComponentInChildren<SmelterFuelTank>();

        if (lidSensor == null)
            lidSensor = GetComponentInChildren<SmelterLidSensor>();

        if (allLidSensors == null || allLidSensors.Length == 0)
            allLidSensors = GetComponentsInChildren<SmelterLidSensor>(true);
    }

    public void Activate(XRHandInteractor hand)
    {
        TryStart();
    }

    public bool TryStart()
    {
        ResolveReferences();

        if (IsBusy)
            return false;

        if (!CanSmeltNow())
        {
            Log("Cannot start. Lid closed: " + IsLidClosed + " | fuel: " + HasFuel);
            return false;
        }

        List<PhysicalItem> detected = GetDetectedItems();

        if (detected.Count <= 0)
            return false;

        List<WorkstationRecipeData> matches =
            matcher != null
            ? matcher.FindFullMatches(detected)
            : new List<WorkstationRecipeData>();

        if (matches == null || matches.Count <= 0)
            return false;

        foreach (WorkstationRecipeData recipe in matches)
        {
            if (recipe == null)
                continue;

            List<PhysicalItem> selected;

            if (!TrySelectInput(recipe, detected, out selected))
                continue;

            BeginProcess(recipe, selected);
            return true;
        }

        return false;
    }

    bool TrySelectInput(
        WorkstationRecipeData recipe,
        List<PhysicalItem> detected,
        out List<PhysicalItem> selected)
    {
        selected = null;

        if (inputSelector != null)
        {
            return inputSelector.TrySelectItems(
                recipe,
                detected,
                out selected
            );
        }

        selected = new List<PhysicalItem>(detected);
        return selected.Count > 0;
    }

    void BeginProcess(
        WorkstationRecipeData recipe,
        List<PhysicalItem> selected)
    {
        IsBusy = true;
        activeRecipe = recipe;
        activeItems = new List<PhysicalItem>(selected);
        activeSnapshot = new WorkstationInputSnapshot(activeItems);
        processTimer = 0f;

        Log("Begin smelting: " + recipe.recipeName + " | time: " + recipe.processTime + "s");
    }

    void TickProcess()
    {
        if (activeRecipe == null)
        {
            CancelProcess("Recipe missing");
            return;
        }

        if (cancelWhenLidOpened && !IsLidClosed)
        {
            CancelProcess("Smelting cancelled: lid opened");
            return;
        }

        if (!ValidateActiveInput())
        {
            CancelProcess("Smelting cancelled: input moved/changed");
            return;
        }

        float delta = Time.deltaTime;

        if (fuelTank == null || !fuelTank.TryConsume(delta))
        {
            if (cancelWhenFuelEmpty)
            {
                CancelProcess("Smelting cancelled: fuel empty");
                return;
            }

            return;
        }

        processTimer += delta;

        float targetTime = Mathf.Max(0.1f, activeRecipe.processTime);

        if (processTimer < targetTime)
            return;

        CompleteProcess();
    }

    bool ValidateActiveInput()
    {
        if (!requireActiveItemsRemainInScanner)
            return activeSnapshot == null || activeSnapshot.IsStillValid();

        if (activeSnapshot == null)
            return true;

        return activeSnapshot.IsStillValid(
            GetDetectedItems(),
            true,
            false
        );
    }

    void CompleteProcess()
    {
        Log("Complete smelting: " + activeRecipe.recipeName);

        if (outputSpawner != null)
            outputSpawner.Spawn(activeRecipe, activeItems);
        else
            Log("OutputSpawner missing");

        if (inputConsumer != null)
            inputConsumer.Consume(activeRecipe, activeItems);
        else
            Log("InputConsumer missing");

        ResetProcess();
    }

    void CancelProcess(string reason)
    {
        Log(reason);
        ResetProcess();
    }

    void ResetProcess()
    {
        IsBusy = false;
        activeRecipe = null;
        activeSnapshot = null;

        if (activeItems != null)
            activeItems.Clear();

        activeItems = new List<PhysicalItem>();
        processTimer = 0f;
    }

    bool CanSmeltNow()
    {
        return IsLidClosed && HasFuel;
    }

    List<PhysicalItem> GetDetectedItems()
    {
        if (inputProvider == null)
            ResolveReferences();

        if (inputProvider == null)
            return new List<PhysicalItem>();

        List<PhysicalItem> items = inputProvider.GetItems();

        if (items == null)
            return new List<PhysicalItem>();

        return items;
    }

    void Log(string message)
    {
        if (!debugLog)
            return;

        Debug.Log("[SmelterWorkstation] " + message, this);
    }
}
