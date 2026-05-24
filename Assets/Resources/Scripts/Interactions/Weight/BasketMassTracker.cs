using System.Collections.Generic;
using UnityEngine;

public class BasketMassTracker :
    MonoBehaviour,
    IWeightProvider
{
    [SerializeField]
    private Rigidbody basketBody;

    [SerializeField]
    private bool debugLog;

    private readonly HashSet<PhysicalItem> trackedItems =
        new HashSet<PhysicalItem>();

    public float TotalMass
    {
        get;
        private set;
    }

    public int TrackedCount =>
        trackedItems.Count;

    void Awake()
    {
        if (basketBody == null)
        {
            basketBody =
                GetComponent<Rigidbody>();
        }

        Recalculate();
    }

    public float GetAdditionalWeight()
    {
        if (basketBody == null)
            return TotalMass;

        return Mathf.Max(
            0f,
            TotalMass - basketBody.mass
        );
    }

    public void AddItem(
        PhysicalItem item)
    {
        if (item == null)
            return;

        if (!trackedItems.Add(item))
            return;

        Recalculate();
    }

    public void RemoveItem(
        PhysicalItem item)
    {
        if (item == null)
            return;

        if (!trackedItems.Remove(item))
            return;

        Recalculate();
    }

    public void Recalculate()
    {
        TotalMass =
            basketBody != null
            ? basketBody.mass
            : 0f;

        trackedItems.RemoveWhere(
            item => item == null
        );

        foreach (PhysicalItem item
            in trackedItems)
        {
            TotalMass +=
                GetItemMass(item);
        }

        if (debugLog)
        {
            Debug.Log(
                "[BasketMassTracker] TotalMass: " +
                TotalMass +
                " | Items: " +
                trackedItems.Count,
                this
            );
        }
    }

    float GetItemMass(
        PhysicalItem item)
    {
        Rigidbody rb =
            item.GetComponent<Rigidbody>();

        if (rb != null)
            return rb.mass;

        if (item.ItemData != null)
        {
            return item.ItemData.mass *
                item.Amount;
        }

        return 0f;
    }
}