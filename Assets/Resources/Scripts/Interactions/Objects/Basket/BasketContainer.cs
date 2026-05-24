using System.Collections.Generic;
using UnityEngine;

public class BasketContainer : MonoBehaviour
{
    [SerializeField]
    private BasketMassTracker massTracker;

    [SerializeField]
    private bool debugLog;

    [SerializeField]
    private Transform basketRoot;

    private readonly HashSet<PhysicalItem> items =
        new HashSet<PhysicalItem>();

    public int ItemCount =>
        items.Count;

    void Awake()
    {
        if (massTracker == null)
        {
            massTracker =
                GetComponentInParent
                <BasketMassTracker>();
        }

        if (basketRoot == null)
        {
            basketRoot =
                massTracker != null
                ? massTracker.transform
                : transform.root;
        }
    }

    void OnTriggerEnter(
        Collider other)
    {
        PhysicalItem item =
            other.GetComponentInParent
            <PhysicalItem>();

        if (!CanAccept(item))
            return;

        Register(item);
    }

    void OnTriggerExit(
        Collider other)
    {
        PhysicalItem item =
            other.GetComponentInParent
            <PhysicalItem>();

        if (item == null)
            return;

        Unregister(item);
    }

    bool CanAccept(
        PhysicalItem item)
    {
        if (item == null)
            return false;

        if (!item.IsValid)
            return false;

        if (!item.CanBasket)
            return false;

        if (basketRoot != null &&
            (
                item.transform == basketRoot ||
                item.transform.IsChildOf(basketRoot)
            ))
        {
            return false;
        }

        return true;
    }

    void Register(
        PhysicalItem item)
    {
        if (!items.Add(item))
            return;

        if (massTracker != null)
        {
            massTracker.AddItem(item);
        }

        if (debugLog)
        {
            Debug.Log(
                "[BasketContainer] Added: " +
                item.ItemName,
                this
            );
        }
    }

    void Unregister(
        PhysicalItem item)
    {
        if (!items.Remove(item))
            return;

        if (massTracker != null)
        {
            massTracker.RemoveItem(item);
        }

        if (debugLog)
        {
            Debug.Log(
                "[BasketContainer] Removed: " +
                item.ItemName,
                this
            );
        }
    }
}