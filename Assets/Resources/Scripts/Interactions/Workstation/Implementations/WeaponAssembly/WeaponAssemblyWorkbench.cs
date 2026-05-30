using System.Collections.Generic;
using UnityEngine;

public class WeaponAssemblyWorkbench :
    GenericWorkstation
{
    [Header("Weapon Validation")]
    [SerializeField]
    private bool requireWeaponPartForAssembly = true;

    [SerializeField]
    private bool requireWeaponForDisassembly = true;

    [SerializeField]
    private WeaponPartRole[] requiredAssemblyRoles =
    {
        WeaponPartRole.Receiver,
        WeaponPartRole.Barrel,
        WeaponPartRole.TriggerGroup
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

        return CanProcessWeaponRecipe(
            recipe,
            selectedItems
        );
    }

    public bool CanProcessWeaponRecipe(
        WorkstationRecipeData recipe,
        List<PhysicalItem> items)
    {
        if (recipe == null)
            return false;

        if (recipe.recipeType ==
            WorkstationRecipeType.Assembly)
        {
            return CanAssembleWeapon(items);
        }

        if (recipe.recipeType ==
            WorkstationRecipeType.Disassembly)
        {
            return CanDisassembleWeapon(items);
        }

        return true;
    }

    bool CanAssembleWeapon(
        List<PhysicalItem> items)
    {
        if (!requireWeaponPartForAssembly)
            return true;

        if (items == null ||
            items.Count <= 0)
        {
            Log("No input items for weapon assembly.");
            return false;
        }

        foreach (WeaponPartRole role
            in requiredAssemblyRoles)
        {
            if (!HasWeaponPartRole(items, role))
            {
                Log(
                    "Missing weapon part role: " +
                    role
                );

                return false;
            }
        }

        return true;
    }

    bool CanDisassembleWeapon(
        List<PhysicalItem> items)
    {
        if (!requireWeaponForDisassembly)
            return true;

        if (items == null ||
            items.Count <= 0)
        {
            Log("No input items for weapon disassembly.");
            return false;
        }

        foreach (PhysicalItem item in items)
        {
            if (item == null)
                continue;

            WeaponInstanceHolder weapon =
                item.GetComponent
                <WeaponInstanceHolder>();

            if (weapon != null &&
                weapon.Instance != null)
            {
                return true;
            }
        }

        Log("No weapon instance detected.");
        return false;
    }

    bool HasWeaponPartRole(
        List<PhysicalItem> items,
        WeaponPartRole role)
    {
        foreach (PhysicalItem item in items)
        {
            if (item == null)
                continue;

            WeaponPartInstanceHolder part =
                item.GetComponent
                <WeaponPartInstanceHolder>();

            if (part == null ||
                part.PartData == null)
            {
                continue;
            }

            if (part.PartData.role == role)
                return true;
        }

        return false;
    }
}