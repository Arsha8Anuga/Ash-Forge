using UnityEngine;

public class SnapAnchor : MonoBehaviour
{
    [SerializeField]
    private string[] anchorTags;

    public string[] AnchorTags =>
        anchorTags;

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

        foreach (string accepted
            in acceptedTags)
        {
            if (string.IsNullOrEmpty(accepted))
                continue;

            foreach (string tag
                in anchorTags)
            {
                if (string.IsNullOrEmpty(tag))
                    continue;

                if (accepted == tag)
                    return true;
            }
        }

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(
            transform.position,
            0.035f
        );

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            transform.up * 0.15f
        );
    }
}