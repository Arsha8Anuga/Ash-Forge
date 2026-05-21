using UnityEngine;

public class WeaponInteractable :
    SmallObjectInteractable,
    IActivatable
{
    [Header("Weapon")]
    [SerializeField]
    private WeaponAttachFollower attachFollower;

    [SerializeField]
    private WeaponLayerHandler layerHandler;

    [SerializeField]
    private WeaponShooter shooter;

    [SerializeField]
    private bool isAttached;

    public bool IsAttached =>
        isAttached;

    protected override void Awake()
    {
        base.Awake();

        supportedModes =
            GrabMode.Physics |
            GrabMode.Attach;
    }

    public override bool Grab(
        XRHandInteractor hand,
        GrabMode mode)
    {
        bool success =
            base.Grab(hand, mode);

        if (!success)
            return false;

        switch (mode)
        {
            case GrabMode.Physics:
                BeginPhysicsMode();
                break;

            case GrabMode.Attach:
                BeginAttachMode();
                break;
        }

        return true;
    }

    public override void Release(
        XRHandInteractor hand)
    {
        switch (currentMode)
        {
            case GrabMode.Physics:
                EndPhysicsMode();
                break;

            case GrabMode.Attach:
                EndAttachMode();
                break;
        }

        base.Release(hand);
    }

    void FixedUpdate()
    {
        if (!IsHeld)
            return;

        if (currentMode ==
            GrabMode.Physics)
        {
            physics.Tick();
        }
    }

    void LateUpdate()
    {
        if (!IsHeld)
            return;

        if (currentMode ==
            GrabMode.Attach)
        {
            attachFollower.Tick(
                currentHand
            );
        }
    }

    void BeginPhysicsMode()
    {
        rb.isKinematic = false;

        rb.useGravity = false;

        rb.drag = damping;

        layerHandler.SetHeldObjectLayer();
    }

    void EndPhysicsMode()
    {
        rb.useGravity = true;

        rb.drag = 0f;

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        layerHandler.Restore();
    }

    void BeginAttachMode()
    {
        isAttached = true;

        ResetPhysics();

        rb.isKinematic = true;

        rb.useGravity = false;

        rb.interpolation = RigidbodyInterpolation.None;

        layerHandler.SetHeldWeaponLayer();
    }

    void EndAttachMode()
    {
        isAttached = false;

        rb.isKinematic = false;

        rb.useGravity = true;

        layerHandler.Restore();
    }

    public void Activate(
        XRHandInteractor hand)
    {
        shooter.Fire();
    }
}