using UnityEngine;

public class WeaponAttachFollower :
    MonoBehaviour
{
    [SerializeField]
    private Transform attachPoint;

    [SerializeField]
    private WeaponRecoil recoil;

    private XRHandInteractor currentHand;

    public void Begin(
        XRHandInteractor hand)
    {
        currentHand = hand;

        if (recoil == null)
        {
            recoil =
                GetComponent<WeaponRecoil>();
        }

        if (recoil != null)
        {
            recoil.ResetRecoil();
        }
    }

    public void End()
    {
        currentHand = null;

        if (recoil != null)
        {
            recoil.ResetRecoil();
        }
    }

    public void Tick(
        XRHandInteractor hand)
    {
        if (hand == null)
            return;

        currentHand = hand;

        if (attachPoint == null)
            return;

        Transform holdPoint =
            hand.holdPoint;

        if (holdPoint == null)
            return;

        Vector3 targetHoldPosition =
            holdPoint.position;

        Quaternion targetHoldRotation =
            holdPoint.rotation;

        if (recoil != null)
        {
            targetHoldPosition +=
                holdPoint.TransformVector(
                    recoil.PositionOffset
                );

            targetHoldRotation =
                holdPoint.rotation *
                recoil.RotationOffset;
        }

        AlignAttachPointToTarget(
            targetHoldPosition,
            targetHoldRotation
        );
    }

    void AlignAttachPointToTarget(
        Vector3 targetPosition,
        Quaternion targetRotation)
    {
        Quaternion rotationOffset =
            Quaternion.Inverse(
                attachPoint.rotation
            ) *
            transform.rotation;

        Quaternion targetRootRotation =
            targetRotation *
            rotationOffset;

        transform.rotation =
            targetRootRotation;

        Vector3 positionOffset =
            transform.position -
            attachPoint.position;

        transform.position =
            targetPosition +
            positionOffset;
    }
}