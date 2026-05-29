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

    [Header("Snap")]
    [SerializeField]
    private SnapTypeData type;

    [SerializeField]
    private bool useCompoundSnap = true;

    private SnapState state;
    private SnapCooldown cooldown;
    private SnapAnchorResolver anchorResolver;
    private SnapTransformHandler transformHandler;
    private SnapPhysicsHandler physicsHandler;
    private SnapCompoundHandler compoundHandler;
    private SnapPostProcessor postProcessor;

    public SnapTypeData Type =>
        type;

    public bool IsSnapped =>
        state.IsSnapped;

    public bool IsSnapping =>
        state.IsSnapping;

    public SnapSocket CurrentSocket =>
        state.CurrentSocket;

    public bool CanAttemptSnap =>
        cooldown.CanSnap;

    void Awake()
    {
        state =
            new SnapState();

        cooldown =
            new SnapCooldown();

        anchorResolver =
            new SnapAnchorResolver(
                transform
            );

        transformHandler =
            new SnapTransformHandler(
                transform
            );

        physicsHandler =
            new SnapPhysicsHandler(
                gameObject
            );

        compoundHandler =
            new SnapCompoundHandler(
                gameObject
            );

        postProcessor =
            new SnapPostProcessor(
                gameObject,
                defaultLayer,
                snappedLayer
            );
    }

    public void BlockSnapFor(
        float duration)
    {
        cooldown.BlockFor(
            duration
        );
    }

    public SnapAnchor GetBestAnchor(
        string[] acceptedTags,
        Vector3 socketPosition)
    {
        return anchorResolver.GetBestAnchor(
            acceptedTags,
            socketPosition
        );
    }

    public bool CanSnap(
        SnapSocket socket)
    {
        if (socket == null)
            return false;

        if (IsSnapped ||
            IsSnapping ||
            !CanAttemptSnap)
        {
            return false;
        }

        return socket.CanAccept(this);
    }

    public void Snap(
        SnapSocket socket)
    {
        if (socket == null)
            return;

        SnapAnchor anchor =
            GetBestAnchor(
                socket.AcceptedAnchorTags,
                socket.Point.position
            );

        Snap(
            socket,
            anchor
        );
    }

    public void Snap(
        SnapSocket socket,
        SnapAnchor anchor)
    {
        if (!CanSnap(socket))
            return;

        if (anchor == null)
            return;

        state.BeginSnap(
            socket,
            anchor
        );

        if (useCompoundSnap)
        {
            compoundHandler.RemoveRigidbody();
        }
        else
        {
            physicsHandler.StopPhysics();
        }

        transformHandler.AttachAndAlign(
            socket,
            anchor
        );

        state.CompleteSnap();

        postProcessor.OnSnapped();
    }

    public void Unsnap()
    {
        if (!IsSnapped)
            return;

        SnapChainNode chain =
            GetComponent<SnapChainNode>();

        if (chain != null)
        {
            chain.DetachAsNewRoot();
        }

        SnapSocket socket =
            state.CurrentSocket;

        state.BeginUnsnap();

        if (socket != null)
        {
            socket.Clear(this);
        }

        transformHandler.Detach();

        if (useCompoundSnap)
        {
            compoundHandler.RestoreRigidbody();
        }
        else
        {
            physicsHandler.ResumePhysics();
        }

        postProcessor.OnUnsnapped();

        state.Clear();

        cooldown.BlockFor(
            0.15f
        );
    }

    public void ForceUnsnapWithoutSocketClear()
    {
        if (!IsSnapped &&
            !IsSnapping)
        {
            return;
        }

        state.BeginUnsnap();

        transformHandler.Detach();

        if (useCompoundSnap)
        {
            compoundHandler.RestoreRigidbody();
        }
        else
        {
            physicsHandler.ResumePhysics();
        }

        postProcessor.OnUnsnapped();

        state.Clear();

        cooldown.BlockFor(
            0.15f
        );
    }
}