using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SmelterFuelIntake : MonoBehaviour
{
    [Header("Fuel")]
    [SerializeField]
    private SmelterFuelTank fuelTank;

    [SerializeField]
    private SmelterFuelData[] acceptedFuels;

    [Header("Input")]
    [SerializeField]
    private float inputDwellTime = 0.15f;

    [SerializeField]
    private bool ignoreHeldItems = true;

    [SerializeField]
    private bool debugLog;

    private readonly Dictionary<PhysicalItem, float> candidates =
        new Dictionary<PhysicalItem, float>();

    private readonly HashSet<PhysicalItem> processing =
        new HashSet<PhysicalItem>();

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        if (fuelTank == null)
            fuelTank = GetComponentInParent<SmelterFuelTank>();
    }

    void OnTriggerEnter(Collider other)
    {
        Register(other);
    }

    void OnTriggerStay(Collider other)
    {
        Register(other);
        TryProcess(other);
    }

    void OnTriggerExit(Collider other)
    {
        PhysicalItem item = FindItem(other);

        if (item == null)
            return;

        candidates.Remove(item);
        processing.Remove(item);
    }

    void Register(Collider other)
    {
        PhysicalItem item = FindItem(other);

        if (item == null)
            return;

        if (candidates.ContainsKey(item))
            return;

        candidates.Add(item, Time.time);
    }

    void TryProcess(Collider other)
    {
        if (fuelTank == null)
            return;

        PhysicalItem item = FindItem(other);

        if (item == null)
            return;

        if (processing.Contains(item))
            return;

        if (!item.IsValid)
            return;

        if (ignoreHeldItems && IsHeld(item))
            return;

        if (!candidates.TryGetValue(item, out float enterTime))
            return;

        if (Time.time - enterTime < inputDwellTime)
            return;

        SmelterFuelData fuel = FindFuel(item.ItemData);

        if (fuel == null)
            return;

        int amount = Mathf.Max(1, item.Amount);
        float seconds = fuel.burnSecondsPerItem * amount;

        if (!fuelTank.TryAdd(seconds))
        {
            Log("Fuel tank full / cannot add: " + item.ItemName);
            return;
        }

        processing.Add(item);
        DestroyFuelItem(item);

        Log("Fuel accepted: " + item.ItemName + " x" + amount + " = " + seconds.ToString("0.##") + "s");
    }

    PhysicalItem FindItem(Collider other)
    {
        if (other == null)
            return null;

        return other.GetComponentInParent<PhysicalItem>();
    }

    SmelterFuelData FindFuel(ItemData itemData)
    {
        if (itemData == null || acceptedFuels == null)
            return null;

        foreach (SmelterFuelData fuel in acceptedFuels)
        {
            if (fuel == null)
                continue;

            if (fuel.itemData == itemData)
                return fuel;
        }

        return null;
    }

    bool IsHeld(PhysicalItem item)
    {
        if (item == null)
            return false;

        IGrabbable grabbable = item.GetComponent<IGrabbable>();
        return grabbable != null && grabbable.IsHeld;
    }

    void DestroyFuelItem(PhysicalItem item)
    {
        if (item == null)
            return;

        ISnappable snap = item.GetComponent<ISnappable>();

        if (snap != null && snap.IsSnapped)
            snap.Unsnap();

        candidates.Remove(item);
        processing.Remove(item);

        Destroy(item.gameObject);
    }

    void Log(string message)
    {
        if (!debugLog)
            return;

        Debug.Log("[SmelterFuelIntake] " + message, this);
    }
}
