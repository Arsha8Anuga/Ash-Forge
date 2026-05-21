using UnityEngine;

public class XRHandTargeting
{
    private XRHandInteractor hand;

    public IGrabbable CurrentTarget { get; private set; }

    public XRHandTargeting(
        XRHandInteractor hand)
    {
        this.hand = hand;
    }

    public void Tick()
    {
        DetectTarget();
    }

    void DetectTarget()
    {
        CurrentTarget = null;

        if (!hand.ray.TryGetCurrent3DRaycastHit(
            out RaycastHit hit))
        {
            return;
        }

        MonoBehaviour[] behaviours =
            hit.collider.GetComponentsInParent<MonoBehaviour>();

        foreach (var behaviour in behaviours)
        {
            if (behaviour is IGrabbable grabbable)
            {
                CurrentTarget = grabbable;
                return;
            }
        }
    }
}