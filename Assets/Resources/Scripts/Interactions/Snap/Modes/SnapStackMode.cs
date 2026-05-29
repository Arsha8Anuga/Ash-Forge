using UnityEngine;

public class SnapStackMode
{
    private readonly SnapSocket socket;
    private readonly SnapSocketValidator validator;

    public SnapStackMode(
        SnapSocket socket,
        SnapSocketValidator validator)
    {
        this.socket = socket;
        this.validator = validator;
    }

    public bool TrySnap(
        SnappableObject snap)
    {
        if (snap == null)
            return false;

        if (!socket.CanAccept(snap))
            return false;

        SnapAnchor anchor =
            snap.GetBestAnchor(
                socket.AcceptedAnchorTags,
                socket.Point.position
            );

        if (anchor == null)
        {
            socket.Log(
                "Stack failed: no valid anchor"
            );

            return false;
        }

        if (!validator.IsAnchorCloseEnough(anchor))
        {
            socket.Log(
                "Stack failed: anchor too far"
            );

            return false;
        }

        if (!socket.TryRegisterStack())
        {
            socket.Log(
                "Stack failed: register failed"
            );

            return false;
        }

        if (socket.Current == null)
        {
            return SnapFirstItem(
                snap,
                anchor
            );
        }

        return StackAdditionalItem(
            snap
        );
    }

    bool SnapFirstItem(
        SnappableObject snap,
        SnapAnchor anchor)
    {
        socket.SetCurrent(snap);

        socket.SetCurrentItem(
            snap.GetComponent<PhysicalItem>()
        );

        snap.Snap(
            socket,
            anchor
        );

        if (!snap.IsSnapped)
        {
            socket.Clear(snap);

            socket.Log(
                "Stack failed: first item snap failed"
            );

            return false;
        }

        socket.Log(
            "Stack first item snapped"
        );

        return true;
    }

    bool StackAdditionalItem(
        SnappableObject snap)
    {
        PhysicalItem item =
            snap.GetComponent<PhysicalItem>();

        if (item == null)
        {
            socket.Log(
                "Stack failed: no PhysicalItem"
            );

            return false;
        }

        socket.Log(
            "Stack additional item: " +
            item.ItemName
        );

        Object.Destroy(
            snap.gameObject
        );

        return true;
    }

    
}