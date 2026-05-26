using System.Collections.Generic;
using UnityEngine;

public class AssemblyProcessor :
    MonoBehaviour,
    IActivatable
{
    [SerializeField]
    private AssemblyAreaScanner scanner;

    [SerializeField]
    private AssemblyRecipeMatcher matcher;

    [SerializeField]
    private AssemblyRecipeSelectionUI selectionUI;

    [SerializeField]
    private Transform outputPoint;

    [SerializeField]
    private bool debugLog;

    void Awake()
    {
        if (scanner == null)
        {
            scanner =
                GetComponentInChildren<AssemblyAreaScanner>();
        }

        if (matcher == null)
        {
            matcher =
                GetComponent<AssemblyRecipeMatcher>();
        }

        if (outputPoint == null)
        {
            outputPoint = transform;
        }

        if (selectionUI != null)
        {
            selectionUI.Bind(this);
        }
    }

    public void Activate(
        XRHandInteractor hand)
    {
        TryAssemble();
    }

    public bool TryAssemble()
    {
        if (scanner == null ||
            matcher == null)
        {
            return false;
        }

        List<PhysicalItem> items =
            scanner.GetItems();

        Log(
            "Detected items: " +
            items.Count
        );

        foreach (PhysicalItem item in items)
        {
            if (item == null)
                continue;

            Log(
                "Item: " +
                item.ItemName +
                " | Data: " +
                (
                    item.ItemData != null
                    ? item.ItemData.name
                    : "NULL"
                ) +
                " | Amount: " +
                item.Amount
            );
        }

        List<AssemblyRecipeData> matches =
            matcher.FindFullMatches(items);

        if (matches.Count == 0)
        {
            Log("No matching recipe");
            return false;
        }

        if (matches.Count == 1)
        {
            return AssembleRecipe(
                matches[0]
            );
        }

        if (selectionUI == null)
        {
            Log(
                "Multiple recipes matched, but Selection UI is missing."
            );

            return false;
        }

        selectionUI.Show(matches);

        Log(
            "Showing recipe selection UI. Matches: " +
            matches.Count
        );

        return true;
    }

    public bool AssembleRecipe(
        AssemblyRecipeData recipe)
    {
        if (recipe == null)
            return false;

        if (recipe.output == null ||
            recipe.output.itemData == null ||
            recipe.output.itemData.prefab == null)
        {
            Log("Recipe output invalid");
            return false;
        }

        List<PhysicalItem> items =
            scanner.GetItems();

        ConsumeRequirements(
            recipe,
            items
        );

        SpawnOutput(recipe);

        Log(
            "Assembled: " +
            recipe.recipeName
        );

        return true;
    }

    void ConsumeRequirements(
        AssemblyRecipeData recipe,
        List<PhysicalItem> items)
    {
        foreach (AssemblyRequirement requirement
            in recipe.requirements)
        {
            int remaining =
                requirement.amount;

            foreach (PhysicalItem item in items)
            {
                if (item == null)
                    continue;

                if (item.ItemData != requirement.itemData)
                    continue;

                if (remaining <= 0)
                    break;

                remaining -= item.Amount;

                Destroy(item.gameObject);
            }
        }
    }

    void SpawnOutput(
        AssemblyRecipeData recipe)
    {
        GameObject obj =
            Instantiate(
                recipe.output.itemData.prefab,
                outputPoint.position,
                outputPoint.rotation
            );

        PhysicalItem item =
            obj.GetComponent<PhysicalItem>();

        if (item != null)
        {
            item.SetAmount(
                recipe.output.amount
            );

            item.SetItemData(
                recipe.output.itemData
            );
        }
    }

    void Log(string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[AssemblyProcessor] " +
            message,
            this
        );
    }
}