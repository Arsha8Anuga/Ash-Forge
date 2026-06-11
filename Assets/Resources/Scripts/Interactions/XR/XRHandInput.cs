using UnityEngine.InputSystem;

public class XRHandInput
{
    private XRHandInteractor hand;

    private bool currentLowerButtonHeld;
    private bool previousLowerButtonHeld;

    public XRHandInput(
        XRHandInteractor hand)
    {
        this.hand = hand;
    }

    public void Tick()
    {
        previousLowerButtonHeld =
            currentLowerButtonHeld;

        currentLowerButtonHeld =
            IsPressed(hand.lowerButton) ||
            ReadXRButton(UnityEngine.XR.CommonUsages.primaryButton) ||
            ReadXRButton(UnityEngine.XR.CommonUsages.secondaryButton);
    }

    public bool GripHeld =>
        IsPressed(hand.grip);

    public bool TriggerHeld =>
        IsPressed(hand.trigger);

    public bool LowerButtonHeld =>
        currentLowerButtonHeld;

    public bool GripDown =>
        WasPressedThisFrame(hand.grip);

    public bool TriggerDown =>
        WasPressedThisFrame(hand.trigger);

    public bool LowerButtonDown =>
        currentLowerButtonHeld &&
        !previousLowerButtonHeld;

    public bool GripUp =>
        WasReleasedThisFrame(hand.grip);

    public bool TriggerUp =>
        WasReleasedThisFrame(hand.trigger);

    public bool LowerButtonUp =>
        !currentLowerButtonHeld &&
        previousLowerButtonHeld;

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

    bool ReadXRButton(
        UnityEngine.XR.InputFeatureUsage<bool> button)
    {
        UnityEngine.XR.InputDevice device =
            UnityEngine.XR.InputDevices.GetDeviceAtXRNode(
                hand.handNode
            );

        if (!device.isValid)
            return false;

        if (!device.TryGetFeatureValue(
            button,
            out bool pressed))
        {
            return false;
        }

        return pressed;
    }
}