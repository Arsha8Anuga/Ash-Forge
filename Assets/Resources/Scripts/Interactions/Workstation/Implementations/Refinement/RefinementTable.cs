using System.Collections.Generic;
using UnityEngine;

public class RefinementTable : WorkstationBase
{
    [Header("Refinement Validation")]
    [SerializeField]
    private bool requireWeaponPart = true;

    [SerializeField]
    private bool allowRefinableTaggedItems = true;

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

        if (FindRefinableSource(
            selectedItems) == null)
        {
            Log("Refinement rejected: no valid weapon part/refinable item.");
            return false;
        }

        return true;
    }

    PhysicalItem FindRefinableSource(
        List<PhysicalItem> items)
    {
        if (items == null)
            return null;

        foreach (PhysicalItem item
            in items)
        {
            if (item == null ||
                !item.IsValid)
            {
                continue;
            }

            WeaponPartInstanceHolder holder =
                item.GetComponent
                <WeaponPartInstanceHolder>();

            if (holder != null)
                return item;

            if (!requireWeaponPart &&
                allowRefinableTaggedItems &&
                item.ItemData != null &&
                item.ItemData.HasProcessTag(
                    ProcessTag.Refinable))
            {
                return item;
            }
        }

        return null;
    }
}
