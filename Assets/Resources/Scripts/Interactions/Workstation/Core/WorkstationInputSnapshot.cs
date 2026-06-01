using System.Collections.Generic;
using UnityEngine;

public class WorkstationInputSnapshot
{
    private readonly List<PhysicalItem> trackedItems =
        new List<PhysicalItem>();

    private readonly Dictionary<PhysicalItem, int> amounts =
        new Dictionary<PhysicalItem, int>();

    private readonly List<PhysicalItem> scannerItems =
        new List<PhysicalItem>();

    private readonly Dictionary<PhysicalItem, int> scannerAmounts =
        new Dictionary<PhysicalItem, int>();

    private bool hasScannerSnapshot;

    public WorkstationInputSnapshot(
        List<PhysicalItem> items)
    {
        CaptureItems(
            items,
            trackedItems,
            amounts
        );
    }

    public WorkstationInputSnapshot(
        List<PhysicalItem> selectedItems,
        List<PhysicalItem> detectedScannerItems)
    {
        CaptureItems(
            selectedItems,
            trackedItems,
            amounts
        );

        CaptureItems(
            detectedScannerItems,
            scannerItems,
            scannerAmounts
        );

        hasScannerSnapshot = true;
    }

    public bool IsStillValid()
    {
        return AreItemsStillValid(
            trackedItems,
            amounts
        );
    }

    public bool IsStillValid(
        List<PhysicalItem> currentScannerItems,
        bool requireTrackedItemsInsideScanner,
        bool requireScannerCompositionUnchanged)
    {
        if (!IsStillValid())
            return false;

        if (requireTrackedItemsInsideScanner)
        {
            if (!TrackedItemsStillInsideScanner(
                currentScannerItems))
            {
                return false;
            }
        }

        if (requireScannerCompositionUnchanged)
        {
            if (!ScannerCompositionMatches(
                currentScannerItems))
            {
                return false;
            }
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

    void CaptureItems(
        List<PhysicalItem> source,
        List<PhysicalItem> targetList,
        Dictionary<PhysicalItem, int> targetAmounts)
    {
        if (source == null)
            return;

        foreach (PhysicalItem item in source)
        {
            if (item == null)
                continue;

            if (!item.IsValid)
                continue;

            if (targetList.Contains(item))
                continue;

            targetList.Add(item);

            targetAmounts[item] =
                item.Amount;
        }
    }

    bool AreItemsStillValid(
        List<PhysicalItem> items,
        Dictionary<PhysicalItem, int> itemAmounts)
    {
        foreach (PhysicalItem item in items)
        {
            if (item == null)
                return false;

            if (!item.IsValid)
                return false;

            if (!itemAmounts.TryGetValue(
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

    bool TrackedItemsStillInsideScanner(
        List<PhysicalItem> currentScannerItems)
    {
        foreach (PhysicalItem item in trackedItems)
        {
            if (item == null)
                return false;

            if (!ContainsPhysicalItem(
                currentScannerItems,
                item))
            {
                return false;
            }
        }

        return true;
    }

    bool ScannerCompositionMatches(
        List<PhysicalItem> currentScannerItems)
    {
        if (!hasScannerSnapshot)
            return true;

        List<PhysicalItem> current =
            BuildValidUniqueList(
                currentScannerItems
            );

        if (current.Count != scannerItems.Count)
            return false;

        foreach (PhysicalItem item in scannerItems)
        {
            if (item == null)
                return false;

            if (!item.IsValid)
                return false;

            if (!ContainsPhysicalItem(
                current,
                item))
            {
                return false;
            }

            if (!scannerAmounts.TryGetValue(
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

    List<PhysicalItem> BuildValidUniqueList(
        List<PhysicalItem> source)
    {
        List<PhysicalItem> result =
            new List<PhysicalItem>();

        if (source == null)
            return result;

        foreach (PhysicalItem item in source)
        {
            if (item == null)
                continue;

            if (!item.IsValid)
                continue;

            if (result.Contains(item))
                continue;

            result.Add(item);
        }

        return result;
    }

    bool ContainsPhysicalItem(
        List<PhysicalItem> items,
        PhysicalItem target)
    {
        if (items == null ||
            target == null)
        {
            return false;
        }

        foreach (PhysicalItem item in items)
        {
            if (item == target)
                return true;
        }

        return false;
    }
}