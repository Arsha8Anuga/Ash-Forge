using System.Collections.Generic;
using UnityEngine;

public class MetalWorkbenchOutputBuilder :
    MonoBehaviour,
    IWorkstationOutputBuilder,
    IWorkstationSpawnPostProcessor
{
    [Header("Output")]
    [SerializeField]
    private bool buildOutputsHere = true;

    [SerializeField]
    private bool preferRecipeQuality = true;

    [Header("Metal Quality Modifier")]
    [SerializeField]
    private float purityDelta = 0f;

    [SerializeField]
    private float hardnessDelta = 8f;

    [SerializeField]
    private float durabilityDelta = 6f;

    [SerializeField]
    private float conductivityDelta = 2f;

    [SerializeField]
    private float stabilityDelta = -2f;

    [SerializeField]
    private float defectResistanceDelta = -1f;

    [Header("Weapon Part Result")]
    [SerializeField]
    private float productionQualityBonus = 4f;

    [SerializeField]
    private float defectPenalty = 1f;

    [Header("Fallback")]
    [SerializeField]
    [Range(0f, 100f)]
    private float defaultProductionQuality = 78f;

    [SerializeField]
    [Range(0f, 100f)]
    private float defaultDefect = 2f;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    public bool CanBuildOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (!buildOutputsHere)
            return false;

        if (recipe == null)
            return false;

        return recipe.outputs != null &&
            recipe.outputs.Length > 0;
    }

    public List<StoredItemStack> BuildOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        List<StoredItemStack> result =
            new List<StoredItemStack>();

        if (recipe == null ||
            recipe.outputs == null)
        {
            return result;
        }

        foreach (WorkstationOutput output
            in recipe.outputs)
        {
            if (output == null ||
                output.itemData == null)
            {
                continue;
            }

            ItemInstanceData instance =
                BuildMetalInstance(
                    output.itemData,
                    recipe,
                    inputItems
                );

            result.Add(
                new StoredItemStack(
                    output.itemData,
                    output.amount,
                    instance
                )
            );

            Log(
                "Metal workbench output: " +
                output.itemData.itemName
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
        if (spawnedObject == null)
            return;

        WeaponPartInstanceHolder partHolder =
            spawnedObject.GetComponent
            <WeaponPartInstanceHolder>();

        if (partHolder == null)
            return;

        partHolder.SetProductionResult(
            GetProductionQuality(recipe),
            GetDefect(recipe)
        );

        Log("Applied metal workbench weapon part result.");
    }

    ItemInstanceData BuildMetalInstance(
        ItemData outputItem,
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        ItemInstanceData instance =
            ItemQualityCalculator.CreateOutputInstance(
                inputItems,
                GetProductionQuality(recipe),
                GetDefect(recipe)
            );

        if (instance == null)
        {
            instance =
                outputItem != null
                ? outputItem.CreateInstance()
                : new ItemInstanceData(
                    new ItemQualityStats()
                );
        }

        ItemQualityStats quality =
            instance.Quality != null
            ? instance.Quality.Clone()
            : new ItemQualityStats();

        quality.purity += purityDelta;
        quality.hardness += hardnessDelta;
        quality.durability += durabilityDelta;
        quality.conductivity += conductivityDelta;
        quality.stability += stabilityDelta;
        quality.defectResistance += defectResistanceDelta;
        quality.Clamp();

        instance.SetQuality(quality);

        return instance;
    }

    float GetProductionQuality(
        WorkstationRecipeData recipe)
    {
        float value =
            preferRecipeQuality && recipe != null
            ? recipe.baseQuality
            : defaultProductionQuality;

        return Mathf.Clamp(
            value + productionQualityBonus,
            0f,
            100f
        );
    }

    float GetDefect(
        WorkstationRecipeData recipe)
    {
        float value =
            preferRecipeQuality && recipe != null
            ? recipe.baseDefect
            : defaultDefect;

        return Mathf.Clamp(
            value + defectPenalty,
            0f,
            100f
        );
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[MetalWorkbenchOutputBuilder] " +
            message,
            this
        );
    }
}
