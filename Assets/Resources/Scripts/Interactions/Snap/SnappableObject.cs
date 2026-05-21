using UnityEngine;

public class SnappableObject :
    MonoBehaviour,
    ISnappable
{
    [Header("Layers")]
    [SerializeField]
    private string defaultLayer =
        "Interactable";

    [SerializeField]
    private string snappedLayer =
        "SnappedObject";

    [Header("Anchor")]
    [SerializeField]
    private Transform anchor;

    [SerializeField]
    private SnapTypeData type;

    private Rigidbody rb;

    public SnapTypeData Type =>
        type;

    public bool IsSnapped
    {
        get;
        private set;
    }

    public SnapSocket CurrentSocket
    {
        get;
        private set;
    }

    void Awake()
    {
        rb =
            GetComponent<Rigidbody>();

        ResolveAnchor();
    }

    void ResolveAnchor()
    {
        if (anchor != null)
            return;

        SnapAnchor point =
            GetComponentInChildren<SnapAnchor>();

        anchor =
            point != null
            ? point.transform
            : transform;
    }

    public bool CanSnap(
        SnapSocket socket)
    {
        if (IsSnapped)
            return false;

        if (socket == null)
            return false;

        return socket.CanAccept(this);
    }

    public void Snap(
        SnapSocket socket)
    {
        if (!CanSnap(socket))
            return;

        IsSnapped = true;

        CurrentSocket = socket;

        StopPhysics();

        Vector3 worldScale =
            transform.lossyScale;

        transform.SetParent(
            socket.Point,
            true
        );

        SnapTransformUtility.KeepWorldScale(
            transform,
            worldScale
        );

        ApplyLocalSnap();

        SnapLayerUtility.SetLayerRecursive(
            gameObject,
            snappedLayer
        );

        RecalculateChain();
    }

    public void Unsnap()
    {
        if (!IsSnapped)
            return;

        IsSnapped = false;

        if (CurrentSocket != null)
        {
            CurrentSocket.Clear(this);
        }

        CurrentSocket = null;

        Vector3 worldScale =
            transform.lossyScale;

        transform.SetParent(
            null,
            true
        );

        SnapTransformUtility.KeepWorldScale(
            transform,
            worldScale
        );

        ResumePhysics();

        SnapLayerUtility.SetLayerRecursive(
            gameObject,
            defaultLayer
        );

        RecalculateChain();
    }

    void StopPhysics()
    {
        if (rb == null)
            return;

        rb.velocity =
            Vector3.zero;

        rb.angularVelocity =
            Vector3.zero;

        rb.useGravity =
            false;

        rb.isKinematic =
            true;
    }

    void ResumePhysics()
    {
        if (rb == null)
            return;

        rb.isKinematic =
            false;

        rb.useGravity =
            true;
    }

    void ApplyLocalSnap()
    {
        if (anchor == null)
            return;

        transform.localRotation =
            Quaternion.Inverse(
                anchor.localRotation
            );

        transform.localPosition =
            -anchor.localPosition;
    }

    void RecalculateChain()
    {
        WeightChain chain =
            GetComponentInParent<WeightChain>();

        if (chain != null)
        {
            chain.Recalculate();
        }
    }
}