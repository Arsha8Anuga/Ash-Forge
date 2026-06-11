using System.Collections.Generic;
using UnityEngine;

public class WoodenWorkbenchOutputBuilder :
    MonoBehaviour,
    IWorkstationOutputBuilder,
    IWorkstationSpawnPostProcessor
{
    [Header("Output")]
    [SerializeField]
    private bool buildOutputsHere = true;

    [SerializeField]
    private bool preferRecipeQuality = true;

    [Header("Wood Quality Modifier")]
    [SerializeField]
    private float purityDelta = 1f;

    [SerializeField]
    private float hardnessDelta = -3f;

    [SerializeField]
    private float durabilityDelta = 3f;

    [SerializeField]
    private float conductivityDelta = -6f;

    [SerializeField]
    private float stabilityDelta = 8f;

    [SerializeField]
    private float defectResistanceDelta = 3f;

    [Header("Weapon Part Result")]
    [SerializeField]
    private float productionQualityBonus = 3f;

    [SerializeField]
    private float defectPenalty = 0f;

    [Header("Fallback")]
    [SerializeField]
    [Range(0f, 100f)]
    private float defaultProductionQuality = 80f;

    [SerializeField]
    [Range(0f, 100f)]
    private float defaultDefect = 1f;

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
                BuildWoodInstance(
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
                "Wooden workbench output: " +
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

        Log("Applied wooden workbench weapon part result.");
    }

    ItemInstanceData BuildWoodInstance(
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
            "[WoodenWorkbenchOutputBuilder] " +
            message,
            this
        );
    }
}
