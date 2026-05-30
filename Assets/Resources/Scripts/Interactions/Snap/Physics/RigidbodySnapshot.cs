using UnityEngine;

public class RigidbodySnapshot
{
    public float mass;

    public float drag;

    public float angularDrag;

    public bool useGravity;

    public RigidbodyInterpolation interpolation;

    public CollisionDetectionMode collisionDetectionMode;

    public RigidbodyConstraints constraints;

    public RigidbodySnapshot(
        Rigidbody rb)
    {
        mass =
            rb.mass;

        drag =
            rb.drag;

        angularDrag =
            rb.angularDrag;

        useGravity =
            rb.useGravity;

        interpolation =
            rb.interpolation;

        collisionDetectionMode =
            rb.collisionDetectionMode;

        constraints =
            rb.constraints;
    }

    public void ApplyTo(
        Rigidbody rb)
    {
        if (rb == null)
            return;

        rb.mass =
            mass;

        rb.drag =
            drag;

        rb.angularDrag =
            angularDrag;

        rb.useGravity =
            useGravity;

        rb.isKinematic =
            false;

        rb.interpolation =
            interpolation;

        rb.collisionDetectionMode =
            collisionDetectionMode;

        rb.constraints =
            constraints;
    }
}