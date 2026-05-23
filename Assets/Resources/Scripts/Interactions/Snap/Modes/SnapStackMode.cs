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
        if (!socket.CanAccept(snap))
            return false;

        SnapAnchor anchor =
            snap.GetBestAnchor(
                socket.AcceptedAnchorTags,
                socket.Point.position
            );

        if (anchor == null)
            return false;

        if (!validator.IsAnchorCloseEnough(anchor))
            return false;

        if (!socket.TryRegisterStack())
            return false;

        if (socket.Current == null)
        {
            socket.SetCurrent(snap);

            socket.SetCurrentItem(
                snap.GetComponent<PhysicalItem>()
            );

            snap.Snap(
                socket,
                anchor
            );
        }
        else
        {
            Object.Destroy(
                snap.gameObject
            );
        }

        return true;
    }
}