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

        if (!validator.IsAnchorValidForSocket(anchor))
        {
            socket.Log(
                "Stack failed: anchor too far"
            );

            return false;
        }

        int incomingAmount =
            GetIncomingAmount(snap);

        if (!socket.CanRegisterStack(
            incomingAmount))
        {
            socket.Log(
                "Stack failed: not enough stack capacity"
            );

            return false;
        }

        if (socket.Current == null)
        {
            return SnapFirstItem(
                snap,
                anchor,
                incomingAmount
            );
        }

        return StackAdditionalItem(
            snap,
            incomingAmount
        );
    }

    bool SnapFirstItem(
        SnappableObject snap,
        SnapAnchor anchor,
        int incomingAmount)
    {
        PhysicalItem item =
            snap.GetComponent<PhysicalItem>();

        snap.Snap(
            socket,
            anchor
        );

        if (!snap.IsSnapped)
        {
            socket.Log(
                "Stack failed: first item snap failed"
            );

            return false;
        }

        if (!socket.TryRegisterStack(
            incomingAmount))
        {
            snap.ForceUnsnapWithoutSocketClear();

            socket.Log(
                "Stack failed: register first item failed"
            );

            return false;
        }

        socket.SetCurrent(snap);

        socket.SetCurrentItem(item);

        if (item != null)
        {
            item.SetAmount(
                socket.CurrentStack
            );
        }

        socket.Log(
            "Stack first item snapped | amount: " +
            socket.CurrentStack
        );

        return true;
    }

    bool StackAdditionalItem(
        SnappableObject snap,
        int incomingAmount)
    {
        PhysicalItem incomingItem =
            snap.GetComponent<PhysicalItem>();

        PhysicalItem currentItem =
            socket.CurrentItem;

        if (incomingItem == null ||
            currentItem == null)
        {
            socket.Log(
                "Stack failed: missing PhysicalItem"
            );

            return false;
        }

        if (!currentItem.IsSameItem(
            incomingItem))
        {
            socket.Log(
                "Stack failed: item mismatch"
            );

            return false;
        }

        if (!socket.TryRegisterStack(
            incomingAmount))
        {
            socket.Log(
                "Stack failed: register additional item failed"
            );

            return false;
        }

        currentItem.SetAmount(
            socket.CurrentStack
        );

        snap.gameObject.SetActive(false);

        Object.Destroy(
            snap.gameObject
        );

        socket.Log(
            "Stack additional item merged | amount: " +
            socket.CurrentStack
        );

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
}