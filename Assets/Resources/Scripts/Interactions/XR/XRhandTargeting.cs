using UnityEngine;

public class XRHandTargeting
{
    private XRHandInteractor hand;

    public IGrabbable CurrentTarget
    {
        get;
        private set;
    }

    public IActivatable CurrentActivatable
    {
        get;
        private set;
    }

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
        CurrentActivatable = null;

        if (!hand.ray.TryGetCurrent3DRaycastHit(
            out RaycastHit hit))
        {
            return;
        }

        MonoBehaviour[] behaviours =
            hit.collider.GetComponentsInParent
            <MonoBehaviour>();

        foreach (MonoBehaviour behaviour
            in behaviours)
        {
            if (CurrentTarget == null &&
                behaviour is IGrabbable grabbable)
            {
                CurrentTarget = grabbable;
            }

            if (CurrentActivatable == null &&
                behaviour is IActivatable activatable)
            {
                CurrentActivatable = activatable;
            }

            if (CurrentTarget != null &&
                CurrentActivatable != null)
            {
                return;
            }
        }
    }
}