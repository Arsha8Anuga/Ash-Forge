using UnityEngine;

public static class SnapTransformUtility
{
    public static void KeepWorldScale(
        Transform target,
        Vector3 worldScale)
    {
        Transform parent =
            target.parent;

        if (parent == null)
        {
            target.localScale =
                worldScale;

            return;
        }

        Vector3 parentScale =
            parent.lossyScale;

        target.localScale =
            new Vector3(
                SafeDivide(
                    worldScale.x,
                    parentScale.x
                ),
                SafeDivide(
                    worldScale.y,
                    parentScale.y
                ),
                SafeDivide(
                    worldScale.z,
                    parentScale.z
                )
            );
    }

    static float SafeDivide(
        float value,
        float divisor)
    {
        if (Mathf.Approximately(
            divisor,
            0f
        ))
        {
            return value;
        }

        return value / divisor;
    }
}