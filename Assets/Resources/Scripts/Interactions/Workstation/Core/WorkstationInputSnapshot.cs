using System.Collections.Generic;
using UnityEngine;

public class WorkstationInputSnapshot
{
    private readonly List<PhysicalItem> trackedItems =
        new List<PhysicalItem>();

    private readonly Dictionary<PhysicalItem, int> amounts =
        new Dictionary<PhysicalItem, int>();

    public WorkstationInputSnapshot(
        List<PhysicalItem> items)
    {
        if (items == null)
            return;

        foreach (PhysicalItem item
            in items)
        {
            if (item == null)
                continue;

            if (!item.IsValid)
                continue;

            if (trackedItems.Contains(item))
                continue;

            trackedItems.Add(item);

            amounts[item] =
                item.Amount;
        }
    }

    public bool IsStillValid()
    {
        foreach (PhysicalItem item
            in trackedItems)
        {
            if (item == null)
                return false;

            if (!item.IsValid)
                return false;

            if (!amounts.TryGetValue(
                item,
                out int amount))
            {
                return false;
            }

            if (item.Amount != amount)
                return false;
        }

        return true;
    }

    public bool Contains(
        PhysicalItem item)
    {
        return item != null &&
            trackedItems.Contains(item);
    }

    public int Count =>
        trackedItems.Count;
}