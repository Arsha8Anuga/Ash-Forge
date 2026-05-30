using UnityEngine;

public class SmallObjectInteractable :
    InteractableObject,
    IGrabbable
{
    [Header("Movement")]
    public float followForce = 60f;
    public float damping = 12f;
    public float deadZoneRadius = 0.15f;
    public float maxVelocity = 20f;

    [Header("Rotation")]
    public float rotationSpeed = 15f;

    [Header("Weight")]
    public float weightMultiplier = 6f;

    [Header("Grab")]
    [SerializeField]
    protected GrabMode supportedModes =
        GrabMode.Physics;

    [SerializeField]
    protected Transform grabPoint;

    protected XRHandInteractor currentHand;
    protected GrabMode currentMode;

    protected SmallObjectPhysics physics;
    protected SmallObjectAttach attach;

    public XRHandInteractor CurrentHand =>
        currentHand;

    public GrabMode CurrentMode =>
        currentMode;

    public Transform InternalGrabPoint =>
        grabPoint;

    public bool IsHeld =>
        currentHand != null;

    protected override void Awake()
    {
        base.Awake();

        RebuildBehaviours();
    }

    public override void RefreshRigidbody()
    {
        base.RefreshRigidbody();

        RebuildBehaviours();
    }

    void RebuildBehaviours()
    {
        physics =
            new SmallObjectPhysics(
                this,
                rb
            );

        attach =
            new SmallObjectAttach(
                this,
                rb
            );
    }

    public virtual bool Grab(
        XRHandInteractor hand,
        GrabMode mode)
    {
        if (IsHeld)
            return false;

        if (!supportedModes.HasFlag(mode))
            return false;

        ISnappable snap =
            GetComponent<ISnappable>();

        if (snap != null &&
            snap.IsSnapped)
        {
            snap.Unsnap();
        }

        RefreshRigidbody();

        if (rb == null)
            return false;

        currentHand = hand;
        currentMode = mode;

        WorkstationTool tool =
            GetComponent<WorkstationTool>();

        if (tool != null)
        {
            tool.SetHeldBy(hand);
        }

        ResetPhysics();

        rb.useGravity = false;
        rb.drag = 0f;
        rb.WakeUp();

        return true;
    }

    public virtual void Release(
        XRHandInteractor hand)
    {
        if (currentHand != hand)
            return;

        WorkstationTool tool =
            GetComponent<WorkstationTool>();

        if (tool != null)
        {
            tool.ClearHeldBy(hand);
        }

        currentHand = null;

        currentMode = GrabMode.None;

        RefreshRigidbody();

        if (rb == null)
            return;

        rb.useGravity = true;

        rb.drag = 0f;

        rb.WakeUp();
    }

    public GrabMode GetSupportedGrabMode()
    {
        return supportedModes;
    }

    public Transform GetGrabPoint()
    {
        if (grabPoint != null)
            return grabPoint;

        return transform;
    }

    void FixedUpdate()
    {
        if (!IsHeld)
            return;

        RefreshRigidbody();

        if (rb == null)
            return;

        switch (currentMode)
        {
            case GrabMode.Physics:
                physics.Tick();
                break;

            case GrabMode.Attach:
                attach.Tick();
                break;
        }
    }
}