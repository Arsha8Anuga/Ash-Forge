using UnityEngine;

public class WeaponMagazineControllerRelease :
    MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private WeaponInteractable weapon;

    [SerializeField]
    private WeaponMagazineEjectButton releaseButton;

    [Header("Input Rule")]
    [SerializeField]
    private bool requireAttachMode = true;

    [SerializeField]
    private bool requireTriggerHeld = true;

    [SerializeField]
    private bool requireLowerButtonDown = true;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    public bool HasReleaseButton =>
        releaseButton != null;

    void Awake()
    {
        ResolveReferences();
    }

    void Update()
    {
        TryReleaseFromController();
    }

    void ResolveReferences()
    {
        if (weapon == null)
        {
            weapon =
                GetComponent<WeaponInteractable>();
        }

        if (releaseButton == null)
        {
            releaseButton =
                GetComponentInChildren
                <WeaponMagazineEjectButton>(true);
        }
    }

    public bool IsReleaseComboActive(
        XRHandInteractor hand)
    {
        if (hand == null ||
            hand.Input == null)
        {
            return false;
        }

        if (requireTriggerHeld &&
            !hand.Input.TriggerHeld)
        {
            return false;
        }

        if (!hand.Input.LowerButtonHeld)
            return false;

        return true;
    }

    void TryReleaseFromController()
    {
        ResolveReferences();

        if (weapon == null ||
            releaseButton == null)
        {
            return;
        }

        if (!weapon.IsHeld)
            return;

        if (requireAttachMode &&
            weapon.CurrentMode != GrabMode.Attach)
        {
            return;
        }

        XRHandInteractor hand =
            weapon.CurrentHand;

        if (hand == null ||
            hand.Input == null)
        {
            return;
        }

        if (requireTriggerHeld &&
            !hand.Input.TriggerHeld)
        {
            return;
        }

        bool lowerButtonValid =
            requireLowerButtonDown
            ? hand.Input.LowerButtonDown
            : hand.Input.LowerButtonHeld;

        if (!lowerButtonValid)
            return;

        releaseButton.Activate(
            hand
        );

        if (!debugLog)
            return;

        Debug.Log(
            "[WeaponMagazineControllerRelease] Controller magazine release.",
            this
        );
    }
}