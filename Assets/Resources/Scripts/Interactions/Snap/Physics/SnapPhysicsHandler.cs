using UnityEngine;

public class SnapPhysicsHandler
{
    private readonly GameObject owner;

    public SnapPhysicsHandler(
        GameObject owner)
    {
        this.owner = owner;
    }

    public void StopPhysics()
    {
        Rigidbody rb =
            owner.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.WakeUp();
    }

    public void ResumePhysics()
    {
        Rigidbody rb =
            owner.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.WakeUp();
    }
}