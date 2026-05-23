using UnityEngine;

public interface ISnappable
{
    bool IsSnapped
    {
        get;
    }

    SnapSocket CurrentSocket
    {
        get;
    }

    bool CanSnap(
        SnapSocket socket
    );

    void Snap(
        SnapSocket socket
    );

    void Unsnap();
}