using UnityEngine;

public class SmallObjectAttach
{
    private SmallObjectInteractable owner;
    private Rigidbody rb;

    public SmallObjectAttach(
        SmallObjectInteractable owner,
        Rigidbody rb)
    {
        this.owner = owner;
        this.rb = rb;
    }

    public void Tick()
    {
        XRHandInteractor hand =
            owner.CurrentHand;

        if (hand == null)
            return;

        Transform grabPoint =
            owner.InternalGrabPoint;

        if (grabPoint == null)
        {
            owner.transform.position =
                hand.holdPoint.position;

            owner.transform.rotation =
                hand.holdPoint.rotation;
        }
        else
        {
            Quaternion rotationOffset =
                Quaternion.Inverse(
                    grabPoint.rotation
                ) *
                owner.transform.rotation;

            owner.transform.rotation =
                hand.holdPoint.rotation *
                rotationOffset;

            Vector3 positionOffset =
                owner.transform.position -
                grabPoint.position;

            owner.transform.position =
                hand.holdPoint.position +
                positionOffset;
        }

        if (rb == null)
            return;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}