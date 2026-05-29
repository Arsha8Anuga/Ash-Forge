using System.Collections.Generic;
using UnityEngine;

public class WorkstationOutputSpawner :
    MonoBehaviour
{
    [Header("Fallback Point")]
    [SerializeField]
    private Transform outputPoint;

    [Header("Optional Area")]
    [SerializeField]
    private BoxCollider outputArea;

    [Header("Spawn Layout")]
    [SerializeField]
    private float spacing = 0.25f;

    [SerializeField]
    private float spawnHeightOffset = 0.05f;

    [SerializeField]
    private int columns = 3;

    [Header("Physics")]
    [SerializeField]
    private bool resetRigidbody = true;

    [SerializeField]
    private bool forcePhysicalAmountToOne = true;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    void Awake()
    {
        if (outputPoint == null)
            outputPoint = transform;

        if (outputArea == null)
        {
            outputArea =
                GetComponent<BoxCollider>();
        }
    }

    public void Spawn(
        WorkstationRecipeData recipe)
    {
        Spawn(
            recipe,
            null
        );
    }

    public void Spawn(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        if (recipe == null ||
            recipe.outputs == null ||
            recipe.outputs.Length == 0)
        {
            Log("No output found.");
            return;
        }

        int spawnIndex = 0;

        foreach (WorkstationOutput output
            in recipe.outputs)
        {
            if (output == null)
                continue;

            int count =
                Mathf.Max(
                    1,
                    output.amount
                );

            for (int i = 0;
                 i < count;
                 i++)
            {
                SpawnSingle(
                    output,
                    recipe,
                    inputItems,
                    spawnIndex
                );

                spawnIndex++;
            }
        }

        Physics.SyncTransforms();
    }

    GameObject SpawnSingle(
        WorkstationOutput output,
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems,
        int index)
    {
        if (output.itemData == null)
        {
            Log("Output item data is null.");
            return null;
        }

        if (output.itemData.prefab == null)
        {
            Log(
                "Output prefab missing: " +
                output.itemData.itemName
            );

            return null;
        }

        Vector3 position =
            GetSpawnPosition(index);

        Quaternion rotation =
            GetSpawnRotation();

        GameObject obj =
            Instantiate(
                output.itemData.prefab,
                position,
                rotation
            );

        SetupPhysicalItem(
            obj,
            output,
            recipe,
            inputItems
        );

        SetupRigidbody(obj);

        Log(
            "Spawned physical object: " +
            output.itemData.itemName
        );

        return obj;
    }

    void SetupPhysicalItem(
        GameObject obj,
        WorkstationOutput output,
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems)
    {
        PhysicalItem item =
            obj.GetComponent<PhysicalItem>();

        if (item == null)
        {
            item =
                obj.GetComponentInChildren
                <PhysicalItem>();
        }

        if (item == null)
        {
            Log("Spawned object has no PhysicalItem.");
            return;
        }

        int physicalAmount =
            forcePhysicalAmountToOne
            ? 1
            : Mathf.Max(
                1,
                output.amount
            );

        ItemInstanceData instance =
            ItemQualityCalculator
            .CreateOutputInstance(
                inputItems,
                recipe != null
                ? recipe.baseQuality
                : 80f,
                recipe != null
                ? recipe.baseDefect
                : 0f
            );

        item.Initialize(
            output.itemData,
            physicalAmount,
            instance
        );
    }

    void SetupRigidbody(
        GameObject obj)
    {
        if (!resetRigidbody)
            return;

        Rigidbody rb =
            obj.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb =
                obj.GetComponentInChildren
                <Rigidbody>();
        }

        if (rb == null)
            return;

        rb.velocity =
            Vector3.zero;

        rb.angularVelocity =
            Vector3.zero;

        rb.WakeUp();
    }

    Vector3 GetSpawnPosition(
        int index)
    {
        if (HasValidArea())
        {
            return GetAreaPosition(index);
        }

        return GetPointPosition(index);
    }

    Quaternion GetSpawnRotation()
    {
        if (outputPoint != null)
            return outputPoint.rotation;

        return transform.rotation;
    }

    bool HasValidArea()
    {
        if (outputArea == null)
            return false;

        if (!outputArea.enabled)
            return false;

        Vector3 size =
            outputArea.size;

        return size.x > 0f &&
            size.y > 0f &&
            size.z > 0f;
    }

    Vector3 GetPointPosition(
        int index)
    {
        Transform point =
            outputPoint != null
            ? outputPoint
            : transform;

        Vector3 offset =
            point.right *
            spacing *
            index;

        return point.position + offset;
    }

    Vector3 GetAreaPosition(
        int index)
    {
        int safeColumns =
            Mathf.Max(
                1,
                columns
            );

        int xIndex =
            index % safeColumns;

        int zIndex =
            index / safeColumns;

        Vector3 localOffset =
            new Vector3(
                GetCenteredOffset(
                    xIndex,
                    safeColumns
                ),
                0f,
                zIndex * spacing
            );

        Vector3 localCenter =
            outputArea.center;

        Vector3 halfSize =
            outputArea.size * 0.5f;

        localOffset.x =
            Mathf.Clamp(
                localOffset.x,
                -halfSize.x + spacing * 0.5f,
                halfSize.x - spacing * 0.5f
            );

        localOffset.z =
            Mathf.Clamp(
                localOffset.z,
                -halfSize.z + spacing * 0.5f,
                halfSize.z - spacing * 0.5f
            );

        localOffset.y =
            halfSize.y + spawnHeightOffset;

        Vector3 localPosition =
            localCenter + localOffset;

        return outputArea.transform
            .TransformPoint(localPosition);
    }

    float GetCenteredOffset(
        int index,
        int count)
    {
        float center =
            (count - 1) * 0.5f;

        return
            (index - center) *
            spacing;
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[WorkstationOutputSpawner] " +
            message,
            this
        );
    }
}