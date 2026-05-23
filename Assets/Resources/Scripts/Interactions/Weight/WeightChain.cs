using UnityEngine;

public class WeightChain :
    MonoBehaviour
{
    [SerializeField]
    private bool debugLog;

    public float TotalMass
    {
        get;
        private set;
    }

    void Awake()
    {
        Recalculate();
    }

    public void Recalculate()
    {
        Rigidbody[] bodies =
            GetComponentsInChildren<Rigidbody>();

        TotalMass = 0f;

        foreach (Rigidbody rb in bodies)
        {
            TotalMass += rb.mass;
        }

        if (debugLog)
        {
            Debug.Log(
                "TotalMass: " + TotalMass
            );
        }
    }
}