using System.Collections.Generic;
using UnityEngine;

public class SmithingOutputBuilder :
    MonoBehaviour,
    IWorkstationOutputBuilder,
    IWorkstationSpawnPostProcessor
{
    [SerializeField]
    private bool buildOutputsHere = true;

    [SerializeField]
    [Range(0f, 100f)]
    private float defaultProductionQuality = 80f;

    [SerializeField]
    [Range(0f, 100f)]
    private float defaultDefect = 0f;

    [SerializeField]
    private bool preferRecipeQuality = true;

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
                ItemQualityCalculator
                .CreateOutputInstance(
                    inputItems,
                    GetProductionQuality(recipe),
                    GetDefect(recipe)
                );

            result.Add(
                new StoredItemStack(
                    output.itemData,
                    output.amount,
                    instance
                )
            );

            Log(
                "Smithing output: " +
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

        Log(
            "Applied smithing result to weapon part."
        );
    }

    float GetProductionQuality(
        WorkstationRecipeData recipe)
    {
        if (preferRecipeQuality &&
            recipe != null)
        {
            return Mathf.Clamp(
                recipe.baseQuality,
                0f,
                100f
            );
        }

        return defaultProductionQuality;
    }

    float GetDefect(
        WorkstationRecipeData recipe)
    {
        if (preferRecipeQuality &&
            recipe != null)
        {
            return Mathf.Clamp(
                recipe.baseDefect,
                0f,
                100f
            );
        }

        return defaultDefect;
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[SmithingOutputBuilder] " +
            message,
            this
        );
    }
}
