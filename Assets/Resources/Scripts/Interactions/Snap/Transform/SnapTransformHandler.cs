using UnityEngine;

public class SnapTransformHandler
{
    private readonly Transform owner;

    public SnapTransformHandler(
        Transform owner)
    {
        this.owner = owner;
    }

    public void AttachAndAlign(
        SnapSocket socket,
        SnapAnchor anchor)
    {
        Vector3 worldScale =
            owner.lossyScale;

        owner.SetParent(
            socket.AttachParent,
            true
        );

        SnapTransformUtility.KeepWorldScale(
            owner,
            worldScale
        );

        ApplyWorldSnap(
            socket,
            anchor
        );
    }

    public void Detach()
    {
        Vector3 worldScale =
            owner.lossyScale;

        owner.SetParent(
            null,
            true
        );

        SnapTransformUtility.KeepWorldScale(
            owner,
            worldScale
        );
    }

    void ApplyWorldSnap(
        SnapSocket socket,
        SnapAnchor anchor)
    {
        Transform socketPoint =
            socket.Point;

        Transform anchorTransform =
            anchor.transform;

        Quaternion anchorLocalRotation =
            Quaternion.Inverse(owner.rotation) *
            anchorTransform.rotation;

        Quaternion targetRotation =
            socketPoint.rotation *
            Quaternion.Inverse(anchorLocalRotation);

        owner.rotation = targetRotation;

        Vector3 correction =
            socketPoint.position -
            anchorTransform.position;

        owner.position +=
            correction +
            socketPoint.up *
            socket.SurfaceOffset;
    }
}