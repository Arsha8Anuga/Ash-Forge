using System.Collections.Generic;
using UnityEngine;

public class AreaItemScanner :
    MonoBehaviour,
    IWorkstationInputProvider
{
    [SerializeField]
    private BoxCollider scanCollider;

    [SerializeField]
    private bool ignoreHeldItems = true;

    [SerializeField]
    private bool ignoreWorkbenchTools = true;

    [SerializeField]
    private LayerMask detectionMask = ~0;

    [SerializeField]
    private QueryTriggerInteraction triggerInteraction =
        QueryTriggerInteraction.Collide;

    [SerializeField]
    private bool debugLog;

    void Awake()
    {
        if (scanCollider == null)
            scanCollider = GetComponent<BoxCollider>();
    }

    public List<PhysicalItem> GetItems()
    {
        Physics.SyncTransforms();

        List<PhysicalItem> result =
            new List<PhysicalItem>();

        if (scanCollider == null)
            return result;

        Bounds bounds =
            scanCollider.bounds;

        Collider[] hits =
            Physics.OverlapBox(
                bounds.center,
                bounds.extents,
                scanCollider.transform.rotation,
                detectionMask,
                triggerInteraction
            );

        HashSet<PhysicalItem> unique =
            new HashSet<PhysicalItem>();

        foreach (Collider hit in hits)
        {
            PhysicalItem item =
                hit.GetComponentInParent
                <PhysicalItem>();

            if (item == null)
                continue;

            if (!item.IsValid)
                continue;

            if (ShouldIgnore(item))
                continue;

            unique.Add(item);
        }

        result.AddRange(unique);

        if (debugLog)
        {
            Debug.Log(
                "[AreaItemScanner] Detected: " +
                result.Count,
                this
            );
        }

        return result;
    }

    bool ShouldIgnore(
        PhysicalItem item)
    {
        if (item == null)
            return true;

        if (!item.IsValid)
            return true;

        if (ignoreWorkbenchTools)
        {
            WorkstationTool tool =
                item.GetComponent<WorkstationTool>();

            if (tool != null)
                return true;
        }

        if (ignoreHeldItems)
        {
            IGrabbable grabbable =
                item.GetComponent<IGrabbable>();

            if (grabbable != null &&
                grabbable.IsHeld)
            {
                return true;
            }
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (scanCollider == null)
            scanCollider = GetComponent<BoxCollider>();

        if (scanCollider == null)
            return;

        Gizmos.color = Color.cyan;

        Gizmos.matrix =
            scanCollider.transform.localToWorldMatrix;

        Gizmos.DrawWireCube(
            scanCollider.center,
            scanCollider.size
        );
    }
}