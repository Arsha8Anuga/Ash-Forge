using System.Collections.Generic;
using UnityEngine;

public class WeaponWorkbenchOutputBuilder :
    MonoBehaviour,
    IWorkstationOutputBuilder,
    IWorkstationSpawnPostProcessor
{
    [SerializeField]
    private bool debugLog;

    public bool CanBuildOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (recipe == null)
            return false;

        if (recipe.recipeType ==
            WorkstationRecipeType.Assembly)
        {
            return HasWeaponParts(inputItems);
        }

        if (recipe.recipeType ==
            WorkstationRecipeType.Disassembly)
        {
            return HasWeaponInstance(inputItems);
        }

        return false;
    }

        public List<StoredItemStack> BuildOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (recipe == null)
        {
            return new List<StoredItemStack>();
        }

        if (recipe.recipeType ==
            WorkstationRecipeType.Assembly)
        {
            return BuildAssemblyOutputs(
                recipe,
                inputItems
            );
        }

        if (recipe.recipeType ==
            WorkstationRecipeType.Disassembly)
        {
            return BuildDisassemblyOutputs(
                recipe,
                inputItems
            );
        }

        return new List<StoredItemStack>();
    }

    List<StoredItemStack> BuildAssemblyOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        List<StoredItemStack> result =
            new List<StoredItemStack>();

        List<WeaponPartInstance> parts =
            CollectParts(inputItems);

        if (parts.Count <= 0)
            return result;

        WeaponStatBlock stats =
            WeaponStatBuilder.BuildStats(parts);

        WeaponInstance weaponInstance =
            new WeaponInstance(
                parts,
                stats
            );

        if (recipe.outputs == null)
            return result;

        foreach (WorkstationOutput output
            in recipe.outputs)
        {
            if (output == null ||
                output.itemData == null)
            {
                continue;
            }

            StoredItemStack stack =
                new StoredItemStack(
                    output.itemData,
                    output.amount,
                    output.itemData.CreateInstance(),
                    weaponInstance
                );

            result.Add(stack);

            Log(
                "Assembly output: " +
                output.itemData.itemName
            );
        }

        return result;
    }

    List<StoredItemStack> BuildDisassemblyOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        List<StoredItemStack> result =
            new List<StoredItemStack>();

        WeaponInstance weapon =
            FindWeaponInstance(inputItems);

        if (weapon == null ||
            weapon.Parts == null)
        {
            return result;
        }

        foreach (WeaponPartInstance part
            in weapon.Parts)
        {
            if (part == null ||
                part.PartData == null)
            {
                continue;
            }

            ItemData itemData =
                part.PartData.outputItemData;

            if (itemData == null)
                continue;

            ItemInstanceData instance =
                part.ItemInstance != null
                ? part.ItemInstance.Clone()
                : itemData.CreateInstance();

            StoredItemStack stack =
                new StoredItemStack(
                    itemData,
                    1,
                    instance
                );

            result.Add(stack);

            Log(
                "Disassembly output: " +
                itemData.itemName
            );
        }

        return result;
    }

    public void ProcessSpawnedObject(
        GameObject spawnedObject,
        StoredItemStack stack,
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (spawnedObject == null ||
            stack == null)
        {
            return;
        }

        WeaponInstanceHolder weaponHolder =
            spawnedObject.GetComponent
            <WeaponInstanceHolder>();

        if (weaponHolder != null &&
            stack.WeaponInstance != null)
        {
            weaponHolder.SetInstance(
                stack.WeaponInstance
            );
        }

        WeaponPartInstanceHolder partHolder =
            spawnedObject.GetComponent
            <WeaponPartInstanceHolder>();

        if (partHolder != null)
        {
            partHolder.BuildInstance();
        }
    }

    bool HasWeaponParts(
        List<PhysicalItem> inputItems)
    {
        if (inputItems == null)
            return false;

        foreach (PhysicalItem item
            in inputItems)
        {
            if (item == null)
                continue;

            if (item.GetComponent
                <WeaponPartInstanceHolder>() != null)
            {
                return true;
            }
        }

        return false;
    }

    bool HasWeaponInstance(
        List<PhysicalItem> inputItems)
    {
        return FindWeaponInstance(inputItems)
            != null;
    }

    WeaponInstance FindWeaponInstance(
        List<PhysicalItem> inputItems)
    {
        if (inputItems == null)
            return null;

        foreach (PhysicalItem item
            in inputItems)
        {
            if (item == null)
                continue;

            WeaponInstanceHolder holder =
                item.GetComponent
                <WeaponInstanceHolder>();

            if (holder == null)
                continue;

            if (holder.Instance != null)
                return holder.Instance;
        }

        return null;
    }

    List<WeaponPartInstance> CollectParts(
        List<PhysicalItem> inputItems)
    {
        List<WeaponPartInstance> result =
            new List<WeaponPartInstance>();

        if (inputItems == null)
            return result;

        foreach (PhysicalItem item
            in inputItems)
        {
            if (item == null)
                continue;

            WeaponPartInstanceHolder holder =
                item.GetComponent
                <WeaponPartInstanceHolder>();

            if (holder == null)
                continue;

            if (holder.Instance == null)
                holder.BuildInstance();

            if (holder.Instance != null)
            {
                result.Add(
                    holder.Instance.Clone()
                );
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
            "[WeaponWorkbenchOutputBuilder] " +
            message,
            this
        );
    }
}