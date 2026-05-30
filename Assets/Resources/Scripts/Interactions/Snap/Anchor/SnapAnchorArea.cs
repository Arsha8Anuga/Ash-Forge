using UnityEngine;

public class SnapAnchorArea : MonoBehaviour
{
    [SerializeField]
    private Collider areaCollider;

    public Collider AreaCollider =>
        areaCollider;

    void Awake()
    {
        if (areaCollider == null)
            areaCollider = GetComponent<Collider>();

        if (areaCollider != null)
            areaCollider.isTrigger = true;
    }

    void Reset()
    {
        areaCollider = GetComponent<Collider>();

        if (areaCollider != null)
            areaCollider.isTrigger = true;
    }

    void OnDrawGizmosSelected()
    {
        if (areaCollider == null)
            areaCollider = GetComponent<Collider>();

        if (areaCollider == null)
            return;

        Gizmos.color = Color.yellow;

        Bounds bounds =
            areaCollider.bounds;

        Gizmos.DrawWireCube(
            bounds.center,
            bounds.size
        );
    }
}