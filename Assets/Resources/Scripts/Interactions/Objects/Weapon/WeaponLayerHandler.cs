using UnityEngine;

public class WeaponLayerHandler :
    MonoBehaviour
{
    [SerializeField]
    private string defaultLayer =
        "Interactable";

    [SerializeField]
    private string heldObjectLayer =
        "HeldObject";

    [SerializeField]
    private string heldWeaponLayer =
        "HeldWeapon";

    public void SetHeldObjectLayer()
    {
        SetLayer(
            heldObjectLayer
        );
    }

    public void SetHeldWeaponLayer()
    {
        SetLayer(
            heldWeaponLayer
        );
    }

    public void Restore()
    {
        SetLayer(
            defaultLayer
        );
    }

    void SetLayer(string layerName)
    {
        int layer =
            LayerMask.NameToLayer(
                layerName
            );

        SetRecursive(
            gameObject,
            layer
        );
    }

    void SetRecursive(
        GameObject target,
        int layer)
    {
        target.layer = layer;

        foreach (Transform child
            in target.transform)
        {
            SetRecursive(
                child.gameObject,
                layer
            );
        }
    }
}