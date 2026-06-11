using UnityEngine;

public class BulletHoleReceiver : MonoBehaviour
{
    [Header("Bullet Hole")]
    public bool acceptBulletHole = true;

    [Header("Follow")]
    public bool followRigidbody = true;

    public Transform customParent;

    public bool CanReceiveBulletHole()
    {
        return acceptBulletHole;
    }

    public Transform GetBulletHoleParent(Collider hitCollider)
    {
        if (customParent != null)
            return customParent;

        if (followRigidbody)
        {
            Rigidbody rb = hitCollider.GetComponentInParent<Rigidbody>();

            if (rb != null)
                return rb.transform;
        }

        return hitCollider.transform;
    }
}