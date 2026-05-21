public class XRHandInput
{
    private XRHandInteractor hand;

    public XRHandInput(
        XRHandInteractor hand)
    {
        this.hand = hand;
    }

    public bool GripHeld =>
        hand.grip.action.IsPressed();

    public bool TriggerHeld =>
        hand.trigger.action.IsPressed();

    public bool GripDown =>
        hand.grip.action.WasPressedThisFrame();

    public bool TriggerDown =>
        hand.trigger.action.WasPressedThisFrame();

    public bool GripUp =>
        hand.grip.action.WasReleasedThisFrame();

    public bool TriggerUp =>
        hand.trigger.action.WasReleasedThisFrame();
}