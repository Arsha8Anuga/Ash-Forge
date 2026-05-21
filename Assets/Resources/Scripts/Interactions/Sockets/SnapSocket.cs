using UnityEngine;

public class SnapSocket : MonoBehaviour
{
    [Header("Host")]
    [SerializeField]
    private SnapHost host;

    [Header("Point")]
    [SerializeField]
    private Transform point;

    [Header("Filter")]
    [SerializeField]
    private SnapTypeData[] acceptedTypes;

    [Header("Stack")]
    [SerializeField]
    private int maxStack = 1;

    private SnappableObject current;
    private int currentStack;

    public Transform Point => point;

    public bool IsOccupied =>
        current != null;

    public int CurrentStack =>
        currentStack;

    public int MaxStack =>
        maxStack;

    void Awake()
    {
        if (point == null)
            point = transform;

        if (host == null)
            host = GetComponentInParent<SnapHost>();
    }

    void OnTriggerEnter(
        Collider other)
    {
        SnappableObject obj =
            other.GetComponentInParent
            <SnappableObject>();

        if (obj == null)
            return;

        IGrabbable grab =
            obj.GetComponent<IGrabbable>();

        if (grab != null &&
            grab.IsHeld)
            return;

        TrySnap(obj);
    }

    public bool TrySnap(
        SnappableObject obj)
    {
        if (!CanAccept(obj))
            return false;

        if (!obj.CanSnap(this))
            return false;

        if (!TryRegisterItem(obj))
            return false;

        if (current == null)
        {
            current = obj;

            obj.Snap(this);
        }
        else
        {
            Destroy(obj.gameObject);
        }

        return true;
    }

    bool TryRegisterItem(
        SnappableObject obj)
    {
        if (currentStack >= maxStack)
            return false;

        if (host != null &&
            !host.TryAdd(1))
            return false;

        currentStack++;
        return true;
    }

    public bool CanAccept(
    SnappableObject obj)
    {
        if (obj == null)
            return false;

        if (current != null &&
            current.Type != obj.Type)
        {
            return false;
        }

        if (currentStack >= maxStack)
            return false;

        if (host != null &&
            !host.CanAdd(1))
        {
            return false;
        }

        bool hasValidFilter = false;

        foreach (SnapTypeData type
            in acceptedTypes)
        {
            if (type == null)
                continue;

            hasValidFilter = true;

            if (obj.Type == null)
                return false;

            if (type == obj.Type)
                return true;
        }

        if (!hasValidFilter)
            return true;

        return false;
    }

    public void Clear(
    SnappableObject snap)
    {
        if (current != snap)
            return;

        current = null;

        if (currentStack > 0)
        {
            int removed =
                currentStack;

            currentStack = 0;

            if (host != null)
                host.Remove(removed);
        }
    }

    public void RemoveOne()
    {
        if (currentStack <= 0)
            return;

        currentStack--;

        if (host != null)
            host.Remove(1);

        if (currentStack <= 0 &&
            current != null)
        {
            current.Unsnap();
        }
    }
}