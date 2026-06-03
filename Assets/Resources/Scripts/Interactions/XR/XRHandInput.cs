using UnityEngine.InputSystem;

public class XRHandInput
{
    private XRHandInteractor hand;

    public XRHandInput(
        XRHandInteractor hand)
    {
        this.hand = hand;
    }

    public bool GripHeld =>
        IsPressed(hand.grip);

    public bool TriggerHeld =>
        IsPressed(hand.trigger);

    public bool LowerButtonHeld =>
        IsPressed(hand.lowerButton);

    public bool GripDown =>
        WasPressedThisFrame(hand.grip);

    public bool TriggerDown =>
        WasPressedThisFrame(hand.trigger);

    public bool LowerButtonDown =>
        WasPressedThisFrame(hand.lowerButton);

    public bool GripUp =>
        WasReleasedThisFrame(hand.grip);

    public bool TriggerUp =>
        WasReleasedThisFrame(hand.trigger);

    public bool LowerButtonUp =>
        WasReleasedThisFrame(hand.lowerButton);

    static bool IsPressed(
        InputActionProperty input)
    {
        return input.action != null &&
            input.action.IsPressed();
    }

    static bool WasPressedThisFrame(
        InputActionProperty input)
    {
        return input.action != null &&
            input.action.WasPressedThisFrame();
    }

    static bool WasReleasedThisFrame(
        InputActionProperty input)
    {
        return input.action != null &&
            input.action.WasReleasedThisFrame();
    }
}