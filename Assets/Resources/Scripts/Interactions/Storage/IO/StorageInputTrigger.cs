using System.Collections.Generic;
using UnityEngine;

public class StorageInputTrigger : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour storageBehaviour;

    [SerializeField]
    private float inputDwellTime = 0.1f;

    [SerializeField]
    private bool debugLog;

    private IItemStorage storage;

    private readonly Dictionary<PhysicalItem, float>
        candidates =
        new Dictionary<PhysicalItem, float>();

    private readonly HashSet<PhysicalItem>
        processing =
        new HashSet<PhysicalItem>();

    void Awake()
    {
        storage =
            storageBehaviour as IItemStorage;

        if (storage == null)
        {
            storage =
                GetComponentInParent<IItemStorage>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        RegisterCandidate(other);
    }

    void OnTriggerStay(Collider other)
    {
        RegisterCandidate(other);
        TryProcess(other);
    }

    void OnTriggerExit(Collider other)
    {
        PhysicalItem item =
            FindPhysicalItem(other);

        if (item == null)
            return;

        candidates.Remove(item);
        processing.Remove(item);
    }

    void RegisterCandidate(Collider other)
    {
        PhysicalItem item =
            FindPhysicalItem(other);

        if (item == null)
            return;

        if (candidates.ContainsKey(item))
            return;

        candidates.Add(
            item,
            Time.time
        );
    }

    void TryProcess(Collider other)
    {
        if (storage == null)
            return;

        PhysicalItem rootItem =
            FindPhysicalItem(other);

        if (rootItem == null)
            return;

        if (processing.Contains(rootItem))
            return;

        if (IsHeld(rootItem))
            return;

        if (!candidates.TryGetValue(
            rootItem,
            out float enterTime))
        {
            return;
        }

        if (Time.time - enterTime <
            inputDwellTime)
        {
            return;
        }

        List<PhysicalItem> items =
            CollectInputItems(rootItem);

        if (items.Count <= 0)
            return;

        foreach (PhysicalItem item in items)
        {
            if (item == null)
                continue;

            if (processing.Contains(item))
                continue;

            if (!storage.CanInput(item))
                continue;

            processing.Add(item);
        }

        foreach (PhysicalItem item in items)
        {
            if (item == null)
                continue;

            if (!processing.Contains(item))
                continue;

            bool success =
                storage.Input(item);

            if (debugLog && success)
            {
                Debug.Log(
                    "[StorageInput] Stored: " +
                    item.ItemName +
                    " x" +
                    item.Amount,
                    this
                );
            }
        }
    }

    PhysicalItem FindPhysicalItem(Collider other)
    {
        return other.GetComponentInParent
            <PhysicalItem>();
    }

    bool IsHeld(PhysicalItem item)
    {
        IGrabbable grabbable =
            item.GetComponent<IGrabbable>();

        return grabbable != null &&
            grabbable.IsHeld;
    }

    List<PhysicalItem> CollectInputItems(
        PhysicalItem rootItem)
    {
        List<PhysicalItem> result =
            new List<PhysicalItem>();

        SnapChainNode chainNode =
            rootItem.GetComponent<SnapChainNode>();

        if (chainNode != null)
        {
            PhysicalItem[] childItems =
                chainNode.GetComponentsInChildren
                <PhysicalItem>();

            foreach (PhysicalItem item in childItems)
            {
                if (item == null)
                    continue;

                if (!result.Contains(item))
                    result.Add(item);
            }

            return result;
        }

        result.Add(rootItem);
        return result;
    }
}