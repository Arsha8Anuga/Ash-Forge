using UnityEngine;

public class SnapSocket : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField]
    private bool useChainMode;

    [Header("Host")]
    [SerializeField]
    private SnapHost host;

    [Header("Chain")]
    [SerializeField]
    private SnapChainNode ownerNode;

    [Header("Point")]
    [SerializeField]
    private Transform point;

    [Header("Socket Area")]
    [SerializeField]
    private Collider socketArea;

    [Header("Parenting")]
    [SerializeField]
    private Transform attachParent;

    [Header("Filter")]
    [SerializeField]
    private SnapTypeData[] acceptedTypes;

    [Header("Anchor")]
    [SerializeField]
    private string[] acceptedAnchorTags;

    [SerializeField]
    private float surfaceOffset;

    [Header("Stack")]
    [SerializeField]
    private int maxStack = 1;

    [Header("Snap Conditions")]
    [SerializeField]
    private float maxSnapVelocity = 1f;

    [SerializeField]
    private float maxAnchorSnapDistance = 0.25f;

    [SerializeField]
    private float snapDwellTime = 0.08f;

    [SerializeField]
    private float snapCooldown = 0.1f;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    private SnappableObject current;
    private PhysicalItem currentItem;
    private int currentStack;

    private SnapSocketTriggerHandler triggerHandler;
    private SnapSocketValidator validator;
    private SnapChainMode chainMode;
    private SnapStackMode stackMode;
    private SnapSocketGizmos gizmos;

    public Transform Point => point;

    public Collider SocketArea =>
        socketArea;

    public Transform AttachParent =>
        attachParent != null
        ? attachParent
        : transform;

    public string[] AcceptedAnchorTags =>
        acceptedAnchorTags;

    public float SurfaceOffset =>
        surfaceOffset;

    public bool IsOccupied =>
        current != null;

    public int CurrentStack =>
        currentStack;

    public int MaxStack =>
        maxStack;

    public bool UseChainMode =>
        useChainMode;

    public SnapHost Host =>
        host;

    public SnapChainNode OwnerNode =>
        ownerNode;

    public SnapTypeData[] AcceptedTypes =>
        acceptedTypes;

    public float MaxSnapVelocity =>
        maxSnapVelocity;

    public float MaxAnchorSnapDistance =>
        maxAnchorSnapDistance;

    public float MaxAnchorDistance =>
        maxAnchorSnapDistance;

    public float SnapDwellTime =>
        snapDwellTime;

    public float SnapCooldown =>
        snapCooldown;

    public SnappableObject Current =>
        current;

    public PhysicalItem CurrentItem =>
        currentItem;

    void Awake()
    {
        if (point == null)
        {
            Debug.LogError(
                "[SnapSocket] Point is missing on " +
                name +
                ". Drag SnapPoint into Point field.",
                this
            );
        }

        if (socketArea == null)
            socketArea = GetComponent<Collider>();

        if (socketArea != null)
            socketArea.isTrigger = true;

        if (host == null)
            host = GetComponentInParent<SnapHost>();

        if (ownerNode == null)
            ownerNode = GetComponentInParent<SnapChainNode>();

        validator =
            new SnapSocketValidator(this);

        triggerHandler =
            new SnapSocketTriggerHandler(
                this,
                validator
            );

        chainMode =
            new SnapChainMode(
                this,
                validator
            );

        stackMode =
            new SnapStackMode(
                this,
                validator
            );

        gizmos =
            new SnapSocketGizmos(this);
    }

    void OnTriggerEnter(Collider other)
    {
        triggerHandler.OnEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        triggerHandler.OnStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        triggerHandler.OnExit(other);
    }

    public bool TrySnap(
        SnappableObject snap)
    {
        if (snap == null)
            return false;

        if (point == null)
        {
            Log("TrySnap failed: point missing");
            return false;
        }

        if (useChainMode)
            return chainMode.TrySnap(snap);

        return stackMode.TrySnap(snap);
    }

    public bool TryUnsnapCurrent()
    {
        if (current == null)
        {
            Log("TryUnsnapCurrent failed: current null");
            return false;
        }

        SnappableObject snap =
            current;

        snap.Unsnap();

        bool success =
            snap == null ||
            !snap.IsSnapped;

        if (!success)
        {
            Log("TryUnsnapCurrent failed: object still snapped");
        }

        return success;
    }

    public bool CanAccept(
        SnappableObject snap)
    {
        return validator.CanAccept(snap);
    }

    public void SetCurrent(
        SnappableObject snap)
    {
        current = snap;
    }

    public void SetCurrentItem(
        PhysicalItem item)
    {
        currentItem = item;
    }

    public void SetCurrentStack(
        int value)
    {
        currentStack =
            Mathf.Clamp(
                value,
                0,
                maxStack
            );

        SyncCurrentItemAmount();
    }

    public void IncrementStack()
    {
        TryRegisterStack(1);
    }

    public bool CanRegisterStack(
        int amount)
    {
        if (useChainMode)
            return false;

        amount =
            Mathf.Max(
                1,
                amount
            );

        if (currentStack + amount >
            maxStack)
        {
            return false;
        }

        if (host != null &&
            !host.CanAdd(amount))
        {
            return false;
        }

        return true;
    }

   public bool TryRegisterStack()
    {
        return TryRegisterStack(1);
    }

    public bool TryRegisterStack(
        int amount)
    {
        if (!CanRegisterStack(amount))
            return false;

        amount =
            Mathf.Max(
                1,
                amount
            );

        if (host != null &&
            !host.TryAdd(amount))
        {
            return false;
        }

        currentStack += amount;

        SyncCurrentItemAmount();

        return true;
    }

    void SyncCurrentItemAmount()
{
    if (currentItem == null)
        return;

    if (currentStack <= 0)
        return;

    currentItem.SetAmount(
        currentStack
    );
}

    public SnapHost ResolveRootHost()
    {
        if (ownerNode != null &&
            ownerNode.RootHost != null)
        {
            return ownerNode.RootHost.Root;
        }

        if (host != null)
            return host.Root;

        return null;
    }

    public Collider[] GetOwnerColliders()
    {
        if (ownerNode == null)
        {
            return GetComponentsInParent<Collider>();
        }

        return ownerNode
            .GetComponentsInChildren<Collider>();
    }

    public void Clear(
        SnappableObject snap)
    {
        if (current != snap)
            return;

        current = null;
        currentItem = null;

        triggerHandler.Cleanup(snap);

        if (useChainMode)
        {
            currentStack = 0;
            return;
        }

        if (currentStack <= 0)
            return;

        int removed =
            currentStack;

        currentStack = 0;

        if (host != null)
            host.Remove(removed);
    }

    public void RemoveOne()
    {
        RemoveStackAmount(1);
    }

    public void RemoveStackAmount(
        int amount)
    {
        if (useChainMode)
            return;

        if (amount <= 0)
            return;

        if (currentStack <= 0)
            return;

        int removed =
            Mathf.Min(
                amount,
                currentStack
            );

        currentStack -= removed;

        if (host != null)
        {
            host.Remove(
                removed
            );
        }

        SyncCurrentItemAmount();

        if (currentStack <= 0 &&
            current != null)
        {
            current.Unsnap();
        }
    }

    public void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[SnapSocket] " +
            name +
            " | " +
            message,
            this
        );
    }

    void OnDrawGizmos()
    {
        if (gizmos == null)
            gizmos =
                new SnapSocketGizmos(this);

        gizmos.Draw();
    }
}