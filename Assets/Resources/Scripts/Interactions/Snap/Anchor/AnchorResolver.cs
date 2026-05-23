using UnityEngine;

public class SnapAnchorResolver
{
    private readonly Transform owner;

    private SnapAnchor[] anchors;

    public SnapAnchorResolver(
        Transform owner)
    {
        this.owner = owner;
        Resolve();
    }

    public void Resolve()
    {
        anchors =
            owner.GetComponentsInChildren
            <SnapAnchor>();
    }

    public SnapAnchor GetBestAnchor(
        string[] acceptedTags,
        Vector3 socketPosition)
    {
        if (anchors == null ||
            anchors.Length == 0)
        {
            Resolve();
        }

        SnapAnchor best = null;
        float bestDistance = float.MaxValue;

        foreach (SnapAnchor anchor in anchors)
        {
            if (anchor == null)
                continue;

            if (!anchor.HasAnyTag(acceptedTags))
                continue;

            float distance =
                Vector3.Distance(
                    anchor.transform.position,
                    socketPosition
                );

            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = anchor;
            }
        }

        return best;
    }
}