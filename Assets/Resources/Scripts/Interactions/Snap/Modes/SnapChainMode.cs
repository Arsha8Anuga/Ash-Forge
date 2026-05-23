using UnityEngine;

public class SnapChainMode
{
    private readonly SnapSocket socket;
    private readonly SnapSocketValidator validator;

    public SnapChainMode(
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

        if (socket.OwnerNode == null)
        {
            socket.Log("Chain failed: ownerNode missing");
            return false;
        }

        SnapChainNode childNode =
            snap.GetComponent<SnapChainNode>();

        if (!ValidateChildNode(childNode))
            return false;

        SnapAnchor anchor =
            snap.GetBestAnchor(
                socket.AcceptedAnchorTags,
                socket.Point.position
            );

        if (!ValidateAnchor(anchor))
            return false;

        SnapHost root =
            socket.ResolveRootHost();

        if (root == null)
        {
            socket.Log("Chain failed: root host missing");
            return false;
        }

        if (!childNode.CanAttachTo(
            socket.OwnerNode))
        {
            socket.Log("Chain failed: possible cycle");
            return false;
        }

        int amount =
            childNode.CountSubtree();

        if (!root.CanAdd(amount))
        {
            socket.Log("Chain failed: root capacity full");
            return false;
        }

        snap.Snap(socket, anchor);

        if (!snap.IsSnapped)
        {
            socket.Log("Chain failed: snap did not complete");
            return false;
        }

        socket.SetCurrent(snap);
        socket.SetCurrentStack(1);

        childNode.AttachTo(
            socket.OwnerNode,
            root.Root
        );

        return true;
    }

    bool ValidateChildNode(
        SnapChainNode childNode)
    {
        if (childNode == null)
        {
            socket.Log("Chain failed: childNode missing");
            return false;
        }

        if (childNode == socket.OwnerNode)
        {
            socket.Log("Chain failed: trying to snap own object");
            return false;
        }

        if (childNode.RootHost != null &&
            socket.OwnerNode.RootHost != null &&
            childNode.RootHost ==
            socket.OwnerNode.RootHost)
        {
            socket.Log("Chain failed: same root host");
            return false;
        }

        return true;
    }

    bool ValidateAnchor(
        SnapAnchor anchor)
    {
        if (anchor == null)
        {
            socket.Log("Chain failed: no matching anchor");
            return false;
        }

        if (!validator.IsAnchorCloseEnough(anchor))
        {
            socket.Log(
                "Chain failed: anchor too far. Distance: " +
                Vector3.Distance(
                    anchor.transform.position,
                    socket.Point.position
                )
            );

            return false;
        }

        return true;
    }
}