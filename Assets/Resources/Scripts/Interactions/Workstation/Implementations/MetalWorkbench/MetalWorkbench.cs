using System.Collections.Generic;
using UnityEngine;

public class MetalWorkbench : WorkstationBase
{
    [Header("Metal Workbench Validation")]
    [SerializeField]
    private bool requireMetalInput = true;

    [SerializeField]
    private bool requireToolSteps = false;

    [SerializeField]
    private bool allowWeaponPartInputs = true;

    [SerializeField]
    private ItemData[] explicitMetalItems;

    [SerializeField]
    private ItemCategory[] allowedCategories =
    {
        ItemCategory.RawMaterial,
        ItemCategory.RefinedMaterial,
        ItemCategory.Component,
        ItemCategory.WeaponPart
    };

    [SerializeField]
    private ProcessTag[] allowedProcessTags =
    {
        ProcessTag.Assemblable,
        ProcessTag.Refinable
    };

    [SerializeField]
    private string[] metalNameKeywords =
    {
        "metal",
        "iron",
        "steel",
        "copper",
        "brass",
        "aluminum",
        "aluminium",
        "alloy",
        "ingot",
        "plate",
        "bar",
        "pipe",
        "receiver",
        "barrel",
        "trigger",
        "bolt",
        "besi",
        "baja",
        "logam",
        "tembaga",
        "kuningan"
    };

    protected override bool CanStartRecipe(
        WorkstationRecipeData recipe,
        List<PhysicalItem> selectedItems)
    {
        if (!base.CanStartRecipe(
            recipe,
            selectedItems))
        {
            return false;
        }

        if (recipe == null)
            return false;

        if (requireToolSteps &&
            !recipe.HasToolSteps)
        {
            Log("Metal workbench rejected: recipe has no tool steps.");
            return false;
        }

        if (requireMetalInput &&
            !HasMetalInput(selectedItems))
        {
            Log("Metal workbench rejected: no valid metal input.");
            return false;
        }

        return true;
    }

    bool HasMetalInput(
        List<PhysicalItem> items)
    {
        if (items == null)
            return false;

        foreach (PhysicalItem item in items)
        {
            if (IsMetalInput(item))
                return true;
        }

        return false;
    }

    bool IsMetalInput(
        PhysicalItem item)
    {
        if (item == null ||
            !item.IsValid ||
            item.ItemData == null)
        {
            return false;
        }

        if (MatchesExplicitItem(item.ItemData))
            return true;

        if (allowWeaponPartInputs &&
            item.GetComponent<WeaponPartInstanceHolder>() != null)
        {
            return true;
        }

        if (MatchesAllowedProcessTag(item.ItemData))
            return true;

        if (!MatchesAllowedCategory(item.ItemData.category))
            return false;

        return MatchesKeyword(item.ItemData);
    }

    bool MatchesExplicitItem(
        ItemData itemData)
    {
        if (itemData == null ||
            explicitMetalItems == null)
        {
            return false;
        }

        foreach (ItemData metalItem
            in explicitMetalItems)
        {
            if (metalItem == null)
                continue;

            if (metalItem == itemData)
                return true;
        }

        return false;
    }

    bool MatchesAllowedCategory(
        ItemCategory category)
    {
        if (allowedCategories == null ||
            allowedCategories.Length == 0)
        {
            return true;
        }

        foreach (ItemCategory allowed
            in allowedCategories)
        {
            if (allowed == category)
                return true;
        }

        return false;
    }

    bool MatchesAllowedProcessTag(
        ItemData itemData)
    {
        if (itemData == null ||
            allowedProcessTags == null)
        {
            return false;
        }

        foreach (ProcessTag tag
            in allowedProcessTags)
        {
            if (itemData.HasProcessTag(tag))
                return true;
        }

        return false;
    }

    bool MatchesKeyword(
        ItemData itemData)
    {
        if (itemData == null ||
            metalNameKeywords == null)
        {
            return false;
        }

        string id =
            itemData.itemId != null
            ? itemData.itemId.ToLowerInvariant()
            : string.Empty;

        string itemName =
            itemData.itemName != null
            ? itemData.itemName.ToLowerInvariant()
            : string.Empty;

        foreach (string keyword
            in metalNameKeywords)
        {
            if (string.IsNullOrEmpty(keyword))
                continue;

            string lowerKeyword =
                keyword.ToLowerInvariant();

            if (id.Contains(lowerKeyword) ||
                itemName.Contains(lowerKeyword))
            {
                return true;
            }
        }

        return false;
    }
}
