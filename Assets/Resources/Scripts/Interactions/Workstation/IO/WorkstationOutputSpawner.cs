using System.Collections.Generic;
using UnityEngine;

public class WorkstationOutputSpawner :
    MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField]
    private StorageSpawnArea spawnArea;

    [SerializeField]
    private bool spawnAmountAsSeparateObjects = true;

    [Header("Routing")]
    [SerializeField]
    private WorkstationOutputRoute[] routes;

    [Header("Extension")]
    [SerializeField]
    private MonoBehaviour outputBuilderBehaviour;

    [SerializeField]
    private MonoBehaviour postProcessorBehaviour;

    [Header("Debug")]
    [SerializeField]
    protected bool debugLog;

    private IWorkstationOutputBuilder outputBuilder;

    private IWorkstationSpawnPostProcessor
        postProcessor;

    void Awake()
    {
        if (spawnArea == null)
        {
            spawnArea =
                GetComponentInChildren
                <StorageSpawnArea>();
        }

        outputBuilder =
            outputBuilderBehaviour
            as IWorkstationOutputBuilder;

        if (outputBuilder == null)
        {
            outputBuilder =
                GetComponent
                <IWorkstationOutputBuilder>();
        }

        postProcessor =
            postProcessorBehaviour
            as IWorkstationSpawnPostProcessor;

        if (postProcessor == null)
        {
            postProcessor =
                GetComponent
                <IWorkstationSpawnPostProcessor>();
        }
    }

    public void Spawn(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (recipe == null)
        {
            Log("Spawn failed: recipe null.");
            return;
        }

        List<StoredItemStack> outputs =
            BuildOutputStacks(
                recipe,
                inputItems
            );

        if (outputs == null ||
            outputs.Count <= 0)
        {
            Log("No outputs.");
            return;
        }

        foreach (StoredItemStack stack
            in outputs)
        {
            if (stack == null ||
                !stack.IsValid ||
                stack.ItemData == null)
            {
                Log("Skipped invalid output stack.");
                continue;
            }

            RouteOutput(
                stack,
                recipe,
                inputItems
            );
        }
    }

    List<StoredItemStack> BuildOutputStacks(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (recipe == null)
            return new List<StoredItemStack>();

        if (outputBuilder != null &&
            outputBuilder.CanBuildOutputs(
                recipe,
                inputItems))
        {
            return outputBuilder.BuildOutputs(
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

        return BuildDefaultOutputs(
            recipe,
            inputItems
        );
    }

    List<StoredItemStack> BuildDisassemblyOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        List<StoredItemStack> result =
            new List<StoredItemStack>();

        if (recipe == null)
            return result;

        if (recipe.reversibility ==
            WorkstationReversibility.Full ||
            recipe.reversibility ==
            WorkstationReversibility.Partial)
        {
            List<StoredItemStack> reverseOutputs =
                BuildGenericReverseOutputs(
                    recipe,
                    inputItems
                );

            if (reverseOutputs != null)
            {
                result.AddRange(
                    reverseOutputs
                );
            }
        }

        if (recipe.reversibility ==
            WorkstationReversibility.Partial ||
            recipe.reversibility ==
            WorkstationReversibility.None ||
            recipe.reversibility ==
            WorkstationReversibility.Irreversible)
        {
            List<StoredItemStack> manualOutputs =
                BuildDefaultOutputs(
                    recipe,
                    inputItems
                );

            if (manualOutputs != null)
            {
                result.AddRange(
                    manualOutputs
                );
            }
        }

        return result;
    }

    List<StoredItemStack> BuildDefaultOutputs(
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
                BuildOutputInstance(
                    output.itemData,
                    recipe,
                    inputItems
                );

            StoredItemStack stack =
                new StoredItemStack(
                    output.itemData,
                    output.amount,
                    instance
                );

            result.Add(
                stack
            );
        }

        return result;
    }

    List<StoredItemStack> BuildGenericReverseOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        List<StoredItemStack> result =
            new List<StoredItemStack>();

        if (recipe == null)
            return result;

        if (!recipe.CanReverse)
            return result;

        if (inputItems == null)
            return result;

        foreach (PhysicalItem item
            in inputItems)
        {
            if (item == null)
                continue;

            List<StoredItemStack> reversed =
                ItemReverseCalculator.Reverse(
                    item,
                    recipe.reversibility
                );

            if (reversed == null)
                continue;

            result.AddRange(
                reversed
            );
        }

        return result;
    }

    ItemInstanceData BuildOutputInstance(
        ItemData outputItemData,
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (outputItemData == null)
            return null;

        if (recipe == null)
            return outputItemData.CreateInstance();

        if (inputItems == null ||
            inputItems.Count == 0)
        {
            return outputItemData.CreateInstance();
        }

        return ItemQualityCalculator
            .CreateOutputInstance(
                inputItems,
                recipe.baseQuality,
                recipe.baseDefect
            );
    }

    void RouteOutput(
        StoredItemStack stack,
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (stack == null ||
            !stack.IsValid ||
            stack.ItemData == null)
        {
            Log("Skipped invalid output stack.");
            return;
        }

        WorkstationOutputRoute route =
            FindRoute(stack.ItemData);

        if (route != null &&
            route.target ==
            WorkstationOutputTarget.Storage)
        {
            if (TrySendToStorage(
                stack,
                route))
            {
                return;
            }

            Log(
                "Storage failed, fallback to spawn: " +
                stack.ItemData.itemName
            );
        }

        SpawnStack(
            stack,
            recipe,
            inputItems
        );
    }

    WorkstationOutputRoute FindRoute(
        ItemData itemData)
    {
        if (routes == null)
            return null;

        foreach (WorkstationOutputRoute route
            in routes)
        {
            if (route == null)
                continue;

            if (route.Matches(itemData))
                return route;
        }

        return null;
    }

    public string GetRouteSummary(
        WorkstationRecipeData recipe)
    {
        if (recipe == null)
            return "Output Route:\n- recipe missing";

        if (recipe.outputs == null ||
            recipe.outputs.Length == 0)
        {
            return "Output Route:\n- no manual output";
        }

        string text =
            "Output Route:\n";

        foreach (WorkstationOutput output
            in recipe.outputs)
        {
            if (output == null ||
                output.itemData == null)
            {
                continue;
            }

            WorkstationOutputRoute route =
                FindRoute(
                    output.itemData
                );

            text +=
                "- " +
                output.itemData.itemName +
                ": " +
                BuildRouteLine(route) +
                "\n";
        }

        return text;
    }

    string BuildRouteLine(
        WorkstationOutputRoute route)
    {
        if (route == null)
            return "Spawn Area";

        if (route.target ==
            WorkstationOutputTarget.SpawnArea)
        {
            return "Spawn Area";
        }

        if (route.target ==
            WorkstationOutputTarget.Storage)
        {
            IStackStorageInput stackStorage =
                route.GetStorage();

            if (stackStorage == null)
                return "Storage missing";

            IItemStorage itemStorage =
                route.storageBehaviour
                as IItemStorage;

            if (itemStorage != null)
            {
                return
                    "Storage " +
                    itemStorage.CurrentCount +
                    " / " +
                    itemStorage.Capacity;
            }

            return "Storage";
        }

        return route.target.ToString();
    }

    bool TrySendToStorage(
        StoredItemStack stack,
        WorkstationOutputRoute route)
    {
        if (stack == null ||
            !stack.IsValid ||
            stack.ItemData == null)
        {
            return false;
        }

        if (route == null)
            return false;

        IStackStorageInput storage =
            route.GetStorage();

        if (storage == null)
        {
            Log(
                "Storage route missing IStackStorageInput."
            );

            return false;
        }

        if (!storage.CanInputStack(stack))
        {
            Log(
                "Storage cannot receive: " +
                stack.ItemData.itemName
            );

            return false;
        }

        bool success =
            storage.InputStack(stack);

        Log(
            "Sent to storage: " +
            stack.ItemData.itemName +
            " | " +
            success
        );

        return success;
    }

    void SpawnStack(
        StoredItemStack stack,
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (stack == null ||
            !stack.IsValid ||
            stack.ItemData == null)
        {
            return;
        }

        int count =
            spawnAmountAsSeparateObjects
            ? stack.Amount
            : 1;

        int amountPerObject =
            spawnAmountAsSeparateObjects
            ? 1
            : stack.Amount;

        for (int i = 0;
            i < count;
            i++)
        {
            SpawnSingle(
                stack,
                amountPerObject,
                recipe,
                inputItems
            );
        }
    }

    void SpawnSingle(
        StoredItemStack stack,
        int amount,
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        GameObject prefab =
            stack.ItemData.prefab;

        if (prefab == null)
        {
            Log(
                "Missing prefab: " +
                stack.ItemData.itemName
            );

            return;
        }

        Vector3 position =
            spawnArea != null
            ? spawnArea.GetSpawnPosition()
            : transform.position;

        Quaternion rotation =
            spawnArea != null
            ? spawnArea.GetSpawnRotation()
            : transform.rotation;

        GameObject obj =
            Instantiate(
                prefab,
                position,
                rotation
            );

        PhysicalItem item =
            obj.GetComponent<PhysicalItem>();

        if (item != null)
        {
            item.SetAmount(amount);

            item.SetInstanceData(
                stack.InstanceData
            );
        }

        WeaponInstanceHolder weapon =
            obj.GetComponent
            <WeaponInstanceHolder>();

        if (weapon != null &&
            stack.WeaponInstance != null)
        {
            weapon.SetInstance(
                stack.WeaponInstance.Clone()
            );
        }

        if (postProcessor != null)
        {
            postProcessor.ProcessSpawnedObject(
                obj,
                stack,
                recipe,
                inputItems
            );
        }

        Physics.SyncTransforms();

        Log(
            "Spawned physical object: " +
            stack.ItemData.itemName
        );
    }

    protected void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[" + GetType().Name + "] " +
            message,
            this
        );
    }
}