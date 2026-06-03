using System.Collections.Generic;
using UnityEngine;

public static class WorkstationTextBuilder
{
    public static string BuildRecipeHeader(
        WorkstationRecipeData recipe)
    {
        if (recipe == null)
            return "Recipe: ?";

        return
            recipe.recipeName +
            "\nType: " +
            recipe.recipeType +
            "\nReversible: " +
            recipe.reversibility;
    }

    public static string BuildRequirementText(
        WorkstationRecipeData recipe)
    {
        if (recipe == null ||
            recipe.requirements == null ||
            recipe.requirements.Length == 0)
        {
            return "Input: ?";
        }

        string text = "Input:\n";

        foreach (WorkstationRequirement req
            in recipe.requirements)
        {
            if (req == null ||
                req.itemData == null)
                continue;

            text +=
                "- " +
                req.itemData.itemName +
                " x" +
                req.amount +
                "\n";
        }

        return text;
    }

    public static string BuildOutputText(
        WorkstationRecipeData recipe)
    {
        if (recipe == null ||
            recipe.outputs == null ||
            recipe.outputs.Length == 0)
        {
            return "Output: ?";
        }

        string text = "Output:\n";

        foreach (WorkstationOutput output
            in recipe.outputs)
        {
            if (output == null ||
                output.itemData == null)
                continue;

            text +=
                "- " +
                output.itemData.itemName +
                " x" +
                output.amount +
                "\n";
        }

        return text;
    }

    public static string BuildRuleText(
        WorkstationRecipeData recipe)
    {
        if (recipe == null)
            return "";

        string text =
            "Rule:\n" +
            recipe.reversibility;

        if (recipe.isDestructive)
            text += "\nDestructive";

        if (!string.IsNullOrEmpty(
            recipe.ruleNote))
        {
            text +=
                "\n" +
                recipe.ruleNote;
        }

        return text;
    }

    public static string BuildStepText(
        WorkstationRecipeData recipe,
        int activeIndex)
    {
        if (recipe == null ||
            recipe.toolSteps == null ||
            recipe.toolSteps.Length == 0)
        {
            return "Steps: Auto";
        }

        string text = "Steps:\n";

        for (int i = 0;
            i < recipe.toolSteps.Length;
            i++)
        {
            WorkstationToolStep step =
                recipe.toolSteps[i];

            if (step == null)
                continue;

            string prefix =
                i == activeIndex
                ? "> "
                : "- ";

            string done =
                i < activeIndex
                ? " done"
                : "";

            text +=
                prefix +
                step.toolType +
                done +
                "\n";
        }

        return text;
    }

    public static string BuildRecipeDetailText(
        WorkstationRecipeData recipe)
    {
        if (recipe == null)
            return "Recipe Detail:\n- none";

        string text =
            "Recipe Detail:\n";

        text +=
            "Type: " +
            recipe.recipeType +
            "\n";

        text +=
            "Reversibility: " +
            recipe.reversibility +
            "\n";

        text +=
            "Process Time: " +
            recipe.processTime.ToString("0.##") +
            "s\n\n";

        text +=
            BuildRequirementText(recipe) +
            "\n";

        text +=
            BuildOutputText(recipe) +
            "\n";

        text +=
            BuildRuleText(recipe) +
            "\n";

        return text;
    }

    public static string BuildDetectedItemsText(
        List<PhysicalItem> items)
    {
        return BuildItemListText(
            "Detected Items",
            items
        );
    }

    public static string BuildSelectedItemsText(
        List<PhysicalItem> items)
    {
        return BuildItemListText(
            "Selected Items",
            items
        );
    }

    static string BuildItemListText(
        string title,
        List<PhysicalItem> items)
    {
        if (items == null ||
            items.Count == 0)
        {
            return title + ":\n- none";
        }

        Dictionary<ItemData, int> counts =
            new Dictionary<ItemData, int>();

        foreach (PhysicalItem item
            in items)
        {
            if (item == null)
                continue;

            if (!item.IsValid)
                continue;

            ItemData data =
                item.ItemData;

            if (data == null)
                continue;

            if (!counts.ContainsKey(data))
            {
                counts.Add(
                    data,
                    0
                );
            }

            counts[data] +=
                Mathf.Max(
                    1,
                    item.Amount
                );
        }

        if (counts.Count == 0)
            return title + ":\n- none";

        string text =
            title +
            ":\n";

        foreach (KeyValuePair<ItemData, int> pair
            in counts)
        {
            if (pair.Key == null)
                continue;

            text +=
                "- " +
                pair.Key.itemName +
                " x" +
                pair.Value +
                "\n";
        }

        return text;
    }
}