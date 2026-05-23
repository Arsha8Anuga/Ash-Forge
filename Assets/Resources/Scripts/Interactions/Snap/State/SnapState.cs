public class SnapState
{
    public bool IsSnapped { get; private set; }

    public bool IsSnapping { get; private set; }

    public SnapSocket CurrentSocket { get; private set; }

    public SnapAnchor ActiveAnchor { get; private set; }

    public void BeginSnap(
        SnapSocket socket,
        SnapAnchor anchor)
    {
        IsSnapping = true;
        CurrentSocket = socket;
        ActiveAnchor = anchor;
    }

    public void CompleteSnap()
    {
        IsSnapped = true;
        IsSnapping = false;
    }

    public void BeginUnsnap()
    {
        IsSnapped = false;
        IsSnapping = false;
    }

    public void Clear()
    {
        CurrentSocket = null;
        ActiveAnchor = null;
        IsSnapped = false;
        IsSnapping = false;
    }
}