using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHandInteractor : MonoBehaviour
{
    [Header("Input")]
    public InputActionProperty grip;

    public InputActionProperty trigger;

    public InputActionProperty lowerButton;

    [Header("Ray")]
    public XRRayInteractor ray;

    [Header("Haptics")]
    public XRBaseController controller;

    [Header("Points")]
    public Transform gravityPoint;

    public Transform holdPoint;

    public Transform heavyPoint;

    [Header("Validation")]
    public float maxHoldDistance = 3f;

    public XRHandInput Input { get; private set; }

    public XRHandTargeting Targeting { get; private set; }

    public XRHandHolding Holding { get; private set; }

    void Awake()
    {
        if (controller == null)
        {
            controller =
                GetComponentInParent
                <XRBaseController>();
        }

        Input = new XRHandInput(this);

        Targeting = new XRHandTargeting(this);

        Holding = new XRHandHolding(this);
    }

    public void SendHaptic(
        float amplitude,
        float duration)
    {
        if (controller == null)
            return;

        controller.SendHapticImpulse(
            Mathf.Clamp01(amplitude),
            Mathf.Max(0f, duration)
        );
    }

    void Update()
    {
        Targeting.Tick();

        Holding.Tick();
    }
}
