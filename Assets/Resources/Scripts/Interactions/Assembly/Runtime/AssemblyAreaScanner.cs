using System.Collections.Generic;
using UnityEngine;

public class AssemblyAreaScanner :
    MonoBehaviour
{
    [SerializeField]
    private BoxCollider scanCollider;

    [SerializeField]
    private LayerMask detectionMask =
        ~0;

    [SerializeField]
    private QueryTriggerInteraction
        triggerInteraction =
        QueryTriggerInteraction.Collide;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    [SerializeField]
    private bool debugDraw = true;

    private readonly List<PhysicalItem> items =
        new List<PhysicalItem>();

    void Awake()
    {
        if (scanCollider == null)
        {
            scanCollider =
                GetComponent<BoxCollider>();
        }
    }

    public List<PhysicalItem> GetItems()
    {
        Physics.SyncTransforms();

        Refresh();

        return new List<PhysicalItem>(
            items
        );
    }

    void Refresh()
    {
        items.Clear();

        if (scanCollider == null)
            return;

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

        if (debugLog)
        {
            Debug.Log(
                "[AssemblyScanner] Hits: " +
                hits.Length,
                this
            );
        }

        HashSet<PhysicalItem> unique =
            new HashSet<PhysicalItem>();

        foreach (Collider hit in hits)
        {
            if (hit == null)
                continue;

            PhysicalItem item =
                FindClosestPhysicalItem(hit);

            if (debugLog)
            {
                Debug.Log(
                    "[AssemblyScanner] Hit Collider: " +
                    hit.name +
                    " | Layer: " +
                    LayerMask.LayerToName(
                        hit.gameObject.layer
                    ) +
                    " | PhysicalItem: " +
                    (
                        item != null
                        ? item.ItemName
                        : "NULL"
                    ),
                    hit
                );
            }

            if (item == null)
                continue;

            if (!IsValidDetectedItem(item))
                continue;

            unique.Add(item);
        }

        foreach (PhysicalItem item
            in unique)
        {
            if (item != null)
            {
                items.Add(item);
            }
        }

        if (debugLog)
        {
            Debug.Log(
                "[AssemblyScanner] Items: " +
                items.Count,
                this
            );
        }
    }

    PhysicalItem FindClosestPhysicalItem(
        Collider hit)
    {
        if (hit == null)
            return null;

        PhysicalItem direct =
            hit.GetComponent<PhysicalItem>();

        if (direct != null)
            return direct;

        Transform current =
            hit.transform;

        while (current != null)
        {
            PhysicalItem item =
                current.GetComponent<PhysicalItem>();

            if (item != null)
                return item;

            current =
                current.parent;
        }

        return null;
    }

    bool IsValidDetectedItem(
        PhysicalItem item)
    {
        if (item == null)
            return false;

        if (!item.gameObject.activeInHierarchy)
            return false;

        if (!item.IsValid)
            return false;

        return true;
    }

    void OnDrawGizmosSelected()
    {
        if (!debugDraw)
            return;

        if (scanCollider == null)
            return;

        Gizmos.color =
            Color.cyan;

        Gizmos.matrix =
            scanCollider.transform.localToWorldMatrix;

        Gizmos.DrawWireCube(
            scanCollider.center,
            scanCollider.size
        );
    }
}