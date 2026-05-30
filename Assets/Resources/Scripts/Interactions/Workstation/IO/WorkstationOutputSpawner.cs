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

        return BuildDefaultOutputs(recipe);
    }

    List<StoredItemStack> BuildDefaultOutputs(
        WorkstationRecipeData recipe)
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

            StoredItemStack stack =
                new StoredItemStack(
                    output.itemData,
                    output.amount,
                    output.itemData.CreateInstance()
                );

            result.Add(stack);
        }

        return result;
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
        if (routes == null ||
            routes.Length == 0)
        {
            return null;
        }

        WorkstationOutputRoute fallback = null;

        foreach (WorkstationOutputRoute route
            in routes)
        {
            if (route == null)
                continue;

            if (route.itemData == null)
            {
                if (fallback == null)
                    fallback = route;

                continue;
            }

            if (route.Matches(itemData))
                return route;
        }

        return fallback;
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