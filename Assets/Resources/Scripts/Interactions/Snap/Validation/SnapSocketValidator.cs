using UnityEngine;

public class SnapSocketValidator
{
    private readonly SnapSocket socket;

    private const float insideTolerance = 0.02f;

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

        if (!socket.UseChainMode)
        {
            int incomingAmount =
                GetIncomingAmount(snap);

            if (!socket.CanRegisterStack(
                incomingAmount))
            {
                return false;
            }
        }

        return true;
    }

    int GetIncomingAmount(
        SnappableObject snap)
    {
        PhysicalItem item =
            snap.GetComponent<PhysicalItem>();

        if (item == null)
            return 1;

        return Mathf.Max(
            1,
            item.Amount
        );
    }

    public bool IsAnchorCloseEnough(
        SnapAnchor anchor)
    {
        if (anchor == null ||
            socket.Point == null)
        {
            return false;
        }

        if (anchor.Point == null)
            return false;

        float distance =
            Vector3.Distance(
                anchor.Point.position,
                socket.Point.position
            );

        return distance <=
            socket.MaxAnchorSnapDistance;
    }

    public bool IsAnchorReadable(
        SnapAnchor anchor)
    {
        if (anchor == null)
            return false;

        if (socket == null)
            return false;

        if (socket.Point == null)
            return false;

        Collider socketArea =
            socket.SocketArea;

        if (socketArea == null)
            return IsAnchorCloseEnough(anchor);

        if (IsAnchorPointInsideSocketArea(
            anchor,
            socketArea))
        {
            return true;
        }

        if (IsAnchorAreaOverlappingSocketArea(
            anchor,
            socketArea))
        {
            return true;
        }

        return false;
    }

    public bool IsAnchorValidForSocket(
        SnapAnchor anchor)
    {
        if (anchor == null)
            return false;

        if (!anchor.HasAnyTag(
            socket.AcceptedAnchorTags))
        {
            return false;
        }

        if (!IsAnchorReadable(anchor))
            return false;

        return true;
    }

    bool IsAnchorPointInsideSocketArea(
        SnapAnchor anchor,
        Collider socketArea)
    {
        if (anchor == null ||
            socketArea == null)
        {
            return false;
        }

        if (anchor.Point == null)
            return false;

        Vector3 point =
            anchor.Point.position;

        Vector3 closest =
            socketArea.ClosestPoint(point);

        float distance =
            Vector3.Distance(
                point,
                closest
            );

        return distance <= insideTolerance;
    }

    bool IsAnchorAreaOverlappingSocketArea(
        SnapAnchor anchor,
        Collider socketArea)
    {
        if (anchor == null ||
            socketArea == null)
        {
            return false;
        }

        SnapAnchorArea area =
            anchor.Area;

        if (area == null ||
            area.AreaCollider == null)
        {
            return false;
        }

        return socketArea.bounds.Intersects(
            area.AreaCollider.bounds
        );
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