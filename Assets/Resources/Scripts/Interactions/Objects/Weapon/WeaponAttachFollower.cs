using UnityEngine;

public class WeaponAttachFollower :
    MonoBehaviour
{
    [SerializeField]
    private Transform attachPoint;

    public void Tick(
        XRHandInteractor hand)
    {
        if (hand == null)
            return;

        if (attachPoint == null)
            return;

        Transform holdPoint =
            hand.holdPoint;

        Quaternion rotationOffset =
            Quaternion.Inverse(
                attachPoint.rotation
            ) * transform.rotation;

        Quaternion targetRotation =
            holdPoint.rotation *
            rotationOffset;

        Vector3 positionOffset =
            transform.position -
            attachPoint.position;

        transform.position =
            holdPoint.position +
            positionOffset;

        transform.rotation =
            targetRotation;
    }
}