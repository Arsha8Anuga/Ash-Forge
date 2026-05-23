using UnityEngine;

public class SnapCompoundHandler
{
    private readonly GameObject owner;

    private RigidbodySnapshot snapshot;

    public SnapCompoundHandler(
        GameObject owner)
    {
        this.owner = owner;
    }

    public void RemoveRigidbody()
    {
        Rigidbody rb =
            owner.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        snapshot =
            new RigidbodySnapshot(rb);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Object.Destroy(rb);
    }

    public void RestoreRigidbody()
    {
        Rigidbody rb =
            owner.GetComponent<Rigidbody>();

        if (rb != null)
            return;

        rb =
            owner.AddComponent<Rigidbody>();

        if (snapshot != null)
        {
            snapshot.ApplyTo(rb);
        }
    }
}