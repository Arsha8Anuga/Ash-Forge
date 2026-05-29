using System.Collections.Generic;
using UnityEngine;

public class WorkstationRecipeMatcher :
    MonoBehaviour
{
    [SerializeField]
    private WorkstationRecipeData[] recipes;

    [SerializeField]
    private WorkstationRecipeType[] allowedRecipeTypes;

    [SerializeField]
    private bool useTypeFilter;

    [SerializeField]
    private bool debugLog;

    public List<WorkstationRecipeData> FindFullMatches(
        List<PhysicalItem> items)
    {
        List<WorkstationRecipeData> matches =
            new List<WorkstationRecipeData>();

        if (recipes == null)
            return matches;

        foreach (WorkstationRecipeData recipe
            in recipes)
        {
            if (recipe == null)
                continue;

            if (!IsRecipeTypeAllowed(recipe))
                continue;

            if (IsMatch(recipe, items))
            {
                matches.Add(recipe);
            }
        }

        matches.Sort(
            (a, b) =>
                GetSpecificity(b)
                .CompareTo(
                    GetSpecificity(a)
                )
        );

        Log(
            "Full matches: " +
            matches.Count
        );

        return matches;
    }

    bool IsRecipeTypeAllowed(
        WorkstationRecipeData recipe)
    {
        if (!useTypeFilter)
            return true;

        if (allowedRecipeTypes == null ||
            allowedRecipeTypes.Length == 0)
        {
            return true;
        }

        foreach (WorkstationRecipeType type
            in allowedRecipeTypes)
        {
            if (recipe.recipeType == type)
                return true;
        }

        return false;
    }

    bool IsMatch(
        WorkstationRecipeData recipe,
        List<PhysicalItem> items)
    {
        if (recipe.requirements == null ||
            recipe.requirements.Length == 0)
        {
            return false;
        }

        foreach (WorkstationRequirement req
            in recipe.requirements)
        {
            if (req == null ||
                req.itemData == null)
            {
                return false;
            }

            int available =
                CountItem(
                    req.itemData,
                    items
                );

            if (available < req.amount)
                return false;
        }

        return true;
    }

    int CountItem(
        ItemData data,
        List<PhysicalItem> items)
    {
        int count = 0;

        if (items == null)
            return count;

        foreach (PhysicalItem item
            in items)
        {
            if (item == null)
                continue;

            if (!item.IsValid)
                continue;

            if (item.ItemData != data)
                continue;

            count += item.Amount;
        }

        return count;
    }

    int GetSpecificity(
        WorkstationRecipeData recipe)
    {
        int score = 0;

        if (recipe.requirements != null)
        {
            foreach (WorkstationRequirement req
                in recipe.requirements)
            {
                if (req == null)
                    continue;

                score += req.amount;
            }
        }

        if (recipe.toolSteps != null)
        {
            score +=
                recipe.toolSteps.Length;
        }

        if (recipe.recipeType ==
            WorkstationRecipeType.Disassembly)
        {
            score += 1;
        }

        return score;
    }

    bool IsFullMatch(
        WorkstationRecipeData recipe,
        List<PhysicalItem> items)
    {
        if (recipe == null ||
            recipe.requirements == null)
        {
            return false;
        }

        foreach (WorkstationRequirement req
            in recipe.requirements)
        {
            if (req == null ||
                req.itemData == null)
            {
                continue;
            }

            int available =
                CountItemAmount(
                    req.itemData,
                    items
                );

            if (available < req.amount)
                return false;
        }

        return true;
    }

    int CountItemAmount(
        ItemData itemData,
        List<PhysicalItem> items)
    {
        if (itemData == null ||
            items == null)
        {
            return 0;
        }

        int total = 0;

        foreach (PhysicalItem item in items)
        {
            if (item == null)
                continue;

            if (!item.IsValid)
                continue;

            if (item.ItemData != itemData)
                continue;

            total += item.Amount;
        }

        return total;
    }

    public List<WorkstationRecipeData> FindFullMatches(
        WorkstationRecipeData[] recipes,
        List<PhysicalItem> items)
    {
        List<WorkstationRecipeData> result =
            new List<WorkstationRecipeData>();

        if (recipes == null ||
            items == null)
        {
            return result;
        }

        foreach (WorkstationRecipeData recipe
            in recipes)
        {
            if (recipe == null)
                continue;

            if (IsFullMatch(recipe, items))
            {
                result.Add(recipe);
            }
        }

        return result;
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[WorkstationMatcher] " +
            message,
            this
        );
    }
}