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
    private WeaponAudio audioFeedback;

    [SerializeField]
    private WeaponEvents weaponEvents;

    [SerializeField]
    private WeaponHaptics weaponHaptics;

    [SerializeField]
    private WeaponMagazineControllerRelease magazineControllerRelease;

    [Header("Fire Rule")]
    [SerializeField]
    private WeaponFireMode fireMode =
        WeaponFireMode.SemiAuto;

    [SerializeField]
    private bool requireAttachModeToFire = true;

    [SerializeField]
    private bool requireSameHoldingHandToFire = true;

    [SerializeField]
    private bool debugFireBlock;

    [SerializeField]
    private float failedFireRepeatDelay = 0.22f;

    [SerializeField]
    private bool requireTriggerReleaseAfterGrab = true;

    [SerializeField]
    private bool blockFireDuringGripTransition = true;

    [SerializeField]
    private bool isAttached;

    private float nextFailedFireTime;

    private bool triggerReleasedAfterGrab = true;

    public bool IsAttached =>
        isAttached;

    public WeaponFireMode FireMode =>
        fireMode;

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

        if (audioFeedback == null)
        {
            audioFeedback =
                GetComponent<WeaponAudio>();
        }

        if (weaponEvents == null)
        {
            weaponEvents =
                GetComponent<WeaponEvents>();
        }

        if (weaponHaptics == null)
        {
            weaponHaptics =
                GetComponent<WeaponHaptics>();
        }

        if (magazineControllerRelease == null)
        {
            magazineControllerRelease =
                GetComponent<WeaponMagazineControllerRelease>();
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
            base.Grab(
                hand,
                mode
            );

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

        ResetTriggerFireGate(
            hand
        );

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

        triggerReleasedAfterGrab = false;

        base.Release(
            hand
        );
    }

    void Update()
    {
        HandleFullAutoFire();
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
        rb.interpolation =
            RigidbodyInterpolation.None;

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
        FireFromHand(
            hand
        );
    }

    void HandleFullAutoFire()
    {
        if (fireMode !=
            WeaponFireMode.FullAuto)
        {
            return;
        }

        if (!IsHeld ||
            currentHand == null ||
            currentHand.Input == null)
        {
            return;
        }

        if (IsGripTransitionBlockingFire(
            currentHand
        ))
        {
            return;
        }

        if (IsMagazineReleaseComboBlockingFire(
            currentHand
        ))
        {
            return;
        }

        if (!currentHand.Input.TriggerHeld)
        {
            MarkTriggerReleasedAfterGrab();
            return;
        }

        if (!CanUseTriggerAfterGrab(
            currentHand
        ))
        {
            return;
        }

        if (shooter != null &&
            shooter.IsCoolingDown)
        {
            return;
        }

        if (Time.time <
            nextFailedFireTime)
        {
            return;
        }

        FireFromHand(
            currentHand
        );
    }

    void FireFromHand(
        XRHandInteractor hand)
    {
        if (!CanFireFromHand(
            hand))
        {
            return;
        }

        if (IsGripTransitionBlockingFire(
            hand
        ))
        {
            return;
        }

        if (IsMagazineReleaseComboBlockingFire(
            hand
        ))
        {
            return;
        }

        if (fireMode ==
            WeaponFireMode.FullAuto &&
            !CanUseTriggerAfterGrab(
                hand
            ))
        {
            return;
        }

        if (shooter == null)
            return;

        WeaponFireResult result =
            shooter.Fire();

        if (result != WeaponFireResult.Fired &&
            result != WeaponFireResult.Cooldown &&
            result != WeaponFireResult.None)
        {
            nextFailedFireTime =
                Time.time +
                Mathf.Max(
                    0f,
                    failedFireRepeatDelay
                );
        }

        if (weaponEvents != null)
        {
            weaponEvents.RaiseFireResult(
                result,
                hand
            );
        }

        if (weaponHaptics != null)
        {
            weaponHaptics.PlayFireResult(
                result,
                hand
            );
        }

        if (audioFeedback != null)
        {
            audioFeedback.PlayFireResult(
                result
            );
        }
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

    bool IsMagazineReleaseComboBlockingFire(
        XRHandInteractor hand)
    {
        if (magazineControllerRelease == null)
            return false;

        if (!magazineControllerRelease
            .IsReleaseComboActive(hand))
        {
            return false;
        }

        LogFireBlock(
            "blocked: magazine release combo is active"
        );

        return true;
    }

    bool IsGripTransitionBlockingFire(
        XRHandInteractor hand)
    {
        if (!blockFireDuringGripTransition)
            return false;

        if (hand == null ||
            hand.Input == null)
        {
            return false;
        }

        if (!hand.Input.GripDown &&
            !hand.Input.GripUp)
        {
            return false;
        }

        LogFireBlock(
            "blocked: grip transition is being used for grab or release"
        );

        return true;
    }

    void ResetTriggerFireGate(
        XRHandInteractor hand)
    {
        if (!requireTriggerReleaseAfterGrab)
        {
            triggerReleasedAfterGrab = true;
            return;
        }

        if (hand == null ||
            hand.Input == null)
        {
            triggerReleasedAfterGrab = false;
            return;
        }

        triggerReleasedAfterGrab =
            !hand.Input.TriggerHeld;
    }

    void MarkTriggerReleasedAfterGrab()
    {
        if (!requireTriggerReleaseAfterGrab)
            return;

        triggerReleasedAfterGrab = true;
    }

    bool CanUseTriggerAfterGrab(
        XRHandInteractor hand)
    {
        if (!requireTriggerReleaseAfterGrab)
            return true;

        if (triggerReleasedAfterGrab)
            return true;

        if (hand == null ||
            hand.Input == null)
        {
            return false;
        }

        if (!hand.Input.TriggerHeld)
        {
            triggerReleasedAfterGrab = true;
            return false;
        }

        LogFireBlock(
            "blocked: trigger must be released after grab"
        );

        return false;
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
