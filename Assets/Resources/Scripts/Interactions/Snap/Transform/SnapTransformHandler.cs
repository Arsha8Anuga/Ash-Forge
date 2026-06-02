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
        if (socket == null)
            return;

        if (anchor == null)
            return;

        Transform socketPoint =
            socket.Point;

        Transform anchorPoint =
            anchor.Point;

        if (socketPoint == null)
            return;

        if (anchorPoint == null)
            return;

        Quaternion anchorLocalRotation =
            Quaternion.Inverse(owner.rotation) *
            anchorPoint.rotation;

        Quaternion targetRotation =
            socketPoint.rotation *
            Quaternion.Inverse(anchorLocalRotation);

        owner.rotation =
            targetRotation;

        Vector3 correction =
            socketPoint.position -
            anchorPoint.position;

        owner.position +=
            correction +
            socketPoint.up *
            socket.SurfaceOffset;
    }
}