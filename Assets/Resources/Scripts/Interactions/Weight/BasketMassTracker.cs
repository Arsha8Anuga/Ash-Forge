using System.Collections.Generic;
using UnityEngine;

public class BasketMassTracker :
    MonoBehaviour,
    IWeightProvider
{
    [SerializeField]
    private Rigidbody basketBody;

    private readonly HashSet<Rigidbody>
        trackedBodies =
        new HashSet<Rigidbody>();

    public float TotalMass
    {
        get;
        private set;
    }

    void Awake()
    {
        if (basketBody == null)
        {
            basketBody =
                GetComponentInParent<Rigidbody>();
        }

        Recalculate();
    }

    public float GetAdditionalWeight()
    {
        if (basketBody == null)
            return TotalMass;

        return Mathf.Max(
            0f,
            TotalMass - basketBody.mass
        );
    }

    public void AddBody(
        Rigidbody body)
    {
        if (body == null)
            return;

        trackedBodies.Add(body);

        Recalculate();
    }

    public void RemoveBody(
        Rigidbody body)
    {
        if (body == null)
            return;

        trackedBodies.Remove(body);

        Recalculate();
    }

    public void Recalculate()
    {
        TotalMass =
            basketBody != null
            ? basketBody.mass
            : 0f;

        trackedBodies.RemoveWhere(
            body => body == null
        );

        foreach (Rigidbody body
            in trackedBodies)
        {
            TotalMass += body.mass;
        }
    }
}