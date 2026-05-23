using UnityEngine;

public abstract class InteractableObject :
    MonoBehaviour
{
    protected Rigidbody rb;

    protected virtual void Awake()
    {
        RefreshRigidbody();
    }

    public virtual void RefreshRigidbody()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void ResetPhysics()
    {
        RefreshRigidbody();

        if (rb == null)
            return;

        if (rb.isKinematic)
            return;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}