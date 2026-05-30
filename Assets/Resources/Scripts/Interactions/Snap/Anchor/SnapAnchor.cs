using UnityEngine;

public class SnapAnchor : MonoBehaviour
{
    [SerializeField]
    private Transform point;

    [SerializeField]
    private SnapAnchorArea area;

    [SerializeField]
    private string[] anchorTags;

    public Transform Point =>
        point != null
        ? point
        : transform;

    public SnapAnchorArea Area =>
        area;

    public string[] AnchorTags =>
        anchorTags;

    void Awake()
    {
        if (point == null)
            point = transform;

        if (area == null)
            area = GetComponentInChildren<SnapAnchorArea>();
    }

    void Reset()
    {
        point = transform;
        area = GetComponentInChildren<SnapAnchorArea>();
    }

    public bool HasAnyTag(
        string[] acceptedTags)
    {
        if (acceptedTags == null ||
            acceptedTags.Length == 0)
        {
            return true;
        }

        if (anchorTags == null ||
            anchorTags.Length == 0)
        {
            return false;
        }

        foreach (string accepted in acceptedTags)
        {
            if (string.IsNullOrEmpty(accepted))
                continue;

            foreach (string tag in anchorTags)
            {
                if (string.IsNullOrEmpty(tag))
                    continue;

                if (accepted == tag)
                    return true;
            }
        }

        return false;
    }
}