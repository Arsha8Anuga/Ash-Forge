using UnityEngine;

public class StorageInteractable :
    MonoBehaviour,
    IActivatable,
    IGrabbable
{
    [SerializeField]
    private StorageOutputSpawner outputSpawner;

    [SerializeField]
    private GrabMode supportedModes =
        GrabMode.Physics;

    [SerializeField]
    private float cooldown = 0.15f;

    [SerializeField]
    private bool requireEmptyHand = true;

    [SerializeField]
    private bool debugLog;

    private float nextUseTime;

    public bool IsHeld =>
        false;

    void Awake()
    {
        if (outputSpawner == null)
        {
            outputSpawner =
                GetComponentInChildren
                <StorageOutputSpawner>();
        }
    }

    public void Activate(
        XRHandInteractor hand)
    {
        if (!CanUse(hand))
            return;

        nextUseTime =
            Time.time + cooldown;

        bool success =
            outputSpawner.SpawnOutput();

        Log("Activate spawn to area: " + success);
    }

    public bool Grab(
        XRHandInteractor hand,
        GrabMode mode)
    {
        if (!supportedModes.HasFlag(mode))
            return false;

        if (!CanUse(hand))
            return false;

        nextUseTime =
            Time.time + cooldown;

        bool success =
            outputSpawner.SpawnOutputAndGrab(
                hand
            );

        Log("Grip spawn and grab: " + success);

        return false;
    }

    public void Release(
        XRHandInteractor hand)
    {
    }

    public GrabMode GetSupportedGrabMode()
    {
        return supportedModes;
    }

    public Transform GetGrabPoint()
    {
        return transform;
    }

    bool CanUse(
        XRHandInteractor hand)
    {
        if (hand == null)
            return false;

        if (Time.time < nextUseTime)
            return false;

        if (outputSpawner == null)
            return false;

        if (requireEmptyHand &&
            hand.Holding.HeldObject != null)
        {
            return false;
        }

        return outputSpawner.CanSpawnOutput();
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[StorageInteractable] " +
            message,
            this
        );
    }
}