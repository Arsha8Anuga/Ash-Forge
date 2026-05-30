using System.Collections.Generic;
using UnityEngine;

public class WorkstationInputConsumer :
    MonoBehaviour
{
    [SerializeField]
    private bool debugLog;

        class ConsumeEntry
    {
        public PhysicalItem item;
        public int amount;

        public ConsumeEntry(
            PhysicalItem item,
            int amount)
        {
            this.item = item;

            this.amount =
                Mathf.Max(
                    0,
                    amount
                );
        }
    }

    public void Consume(
        WorkstationRecipeData recipe,
        List<PhysicalItem> selectedItems)
    {
        if (recipe == null)
            return;

        if (selectedItems == null)
            return;

        if (recipe.requirements == null)
            return;

        List<ConsumeEntry> consumeEntries =
            BuildConsumeEntries(
                recipe,
                selectedItems
            );

        SortDeepestChainFirst(
            consumeEntries
        );

        foreach (ConsumeEntry entry
            in consumeEntries)
        {
            ConsumeEntryAmount(
                entry
            );
        }
    }

   List<ConsumeEntry> BuildConsumeEntries(
        WorkstationRecipeData recipe,
        List<PhysicalItem> selectedItems)
    {
        List<ConsumeEntry> result =
            new List<ConsumeEntry>();

        if (recipe == null ||
            recipe.requirements == null ||
            selectedItems == null)
        {
            return result;
        }

        Dictionary<PhysicalItem, int> availableAmounts =
            new Dictionary<PhysicalItem, int>();

        foreach (PhysicalItem item
            in selectedItems)
        {
            if (item == null)
                continue;

            if (!item.IsValid)
                continue;

            if (availableAmounts.ContainsKey(item))
                continue;

            availableAmounts.Add(
                item,
                Mathf.Max(
                    1,
                    item.Amount
                )
            );
        }

        foreach (WorkstationRequirement req
            in recipe.requirements)
        {
            if (req == null ||
                req.itemData == null)
            {
                continue;
            }

            int remaining =
                Mathf.Max(
                    0,
                    req.amount
                );

            foreach (PhysicalItem item
                in selectedItems)
            {
                if (remaining <= 0)
                    break;

                if (item == null)
                    continue;

                if (!item.IsValid)
                    continue;

                if (item.ItemData != req.itemData)
                    continue;

                if (!availableAmounts.TryGetValue(
                    item,
                    out int available))
                {
                    continue;
                }

                if (available <= 0)
                    continue;

                int consumeAmount =
                    Mathf.Min(
                        available,
                        remaining
                    );

                AddOrMergeConsumeEntry(
                    result,
                    item,
                    consumeAmount
                );

                availableAmounts[item] =
                    available - consumeAmount;

                remaining -= consumeAmount;
            }

            if (remaining > 0)
            {
                Log(
                    "Consume warning: insufficient item during consume: " +
                    req.itemData.itemName +
                    " missing " +
                    remaining
                );
            }
        }

        return result;
    }

    void AddOrMergeConsumeEntry(
        List<ConsumeEntry> entries,
        PhysicalItem item,
        int amount)
    {
        if (entries == null ||
            item == null ||
            amount <= 0)
        {
            return;
        }

        foreach (ConsumeEntry entry
            in entries)
        {
            if (entry == null)
                continue;

            if (entry.item != item)
                continue;

            entry.amount += amount;
            return;
        }

        entries.Add(
            new ConsumeEntry(
                item,
                amount
            )
        );
    }

    void SortDeepestChainFirst(
        List<ConsumeEntry> entries)
    {
        if (entries == null)
            return;

        entries.Sort(
            CompareDepthDescending
        );
    }

    int CompareDepthDescending(
        ConsumeEntry a,
        ConsumeEntry b)
    {
        PhysicalItem itemA =
            a != null
            ? a.item
            : null;

        PhysicalItem itemB =
            b != null
            ? b.item
            : null;

        int depthA =
            GetChainDepth(itemA);

        int depthB =
            GetChainDepth(itemB);

        return depthB.CompareTo(
            depthA
        );
    }

    int GetChainDepth(
        PhysicalItem item)
    {
        if (item == null)
            return 0;

        SnapChainNode node =
            item.GetComponent<SnapChainNode>();

        if (node == null)
            return 0;

        return node.GetDepth();
    }

    void ConsumeEntryAmount(
        ConsumeEntry entry)
    {
        if (entry == null ||
            entry.item == null)
        {
            return;
        }

        PhysicalItem item =
            entry.item;

        int consumeAmount =
            Mathf.Max(
                0,
                entry.amount
            );

        if (consumeAmount <= 0)
            return;

        int currentAmount =
            Mathf.Max(
                0,
                item.Amount
            );

        int remainingAmount =
            currentAmount - consumeAmount;

        if (remainingAmount > 0)
        {
            Log(
                "Consume partial: " +
                item.ItemName +
                " -" +
                consumeAmount +
                " | remain: " +
                remainingAmount
            );

            item.SetAmount(
                remainingAmount
            );

            return;
        }

        Log(
            "Consume full object: " +
            item.ItemName +
            " x" +
            currentAmount
        );

        DestroyPhysicalItemSafely(
            item
        );
    }

    void DestroyPhysicalItemSafely(
        PhysicalItem item)
    {
        if (item == null)
            return;

        SnapChainNode chain =
            item.GetComponent<SnapChainNode>();

        if (chain != null)
        {
            chain.DetachChildrenBeforeDestroy();

            chain.DetachFromParentOnly();
        }

        SnappableObject snap =
            item.GetComponent<SnappableObject>();

        if (snap != null &&
            snap.IsSnapped)
        {
            snap.Unsnap();
        }

        Destroy(
            item.gameObject
        );
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[WorkstationInputConsumer] " +
            message,
            this
        );
    }
}