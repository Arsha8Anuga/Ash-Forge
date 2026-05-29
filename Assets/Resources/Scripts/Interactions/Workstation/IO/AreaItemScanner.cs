using System.Collections.Generic;
using UnityEngine;

public class AreaItemScanner :
    MonoBehaviour,
    IWorkstationInputProvider
{
    [SerializeField]
    private BoxCollider scanCollider;

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