using UnityEngine;

public class HeavyObjectInteractable :
    InteractableObject,
    IGrabbable
{
    [Header("Grab")]
    [SerializeField]
    private GrabMode supportedModes =
        GrabMode.Physics;

    [SerializeField]
    private HeavyObjectPhysics physics;

    public XRHandInteractor LeftHand
    {
        get;
        private set;
    }

    public XRHandInteractor RightHand
    {
        get;
        private set;
    }

    public bool IsHeld =>
        LeftHand != null ||
        RightHand != null;

    public bool IsTwoHanded =>
        LeftHand != null &&
        RightHand != null;

    protected override void Awake()
    {
        base.Awake();

        if (physics == null)
        {
            physics =
                GetComponent<HeavyObjectPhysics>();
        }
    }

    public bool Grab(
        XRHandInteractor hand,
        GrabMode mode)
    {
        if (!supportedModes.HasFlag(mode))
            return false;

        if (LeftHand == hand ||
            RightHand == hand)
            return false;

        if (LeftHand == null)
        {
            LeftHand = hand;
            BeginHold();
            return true;
        }

        if (RightHand == null)
        {
            RightHand = hand;
            BeginHold();
            return true;
        }

        return false;
    }

    public void Release(
        XRHandInteractor hand)
    {
        if (LeftHand == hand)
            LeftHand = null;

        if (RightHand == hand)
            RightHand = null;

        if (!IsHeld)
            EndHold();
    }

    void BeginHold()
    {
        if (rb == null)
            return;

        rb.useGravity = false;

        rb.drag = 6f;

        rb.WakeUp();
    }

    void EndHold()
    {
        if (rb == null)
            return;

        rb.useGravity = true;

        rb.drag = 0f;
    }

    public GrabMode GetSupportedGrabMode()
    {
        return supportedModes;
    }

    public Transform GetGrabPoint()
    {
        return transform;
    }

    void FixedUpdate()
    {
        if (!IsHeld)
            return;

        if (physics == null)
            return;

        physics.Tick(
            LeftHand,
            RightHand
        );
    }
}