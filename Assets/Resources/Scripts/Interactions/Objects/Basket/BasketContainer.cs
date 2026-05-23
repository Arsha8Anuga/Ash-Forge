using System.Collections.Generic;
using UnityEngine;

public class BasketContainer : MonoBehaviour
{
    [SerializeField]
    private BasketMassTracker massTracker;

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

        return true;
    }

    void Register(
        PhysicalItem item)
    {
        if (!items.Add(item))
            return;

        Rigidbody rb =
            item.GetComponent<Rigidbody>();

        if (rb != null &&
            massTracker != null)
        {
            massTracker.AddBody(rb);
        }
    }

    void Unregister(
        PhysicalItem item)
    {
        if (!items.Remove(item))
            return;

        Rigidbody rb =
            item.GetComponent<Rigidbody>();

        if (rb != null &&
            massTracker != null)
        {
            massTracker.RemoveBody(rb);
        }
    }
}