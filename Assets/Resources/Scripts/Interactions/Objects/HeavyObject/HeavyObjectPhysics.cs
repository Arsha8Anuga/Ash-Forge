using UnityEngine;

public class HeavyObjectPhysics :
    MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField]
    private float force = 65f;

    [SerializeField]
    private float damping = 5f;

    [SerializeField]
    private float maxVelocity = 10f;

    [SerializeField]
    private float deadZone = 0.08f;

    [Header("Weight")]
    [SerializeField]
    private float weightMultiplier = 2f;

    [SerializeField]
    private float oneHandWeightPenalty = 2.5f;

    [SerializeField]
    private float twoHandWeightBonus = 0.65f;

    [SerializeField]
    private float maxHandDistance = 4f;

    [Header("Direction Strength")]
    [SerializeField]
    private float horizontalStrength = 1f;

    [SerializeField]
    private float verticalStrength = 0.85f;

    [SerializeField]
    private float oneHandVerticalStrength = 0.35f;

    void Awake()
    {
        if (rb == null)
        {
            rb =
                GetComponent<Rigidbody>();
        }
    }

    public void Tick(
        XRHandInteractor left,
        XRHandInteractor right)
    {
        if (rb == null)
            return;

        XRHandInteractor activeHand =
            GetActiveHand(left, right);

        if (activeHand == null)
            return;

        bool twoHanded =
            left != null &&
            right != null;

        if (twoHanded &&
            HandsTooFar(left, right))
        {
            ApplyDampingOnly();
            return;
        }

        Vector3 target =
            GetTargetPoint(
                left,
                right,
                twoHanded
            );

        Vector3 delta =
            target - rb.position;

        if (delta.magnitude <= deadZone)
        {
            ApplyDampingOnly();
            return;
        }

        float weight =
            GetEffectiveWeight(twoHanded);

        float vertical =
            twoHanded
            ? verticalStrength
            : oneHandVerticalStrength;

        Vector3 weightedDelta =
            new Vector3(
                delta.x * horizontalStrength,
                delta.y * vertical,
                delta.z * horizontalStrength
            );

        Vector3 acceleration =
            weightedDelta *
            (force / weight);

        Vector3 dampingForce =
            rb.velocity * damping;

        rb.AddForce(
            acceleration - dampingForce,
            ForceMode.Acceleration
        );

        rb.velocity =
            Vector3.ClampMagnitude(
                rb.velocity,
                maxVelocity
            );
    }

    XRHandInteractor GetActiveHand(
        XRHandInteractor left,
        XRHandInteractor right)
    {
        if (left != null)
            return left;

        return right;
    }

    bool HandsTooFar(
        XRHandInteractor left,
        XRHandInteractor right)
    {
        float handDistance =
            Vector3.Distance(
                GetHeavyPoint(left),
                GetHeavyPoint(right)
            );

        return handDistance >
            maxHandDistance;
    }

    Vector3 GetTargetPoint(
        XRHandInteractor left,
        XRHandInteractor right,
        bool twoHanded)
    {
        if (!twoHanded)
        {
            XRHandInteractor hand =
                GetActiveHand(left, right);

            return GetHeavyPoint(hand);
        }

        return
            (
                GetHeavyPoint(left) +
                GetHeavyPoint(right)
            ) * 0.5f;
    }

    float GetEffectiveWeight(
    bool twoHanded)
    {
        float mass =
            rb.mass;

        IWeightProvider provider =
            GetComponent<IWeightProvider>();

        if (provider != null)
        {
            mass +=
                provider.GetAdditionalWeight();
        }

        float weight =
            Mathf.Max(
                1f,
                mass * weightMultiplier
            );

        if (twoHanded)
        {
            weight *=
                twoHandWeightBonus;
        }
        else
        {
            weight *=
                oneHandWeightPenalty;
        }

        return Mathf.Max(
            1f,
            weight
        );
    }

    Vector3 GetHeavyPoint(
        XRHandInteractor hand)
    {
        if (hand.heavyPoint != null)
            return hand.heavyPoint.position;

        return hand.holdPoint.position;
    }

    void ApplyDampingOnly()
    {
        rb.velocity =
            Vector3.Lerp(
                rb.velocity,
                Vector3.zero,
                damping * Time.fixedDeltaTime
            );
    }
}