using UnityEngine;

public class SnapSocketValidator
{
    private readonly SnapSocket socket;

    public SnapSocketValidator(
        SnapSocket socket)
    {
        this.socket = socket;
    }

    public bool ShouldIgnore(
        SnappableObject snap)
    {
        if (snap == null)
            return true;

        if (socket.Point == null)
        {
            socket.Log("Rejected: point missing");
            return true;
        }

        if (snap.IsSnapped ||
            snap.IsSnapping ||
            !snap.CanAttemptSnap)
        {
            return true;
        }

        SnapChainNode snapNode =
            snap.GetComponent<SnapChainNode>();

        if (socket.UseChainMode &&
            socket.OwnerNode != null &&
            snapNode == socket.OwnerNode)
        {
            return true;
        }

        return false;
    }

    public bool IsHeld(
        SnappableObject snap)
    {
        IGrabbable grab =
            snap.GetComponent<IGrabbable>();

        return grab != null &&
            grab.IsHeld;
    }

    public bool IsMovingTooFast(
        SnappableObject snap)
    {
        Rigidbody rb =
            snap.GetComponent<Rigidbody>();

        if (rb == null)
            return false;

        return rb.velocity.magnitude >
            socket.MaxSnapVelocity;
    }

    public bool CanAccept(
        SnappableObject snap)
    {
        if (snap == null)
            return false;

        if (socket.Point == null)
            return false;

        if (socket.UseChainMode &&
            socket.Current != null)
        {
            return false;
        }

        if (!CanAcceptSnapType(snap))
            return false;

        if (!socket.UseChainMode &&
            !CanAcceptItemIdentity(snap))
        {
            return false;
        }

        if (!socket.UseChainMode &&
            socket.CurrentStack >= socket.MaxStack)
        {
            return false;
        }

        if (!socket.UseChainMode &&
            socket.Host != null &&
            !socket.Host.CanAdd(1))
        {
            return false;
        }

        return true;
    }

    public bool IsAnchorCloseEnough(
        SnapAnchor anchor)
    {
        if (anchor == null ||
            socket.Point == null)
        {
            return false;
        }

        float distance =
            Vector3.Distance(
                anchor.transform.position,
                socket.Point.position
            );

        return distance <=
            socket.MaxAnchorSnapDistance;
    }

    bool CanAcceptSnapType(
        SnappableObject snap)
    {
        SnapTypeData[] acceptedTypes =
            socket.AcceptedTypes;

        if (acceptedTypes == null ||
            acceptedTypes.Length == 0)
        {
            return true;
        }

        bool hasValidFilter = false;

        foreach (SnapTypeData type
            in acceptedTypes)
        {
            if (type == null)
                continue;

            hasValidFilter = true;

            if (snap.Type == null)
                return false;

            if (type == snap.Type)
                return true;
        }

        return !hasValidFilter;
    }

    bool CanAcceptItemIdentity(
        SnappableObject snap)
    {
        PhysicalItem item =
            snap.GetComponent<PhysicalItem>();

        if (socket.Current == null)
            return true;

        if (socket.CurrentItem == null ||
            item == null)
        {
            return snap.Type ==
                socket.Current.Type;
        }

        return socket.CurrentItem
            .IsSameItem(item);
    }
}