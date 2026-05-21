using UnityEngine;

public class XRHandHolding
{
    private XRHandInteractor hand;

    public IGrabbable HeldObject { get; private set; }

    public GrabMode CurrentMode { get; private set; }

    public XRHandHolding(
        XRHandInteractor hand)
    {
        this.hand = hand;
    }

    public void Tick()
    {
        HandleInput();

        ValidateDistance();
    }

    void HandleInput()
    {
        HandlePhysicsGrab();

        HandleAttachGrab();
    }

    void HandlePhysicsGrab()
    {
        bool physicsHeld =
            hand.Input.GripHeld &&
            !hand.Input.TriggerHeld;

        if (physicsHeld)
        {
            if (HeldObject == null)
            {
                TryGrab(
                    GrabMode.Physics
                );
            }

            return;
        }

        if (HeldObject != null &&
            CurrentMode == GrabMode.Physics)
        {
            Release();
        }
    }

    void HandleAttachGrab()
    {
        bool attachPressed =
            hand.Input.GripDown &&
            hand.Input.TriggerHeld;

        if (!attachPressed)
            return;

        if (HeldObject != null)
        {
            if (CurrentMode ==
                GrabMode.Attach)
            {
                Release();
            }

            return;
        }

        TryGrab(
            GrabMode.Attach
        );
    }

    void TryGrab(GrabMode mode)
    {
        IGrabbable target =
            hand.Targeting.CurrentTarget;

        if (target == null)
            return;

        GrabMode supported =
            target.GetSupportedGrabMode();

        if (!supported.HasFlag(mode))
            return;

        bool success =
            target.Grab(hand, mode);

        if (!success)
            return;

        HeldObject = target;

        CurrentMode = mode;
    }

    void ValidateDistance()
    {
        if (HeldObject == null)
            return;

        Transform point =
            HeldObject.GetGrabPoint();

        if (point == null)
            return;

        float distance =
            Vector3.Distance(
                hand.holdPoint.position,
                point.position
            );

        if (distance >
            hand.maxHoldDistance)
        {
            Release();
        }
    }

    public void Release()
    {
        if (HeldObject == null)
            return;

        HeldObject.Release(hand);

        HeldObject = null;

        CurrentMode = GrabMode.None;
    }
}