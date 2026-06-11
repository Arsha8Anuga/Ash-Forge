using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RefinementRecipeProfile
{
    public WorkstationRecipeData recipe;

    public RefinementProfileData profile;
}

public class RefinementOutputBuilder :
    MonoBehaviour,
    IWorkstationOutputBuilder,
    IWorkstationSpawnPostProcessor
{
    [Header("Profile")]
    [SerializeField]
    private RefinementProfileData defaultProfile;

    [SerializeField]
    private RefinementRecipeProfile[] recipeProfiles;

    [Header("Fallback Profile When No Asset Is Assigned")]
    [SerializeField]
    private float fallbackPurityDelta = -2f;

    [SerializeField]
    private float fallbackHardnessDelta = 8f;

    [SerializeField]
    private float fallbackDurabilityDelta = 4f;

    [SerializeField]
    private float fallbackConductivityDelta = -2f;

    [SerializeField]
    private float fallbackStabilityDelta = -4f;

    [SerializeField]
    private float fallbackDefectResistanceDelta = -5f;

    [SerializeField]
    private float fallbackProductionQualityDelta = 12f;

    [SerializeField]
    private float fallbackDefectDelta = 3f;

    [Header("Output")]
    [SerializeField]
    private bool useFirstRecipeOutputItem = true;

    [SerializeField]
    private bool outputSameItemWhenRecipeOutputMissing = true;

    [SerializeField]
    private bool requireWeaponPart = true;

    [SerializeField]
    private bool allowRefinableTaggedItems = true;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    public bool CanBuildOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (recipe == null)
            return false;

        PhysicalItem source =
            FindRefinableSource(inputItems);

        if (source == null)
            return false;

        return ResolveOutputItem(
            recipe,
            source
        ) != null;
    }

    public List<StoredItemStack> BuildOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        List<StoredItemStack> result =
            new List<StoredItemStack>();

        PhysicalItem source =
            FindRefinableSource(inputItems);

        if (source == null)
            return result;

        ItemData outputItem =
            ResolveOutputItem(
                recipe,
                source
            );

        if (outputItem == null)
            return result;

        ItemInstanceData refinedInstance =
            BuildRefinedInstance(
                source,
                outputItem,
                recipe
            );

        int amount =
            ResolveOutputAmount(recipe);

        result.Add(
            new StoredItemStack(
                outputItem,
                amount,
                refinedInstance
            )
        );

        Log(
            "Refined output: " +
            outputItem.itemName
        );

        return result;
    }

    public void ProcessSpawnedObject(
        GameObject spawnedObject,
        StoredItemStack stack,
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (spawnedObject == null)
            return;

        WeaponPartInstanceHolder spawnedHolder =
            spawnedObject.GetComponent
            <WeaponPartInstanceHolder>();

        if (spawnedHolder == null)
            return;

        PhysicalItem source =
            FindRefinableSource(inputItems);

        float productionQuality =
            BuildRefinedProductionQuality(
                source,
                recipe
            );

        float defect =
            BuildRefinedDefect(
                source,
                recipe
            );

        spawnedHolder.SetProductionResult(
            productionQuality,
            defect
        );

        Log(
            "Applied refinement to weapon part: quality " +
            productionQuality +
            ", defect " +
            defect
        );
    }

    ItemInstanceData BuildRefinedInstance(
        PhysicalItem source,
        ItemData outputItem,
        WorkstationRecipeData recipe)
    {
        if (source != null)
            source.EnsureInstanceData();

        ItemQualityStats sourceQuality =
            source != null &&
            source.InstanceData != null &&
            source.InstanceData.Quality != null
            ? source.InstanceData.Quality.Clone()
            : BuildDefaultQuality(outputItem);

        ItemQualityStats refinedQuality =
            ApplyProfile(
                sourceQuality,
                ResolveProfile(recipe)
            );

        ItemInstanceData instance =
            source != null &&
            source.InstanceData != null
            ? source.InstanceData.Clone()
            : new ItemInstanceData(refinedQuality);

        instance.SetQuality(refinedQuality);

        if (source != null &&
            source.ItemData != null)
        {
            instance.AddOrigin(
                new ItemOriginRecord(
                    source.ItemData,
                    1,
                    sourceQuality
                )
            );
        }

        return instance;
    }

    ItemQualityStats BuildDefaultQuality(
        ItemData outputItem)
    {
        if (outputItem == null)
            return new ItemQualityStats();

        ItemInstanceData instance =
            outputItem.CreateInstance();

        return instance != null &&
            instance.Quality != null
            ? instance.Quality.Clone()
            : new ItemQualityStats();
    }

    ItemQualityStats ApplyProfile(
        ItemQualityStats source,
        RefinementProfileData profile)
    {
        if (profile != null)
        {
            return profile.ApplyTo(source);
        }

        ItemQualityStats result =
            source != null
            ? source.Clone()
            : new ItemQualityStats();

        result.purity += fallbackPurityDelta;
        result.hardness += fallbackHardnessDelta;
        result.durability += fallbackDurabilityDelta;
        result.conductivity += fallbackConductivityDelta;
        result.stability += fallbackStabilityDelta;
        result.defectResistance += fallbackDefectResistanceDelta;

        result.Clamp();

        return result;
    }

    float BuildRefinedProductionQuality(
        PhysicalItem source,
        WorkstationRecipeData recipe)
    {
        float value =
            recipe != null
            ? recipe.baseQuality
            : 80f;

        WeaponPartInstanceHolder holder =
            source != null
            ? source.GetComponent
                <WeaponPartInstanceHolder>()
            : null;

        if (holder != null &&
            holder.Instance != null)
        {
            value =
                holder.Instance.ProductionQuality;
        }

        RefinementProfileData profile =
            ResolveProfile(recipe);

        if (profile != null)
        {
            return profile.ApplyProductionQuality(
                value
            );
        }

        return Mathf.Clamp(
            value + fallbackProductionQualityDelta,
            0f,
            100f
        );
    }

    float BuildRefinedDefect(
        PhysicalItem source,
        WorkstationRecipeData recipe)
    {
        float value =
            recipe != null
            ? recipe.baseDefect
            : 0f;

        WeaponPartInstanceHolder holder =
            source != null
            ? source.GetComponent
                <WeaponPartInstanceHolder>()
            : null;

        if (holder != null &&
            holder.Instance != null)
        {
            value =
                holder.Instance.DefectLevel;
        }

        RefinementProfileData profile =
            ResolveProfile(recipe);

        if (profile != null)
        {
            return profile.ApplyDefect(value);
        }

        return Mathf.Clamp(
            value + fallbackDefectDelta,
            0f,
            100f
        );
    }

    PhysicalItem FindRefinableSource(
        List<PhysicalItem> inputItems)
    {
        if (inputItems == null)
            return null;

        foreach (PhysicalItem item
            in inputItems)
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

    ItemData ResolveOutputItem(
        WorkstationRecipeData recipe,
        PhysicalItem source)
    {
        if (useFirstRecipeOutputItem &&
            recipe != null &&
            recipe.outputs != null &&
            recipe.outputs.Length > 0 &&
            recipe.outputs[0] != null &&
            recipe.outputs[0].itemData != null)
        {
            return recipe.outputs[0].itemData;
        }

        if (outputSameItemWhenRecipeOutputMissing &&
            source != null)
        {
            return source.ItemData;
        }

        return null;
    }

    int ResolveOutputAmount(
        WorkstationRecipeData recipe)
    {
        if (recipe != null &&
            recipe.outputs != null &&
            recipe.outputs.Length > 0 &&
            recipe.outputs[0] != null)
        {
            return Mathf.Max(
                1,
                recipe.outputs[0].amount
            );
        }

        return 1;
    }

    RefinementProfileData ResolveProfile(
        WorkstationRecipeData recipe)
    {
        if (recipeProfiles != null &&
            recipe != null)
        {
            foreach (RefinementRecipeProfile entry
                in recipeProfiles)
            {
                if (entry == null)
                    continue;

                if (entry.recipe == recipe &&
                    entry.profile != null)
                {
                    return entry.profile;
                }
            }
        }

        return defaultProfile;
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[RefinementOutputBuilder] " +
            message,
            this
        );
    }
}
