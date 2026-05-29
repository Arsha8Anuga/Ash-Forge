using UnityEngine;

public class WorkstationTool :
    MonoBehaviour
{
    [Header("Tool")]
    [SerializeField]
    private WorkstationToolType toolType;

    [SerializeField]
    private bool requireAttachMode = true;

    [SerializeField]
    private bool requireTriggerForActive;

    [Header("References")]
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private SmallObjectInteractable interactable;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    private XRHandInteractor currentHand;

    private Vector3 lastPosition;

    private float currentVelocity;

    private float holdDuration;

    public WorkstationToolType ToolType =>
        toolType;

    public bool TriggerHeld
    {
        get;
        private set;
    }

    public float HoldDuration =>
        holdDuration;

    public float CurrentVelocity =>
        currentVelocity;

    public bool IsHeld =>
        currentHand != null;

    public XRHandInteractor CurrentHand =>
        currentHand;

    public bool IsActive
    {
        get
        {
            if (requireAttachMode &&
                !IsAttached())
            {
                return false;
            }

            if (requireTriggerForActive &&
                !TriggerHeld)
            {
                return false;
            }

            return IsHeld;
        }
    }

    void Awake()
    {
        if (rb == null)
        {
            rb =
                GetComponent<Rigidbody>();
        }

        if (interactable == null)
        {
            interactable =
                GetComponent<SmallObjectInteractable>();
        }

        lastPosition =
            transform.position;
    }

    void Update()
    {
        UpdateInputState();

        UpdateVelocity();
    }

    public void SetHeldBy(
        XRHandInteractor hand)
    {
        currentHand = hand;

        holdDuration = 0f;

        TriggerHeld = false;

        Log(
            "Held by: " +
            hand.name
        );
    }

    public void ClearHeldBy(
        XRHandInteractor hand)
    {
        if (currentHand != hand)
            return;

        currentHand = null;

        holdDuration = 0f;

        TriggerHeld = false;

        Log("Released.");
    }

    void UpdateInputState()
    {
        if (currentHand == null)
        {
            TriggerHeld = false;

            holdDuration = 0f;

            return;
        }

        TriggerHeld =
            currentHand.Input.TriggerHeld;

        if (TriggerHeld)
        {
            holdDuration +=
                Time.deltaTime;
        }
        else
        {
            holdDuration = 0f;
        }
    }

    void UpdateVelocity()
    {
        if (rb != null &&
            !rb.isKinematic)
        {
            currentVelocity =
                rb.velocity.magnitude;

            lastPosition =
                transform.position;

            return;
        }

        Vector3 delta =
            transform.position -
            lastPosition;

        currentVelocity =
            delta.magnitude /
            Mathf.Max(
                Time.deltaTime,
                0.0001f
            );

        lastPosition =
            transform.position;
    }

    bool IsAttached()
    {
        if (interactable == null)
            return false;

        return
            interactable.IsHeld &&
            interactable.CurrentMode ==
            GrabMode.Attach;
    }

    public float GetVelocity()
    {
        return currentVelocity;
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[WorkstationTool] " +
            message,
            this
        );
    }
}