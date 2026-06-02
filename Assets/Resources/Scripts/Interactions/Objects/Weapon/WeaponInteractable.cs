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

    [Header("Fire Rule")]
    [SerializeField]
    private bool requireAttachModeToFire = true;

    [SerializeField]
    private bool requireSameHoldingHandToFire = true;

    [SerializeField]
    private bool debugFireBlock;

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

        ResolveReferences();
    }

    void ResolveReferences()
    {
        RefreshRigidbody();

        if (attachFollower == null)
        {
            attachFollower =
                GetComponent<WeaponAttachFollower>();
        }

        if (layerHandler == null)
        {
            layerHandler =
                GetComponent<WeaponLayerHandler>();
        }

        if (shooter == null)
        {
            shooter =
                GetComponent<WeaponShooter>();
        }
    }

    public override bool Grab(
        XRHandInteractor hand,
        GrabMode mode)
    {
        ResolveReferences();

        if (hand == null)
            return false;

        if (mode == GrabMode.Attach &&
            attachFollower == null)
        {
            Debug.LogWarning(
                "[WeaponInteractable] Cannot use attach mode. " +
                "WeaponAttachFollower is missing.",
                this
            );

            return false;
        }

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
            if (attachFollower == null)
                return;

            attachFollower.Tick(
                currentHand
            );
        }
    }

    void BeginPhysicsMode()
    {
        RefreshRigidbody();

        if (rb == null)
            return;

        rb.isKinematic = false;

        rb.useGravity = false;

        rb.drag = damping;

        if (layerHandler != null)
        {
            layerHandler.SetHeldObjectLayer();
        }
    }

    void EndPhysicsMode()
    {
        RefreshRigidbody();

        if (rb != null)
        {
            rb.useGravity = true;

            rb.drag = 0f;

            rb.interpolation =
                RigidbodyInterpolation.Interpolate;
        }

        if (layerHandler != null)
        {
            layerHandler.Restore();
        }
    }

    void BeginAttachMode()
    {
        isAttached = true;

        ResetPhysics();

        rb.isKinematic = true;

        rb.useGravity = false;

        rb.interpolation = RigidbodyInterpolation.None;

        if (attachFollower != null)
        {
            attachFollower.Begin(
                currentHand
            );
        }

        if (layerHandler != null)
        {
            layerHandler.SetHeldWeaponLayer();
        }
    }

    void EndAttachMode()
    {
        isAttached = false;

        if (attachFollower != null)
        {
            attachFollower.End();
        }

        rb.isKinematic = false;

        rb.useGravity = true;

        if (layerHandler != null)
        {
            layerHandler.Restore();
        }
    }

    public void Activate(
        XRHandInteractor hand)
    {
        if (!CanFireFromHand(hand))
            return;

        if (shooter == null)
            return;

        shooter.Fire();
    }

    bool CanFireFromHand(
        XRHandInteractor hand)
    {
        if (hand == null)
        {
            LogFireBlock(
                "blocked: hand null"
            );

            return false;
        }

        if (!IsHeld)
        {
            LogFireBlock(
                "blocked: weapon is not held"
            );

            return false;
        }

        if (requireSameHoldingHandToFire &&
            currentHand != hand)
        {
            LogFireBlock(
                "blocked: different hand"
            );

            return false;
        }

        if (requireAttachModeToFire &&
            currentMode != GrabMode.Attach)
        {
            LogFireBlock(
                "blocked: weapon is not in attach mode"
            );

            return false;
        }

        return true;
    }

    void LogFireBlock(
        string message)
    {
        if (!debugFireBlock)
            return;

        Debug.Log(
            "[WeaponInteractable] Fire " +
            message,
            this
        );
    }
}