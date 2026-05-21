using UnityEngine;

public abstract class InteractableObject :
    MonoBehaviour
{
    protected Rigidbody rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void ResetPhysics()
    {
        if (rb == null)
            return;

        if (rb.isKinematic)
            return;

        rb.velocity = Vector3.zero;

        rb.angularVelocity = Vector3.zero;
    }
}