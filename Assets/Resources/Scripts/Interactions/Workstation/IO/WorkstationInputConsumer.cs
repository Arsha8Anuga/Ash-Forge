using System.Collections.Generic;
using UnityEngine;

public class WorkstationInputConsumer :
    MonoBehaviour
{
    [SerializeField]
    private bool debugLog;

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

        List<PhysicalItem> consumeItems =
            BuildConsumeList(
                recipe,
                selectedItems
            );

        SortDeepestChainFirst(
            consumeItems
        );

        foreach (PhysicalItem item
            in consumeItems)
        {
            ConsumeOnePhysicalItem(
                item
            );
        }
    }

    List<PhysicalItem> BuildConsumeList(
        WorkstationRecipeData recipe,
        List<PhysicalItem> selectedItems)
    {
        List<PhysicalItem> result =
            new List<PhysicalItem>();

        List<PhysicalItem> pool =
            new List<PhysicalItem>(
                selectedItems
            );

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

            for (int i = pool.Count - 1;
                i >= 0 && remaining > 0;
                i--)
            {
                PhysicalItem item =
                    pool[i];

                if (item == null)
                {
                    pool.RemoveAt(i);
                    continue;
                }

                if (!item.IsValid)
                {
                    pool.RemoveAt(i);
                    continue;
                }

                if (item.ItemData != req.itemData)
                    continue;

                result.Add(item);

                pool.RemoveAt(i);

                remaining--;
            }
        }

        return result;
    }

    void SortDeepestChainFirst(
        List<PhysicalItem> items)
    {
        if (items == null)
            return;

        items.Sort(
            CompareDepthDescending
        );
    }

    int CompareDepthDescending(
        PhysicalItem a,
        PhysicalItem b)
    {
        int depthA =
            GetChainDepth(a);

        int depthB =
            GetChainDepth(b);

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

    void ConsumeOnePhysicalItem(
        PhysicalItem item)
    {
        if (item == null)
            return;

        Log(
            "Consume object: " +
            item.ItemName
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