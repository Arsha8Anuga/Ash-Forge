using UnityEngine;

public class SnapAnchorResolver
{
    private readonly Transform root;

    public SnapAnchorResolver(
        Transform root)
    {
        this.root = root;
    }

    public SnapAnchor GetBestAnchor(
        string[] acceptedTags,
        Vector3 socketPosition)
    {
        if (root == null)
            return null;

        SnapAnchor[] anchors =
            root.GetComponentsInChildren
            <SnapAnchor>();

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
                    anchor.Point.position,
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