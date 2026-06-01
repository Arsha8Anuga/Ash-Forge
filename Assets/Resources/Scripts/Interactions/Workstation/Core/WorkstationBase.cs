using System.Collections.Generic;
using UnityEngine;

public class WorkstationBase :
    MonoBehaviour,
    IActivatable
{
    [Header("Input")]
    [SerializeField]
    private MonoBehaviour inputProviderBehaviour;

    [SerializeField]
    private WorkstationInputSelector inputSelector;

    [SerializeField]
    private WorkstationInputConsumer inputConsumer;

    [Header("Recipe")]
    [SerializeField]
    private WorkstationRecipeMatcher matcher;

    [Header("Output")]
    [SerializeField]
    private WorkstationOutputSpawner outputSpawner;

    [Header("Execution")]
    [SerializeField]
    private WorkstationExecutionBase execution;

    [Header("Interruption")]
    [SerializeField]
    private bool requireActiveItemsRemainInScanner = true;

    [SerializeField]
    private bool interruptOnScannerCompositionChanged = true;

    [SerializeField]
    private string inputChangedMessage =
        "Process failed: input area changed";

    [Header("UI")]
    [SerializeField]
    private WorkstationUI ui;

    [Header("Timing")]
    [SerializeField]
    private float activateCooldown = 0.25f;

    [Header("Debug")]
    [SerializeField]
    protected bool debugLog;

    private IWorkstationInputProvider inputProvider;

    private float nextActivateTime;

    private bool isBusy;

    private WorkstationRecipeData activeRecipe;

    private List<PhysicalItem> activeItems =
        new List<PhysicalItem>();

    private WorkstationInputSnapshot activeSnapshot;

    public bool IsBusy =>
        isBusy;

    protected virtual void Awake()
    {
        ResolveReferences();

        if (ui != null)
        {
            ui.Bind(this);

            ui.SetOutputSpawner(
                outputSpawner
            );
        }
    }

    void Update()
    {
        if (!isBusy)
            return;

        ValidateActiveInput();
    }

    void ResolveReferences()
    {
        if (inputProviderBehaviour != null)
        {
            inputProvider =
                inputProviderBehaviour
                as IWorkstationInputProvider;
        }

        if (inputProvider == null)
        {
            inputProvider =
                GetComponentInChildren
                <IWorkstationInputProvider>();
        }

        if (matcher == null)
            matcher =
                GetComponent
                <WorkstationRecipeMatcher>();

        if (inputSelector == null)
            inputSelector =
                GetComponent
                <WorkstationInputSelector>();

        if (inputConsumer == null)
            inputConsumer =
                GetComponent
                <WorkstationInputConsumer>();

        if (outputSpawner == null)
        {
            outputSpawner =
                GetComponentInChildren
                <WorkstationOutputSpawner>();
        }

        if (execution == null)
        {
            execution =
                GetComponent
                <WorkstationExecutionBase>();
        }

        if (ui == null)
        {
            ui =
                GetComponentInChildren
                <WorkstationUI>();
        }
    }

    public void Activate(
        XRHandInteractor hand)
    {
        if (Time.time < nextActivateTime)
            return;

        nextActivateTime =
            Time.time + activateCooldown;

        if (isBusy)
        {
            Log("Machine busy");

            if (ui != null)
                ui.ShowBusy();

            return;
        }

        TryStart();
    }

    public void TryStart()
    {
        List<PhysicalItem> detected =
            GetDetectedItems();

        Log(
            "Detected items: " +
            detected.Count
        );

        List<WorkstationRecipeData> matches =
            matcher != null
            ? matcher.FindFullMatches(detected)
            : new List<WorkstationRecipeData>();

        if (matches == null ||
            matches.Count == 0)
        {
            Log("No matching recipe");

            if (ui != null)
                ui.ShowNoRecipe();

            return;
        }

        if (matches.Count == 1)
        {
            SelectRecipe(matches[0]);
            return;
        }

        if (ui != null)
        {
            ui.ShowSelection(
                matches,
                detected
            );
            return;
        }

        Log(
            "Multiple recipes matched, but UI missing"
        );
    }

    public void SelectRecipe(
        WorkstationRecipeData recipe)
    {
        if (recipe == null)
            return;

        if (isBusy)
        {
            if (ui != null)
                ui.ShowBusy();

            return;
        }

        List<PhysicalItem> detected =
            GetDetectedItems();

        List<PhysicalItem> selected =
            null;

        if (inputSelector != null)
        {
            bool success =
                inputSelector.TrySelectItems(
                    recipe,
                    detected,
                    out selected
                );

            if (!success)
            {
                selected = null;
            }
        }
        else
        {
            selected =
                new List<PhysicalItem>(
                    detected
                );
        }

        if (selected == null ||
            selected.Count == 0)
        {
            Log(
                "No selected input for recipe: " +
                recipe.recipeName
            );

            if (ui != null)
            {
                ui.ShowFailed(
                    "Input selection failed"
                );
            }

            return;
        }

        if (!CanStartRecipe(
            recipe,
            selected
        ))
        {
            Log(
                "Recipe rejected by workstation rule: " +
                recipe.recipeName
            );

            if (ui != null)
            {
                ui.ShowFailed(
                    "Recipe rule failed"
                );
            }

            return;
        }

              if (!CanStartRecipe(
            recipe,
            selected))
        {
            Log(
                "Recipe rejected by validation: " +
                recipe.recipeName
            );

            if (ui != null)
            {
                ui.ShowFailed(
                    "Recipe validation failed"
                );
            }

            return;
        }

        BeginProcess(
            recipe,
            selected
        );
    }

    void BeginProcess(
        WorkstationRecipeData recipe,
        List<PhysicalItem> selectedItems)
    {
        isBusy = true;

        activeRecipe = recipe;

        activeItems =
            new List<PhysicalItem>(
                selectedItems
            );

        activeSnapshot =
            new WorkstationInputSnapshot(
                activeItems,
                GetDetectedItems()
            );

        Log(
            "Begin process: " +
            recipe.recipeName +
            " | items: " +
            activeItems.Count
        );

        if (ui != null)
        {
            ui.ShowProcessing(
                activeRecipe,
                0,
                activeItems
            );
        }

        if (execution != null)
        {
            execution.Begin(
                this,
                activeRecipe,
                activeItems
            );

            return;
        }

        CompleteWork();
    }

    void ValidateActiveInput()
    {
        if (activeSnapshot == null)
            return;

        List<PhysicalItem> currentDetected =
            GetDetectedItems();

        bool valid =
            activeSnapshot.IsStillValid(
                currentDetected,
                requireActiveItemsRemainInScanner,
                interruptOnScannerCompositionChanged
            );

        if (valid)
            return;

        CancelWork(
            inputChangedMessage
        );
    }

    protected virtual bool CanStartRecipe(
        WorkstationRecipeData recipe,
        List<PhysicalItem> selectedItems)
    {
        return true;
    }

    public void CompleteWork()
    {
        if (!isBusy)
            return;

        if (activeSnapshot != null)
        {
            List<PhysicalItem> currentDetected =
                GetDetectedItems();

            bool valid =
                activeSnapshot.IsStillValid(
                    currentDetected,
                    requireActiveItemsRemainInScanner,
                    interruptOnScannerCompositionChanged
                );

            if (!valid)
            {
                CancelWork(
                    inputChangedMessage
                );

                return;
            }
        }

        SpawnOutputs();

        ConsumeInputs();

        ResetProcessState();

        if (ui != null)
            ui.ShowDone();

        Log("Process complete");
    }

    public void CancelWork()
    {
        CancelWork(
            "Process cancelled"
        );
    }

    public void CancelWork(
        string reason)
    {
        if (!isBusy)
            return;

        if (execution != null)
            execution.Cancel();

        ResetProcessState();

        if (ui != null)
            ui.ShowFailed(reason);

        Log(reason);
    }

    void SpawnOutputs()
    {
        if (outputSpawner == null)
        {
            Log("No output spawner.");
            return;
        }

        if (activeRecipe == null)
            return;

        outputSpawner.Spawn(
            activeRecipe,
            activeItems
        );
    }

    void ConsumeInputs()
    {
        if (inputConsumer == null)
        {
            Log("No input consumer.");
            return;
        }

        if (activeRecipe == null)
            return;

        if (activeItems == null)
            return;

        inputConsumer.Consume(
            activeRecipe,
            activeItems
        );
    }

    void ResetProcessState()
    {
        isBusy = false;

        activeRecipe = null;

        activeSnapshot = null;

        if (activeItems != null)
            activeItems.Clear();

        activeItems =
            new List<PhysicalItem>();
    }

    List<PhysicalItem> GetDetectedItems()
    {
        if (inputProvider == null)
            ResolveReferences();

        if (inputProvider == null)
            return new List<PhysicalItem>();

        List<PhysicalItem> items =
            inputProvider.GetItems();

        if (items == null)
            return new List<PhysicalItem>();

        return items;
    }

    public void ShowStep(
        WorkstationRecipeData recipe,
        int stepIndex)
    {
        if (ui == null)
            return;

        ui.ShowStep(
            recipe,
            stepIndex
        );
    }

    

    protected void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[" + GetType().Name + "] " +
            message,
            this
        );
    }
}