using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHandInteractor : MonoBehaviour
{
    [Header("Input")]
    public InputActionProperty grip;

    public InputActionProperty trigger;

    [Header("Ray")]
    public XRRayInteractor ray;

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
        Input = new XRHandInput(this);

        Targeting = new XRHandTargeting(this);

        Holding = new XRHandHolding(this);
    }

    void Update()
    {
        Targeting.Tick();

        Holding.Tick();
    }
}