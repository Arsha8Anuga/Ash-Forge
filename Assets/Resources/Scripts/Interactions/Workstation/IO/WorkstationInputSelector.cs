using System.Collections.Generic;
using UnityEngine;

public class WorkstationInputSelector :
    MonoBehaviour
{
    [SerializeField]
    private bool debugLog;

    public bool TrySelectItems(
        WorkstationRecipeData recipe,
        List<PhysicalItem> detectedItems,
        out List<PhysicalItem> selectedItems)
    {
        selectedItems =
            new List<PhysicalItem>();

        if (recipe == null ||
            recipe.requirements == null)
        {
            return false;
        }

        if (detectedItems == null ||
            detectedItems.Count <= 0)
        {
            return false;
        }

        List<PhysicalItem> available =
            new List<PhysicalItem>(
                detectedItems
            );

        foreach (WorkstationRequirement req
            in recipe.requirements)
        {
            if (req == null ||
                req.itemData == null)
            {
                continue;
            }

            int needed =
                Mathf.Max(
                    0,
                    req.amount
                );

            if (!TryTakeRequirement(
                req.itemData,
                needed,
                available,
                selectedItems))
            {
                selectedItems.Clear();

                Log(
                    "Failed requirement: " +
                    req.itemData.itemName +
                    " x" +
                    needed
                );

                return false;
            }
        }

        Log(
            "Selected items: " +
            selectedItems.Count
        );

        return selectedItems.Count > 0;
    }

    bool TryTakeRequirement(
        ItemData requiredItem,
        int neededAmount,
        List<PhysicalItem> available,
        List<PhysicalItem> selected)
    {
        if (requiredItem == null)
            return false;

        if (neededAmount <= 0)
            return true;

        int remaining =
            neededAmount;

        for (int i = available.Count - 1;
             i >= 0 && remaining > 0;
             i--)
        {
            PhysicalItem item =
                available[i];

            if (item == null)
            {
                available.RemoveAt(i);
                continue;
            }

            if (!item.IsValid)
            {
                available.RemoveAt(i);
                continue;
            }

            if (item.ItemData != requiredItem)
                continue;

            selected.Add(item);

            remaining -=
                Mathf.Max(
                    1,
                    item.Amount
                );

            available.RemoveAt(i);
        }

        return remaining <= 0;
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[WorkstationInputSelector] " +
            message,
            this
        );
    }
}