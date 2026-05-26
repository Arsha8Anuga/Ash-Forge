using System.Collections.Generic;
using UnityEngine;

public class AssemblyRecipeMatcher :
    MonoBehaviour
{
    [SerializeField]
    private AssemblyRecipeDatabase database;

    [SerializeField]
    private bool allowPartialMatchForPreview = true;

    void Awake()
    {
        if (database == null)
        {
            database =
                GetComponent<AssemblyRecipeDatabase>();
        }
    }

    public bool TryFindRecipe(
        List<PhysicalItem> items,
        out AssemblyRecipeData recipe)
    {
        recipe = null;

        List<AssemblyRecipeData> matches =
            FindFullMatches(items);

        if (matches.Count <= 0)
            return false;

        recipe = matches[0];
        return true;
    }

    public List<AssemblyRecipeData> FindFullMatches(
        List<PhysicalItem> items)
    {
        List<AssemblyRecipeData> result =
            new List<AssemblyRecipeData>();

        if (database == null ||
            database.Recipes == null)
        {
            return result;
        }

        foreach (AssemblyRecipeData candidate
            in database.Recipes)
        {
            if (candidate == null)
                continue;

            AssemblyRecipeMatch match =
                EvaluateRecipe(
                    candidate,
                    items
                );

            if (match != null &&
                match.IsFullyMatched)
            {
                result.Add(candidate);
            }
        }

        result.Sort(
            (a, b) =>
                GetTotalRequirementAmount(b)
                .CompareTo(
                    GetTotalRequirementAmount(a)
                )
        );

        Debug.Log(
            "[AssemblyMatcher] Full matches: " +
            result.Count
        );

        return result;
    }

    public AssemblyRecipeMatch FindBestMatch(
        List<PhysicalItem> items)
    {
        if (database == null ||
            database.Recipes == null)
        {
            return null;
        }

        AssemblyRecipeMatch best = null;

        foreach (AssemblyRecipeData candidate
            in database.Recipes)
        {
            if (candidate == null)
                continue;

            AssemblyRecipeMatch match =
                EvaluateRecipe(
                    candidate,
                    items
                );

            if (match == null)
                continue;

            if (!allowPartialMatchForPreview &&
                !match.IsFullyMatched)
            {
                continue;
            }

            if (best == null ||
                match.Score > best.Score)
            {
                best = match;
            }
        }

        return best;
    }

    AssemblyRecipeMatch EvaluateRecipe(
        AssemblyRecipeData recipe,
        List<PhysicalItem> items)
    {
        if (recipe.requirements == null ||
            recipe.requirements.Length == 0)
        {
            return null;
        }

        int totalRequired = 0;
        int totalMatched = 0;
        int matchedTypes = 0;
        int missingTotal = 0;

        foreach (AssemblyRequirement requirement
            in recipe.requirements)
        {
            if (requirement == null ||
                requirement.itemData == null)
            {
                return null;
            }

            int requiredAmount =
                Mathf.Max(
                    1,
                    requirement.amount
                );

            int availableAmount =
                CountItem(
                    items,
                    requirement.itemData
                );

            int matchedAmount =
                Mathf.Min(
                    requiredAmount,
                    availableAmount
                );

            totalRequired += requiredAmount;
            totalMatched += matchedAmount;

            if (matchedAmount > 0)
                matchedTypes++;

            if (availableAmount < requiredAmount)
            {
                missingTotal +=
                    requiredAmount -
                    availableAmount;
            }
        }

        bool fullyMatched =
            missingTotal == 0;

        int extraItems =
            CountAllItems(items) -
            totalMatched;

        float coverage =
            totalRequired > 0
            ? (float)totalMatched /
              totalRequired
            : 0f;

        float specificity =
            totalRequired;

        float typeCoverage =
            recipe.requirements.Length > 0
            ? (float)matchedTypes /
              recipe.requirements.Length
            : 0f;

        float score =
            coverage * 1000f +
            typeCoverage * 200f +
            specificity * 10f;

        if (fullyMatched)
        {
            score += 10000f;
        }
        else
        {
            score -= missingTotal * 500f;
        }

        score -= extraItems * 2f;

        return new AssemblyRecipeMatch(
            recipe,
            fullyMatched,
            score,
            totalMatched,
            totalRequired,
            missingTotal
        );
    }

    int CountItem(
        List<PhysicalItem> items,
        ItemData itemData)
    {
        int count = 0;

        if (items == null)
            return 0;

        foreach (PhysicalItem item in items)
        {
            if (item == null)
                continue;

            if (item.ItemData != itemData)
                continue;

            count += item.Amount;
        }

        return count;
    }

    int CountAllItems(
        List<PhysicalItem> items)
    {
        int count = 0;

        if (items == null)
            return 0;

        foreach (PhysicalItem item in items)
        {
            if (item == null)
                continue;

            count += item.Amount;
        }

        return count;
    }

    int GetTotalRequirementAmount(
        AssemblyRecipeData recipe)
    {
        if (recipe == null ||
            recipe.requirements == null)
        {
            return 0;
        }

        int total = 0;

        foreach (AssemblyRequirement req
            in recipe.requirements)
        {
            if (req == null)
                continue;

            total += Mathf.Max(
                1,
                req.amount
            );
        }

        return total;
    }
}