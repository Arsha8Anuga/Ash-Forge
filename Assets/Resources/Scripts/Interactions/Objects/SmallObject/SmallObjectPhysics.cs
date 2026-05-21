using UnityEngine;

public class SmallObjectPhysics
{
    private SmallObjectInteractable owner;

    private Rigidbody rb;

    public SmallObjectPhysics(
     SmallObjectInteractable owner,
     Rigidbody rb)
    {
        this.owner =
            owner;

        this.rb =
            rb;
    }

    public void Tick()
    {
        XRHandInteractor hand =
            owner.CurrentHand;

        if (rb.isKinematic)
            return;

        if (hand == null)
            return;

        Vector3 target =
            hand.gravityPoint.position;

        Vector3 toTarget =
            target -
            rb.position;

        float distance =
            toTarget.magnitude;

        if (distance <=
            owner.deadZoneRadius)
        {
            rb.velocity *=
                0.85f;

            return;
        }

        Vector3 direction =
            toTarget.normalized;

        WeightChain chain =
            owner.GetComponentInParent<
                WeightChain>();

        float mass =
            rb.mass;

        if(
            chain != null
        )
        {
            Debug.Log(
                "Chain: " +
                chain.TotalMass
            );

            mass =
                Mathf.Max(
                    rb.mass,
                    chain.TotalMass
                );
        }
        else
        {
            Debug.Log(
                "No chain found"
            );
        }

        float weightFactor =
            Mathf.Max(
                1f,
                mass *
                owner.weightMultiplier
            );

        Vector3 springForce =
            direction *
            (
                distance -
                owner.deadZoneRadius
            ) *
            (
                owner.followForce /
                weightFactor
            );

        Vector3 dampingForce =
            rb.velocity *
            owner.damping;

        rb.AddForce(
            springForce -
            dampingForce,
            ForceMode.Acceleration
        );

        rb.velocity =
            Vector3.ClampMagnitude(
                rb.velocity,
                owner.maxVelocity
            );

        Rotate(hand);
    }

    void Rotate(
        XRHandInteractor hand)
    {
        Quaternion targetRotation =
            hand.holdPoint.rotation;

        Quaternion smoothed =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                owner.rotationSpeed *
                Time.fixedDeltaTime
            );

        rb.MoveRotation(
            smoothed
        );
    }
}