using UnityEngine;

public static class SnapLayerUtility
{
    public static void SetLayerRecursive(
        GameObject target,
        string layerName)
    {
        int layer =
            LayerMask.NameToLayer(
                layerName
            );

        if (layer < 0)
        {
            Debug.LogWarning(
                $"Layer '{layerName}' does not exist."
            );

            return;
        }

        SetLayerRecursive(
            target,
            layer
        );
    }

    static void SetLayerRecursive(
        GameObject target,
        int layer)
    {
        target.layer =
            layer;

        foreach (Transform child
            in target.transform)
        {
            SetLayerRecursive(
                child.gameObject,
                layer
            );
        }
    }
}