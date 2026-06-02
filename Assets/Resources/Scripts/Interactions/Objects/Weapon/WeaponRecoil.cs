using UnityEngine;

public class WeaponRecoil :
    MonoBehaviour
{
    [Header("Base Recoil")]
    [SerializeField]
    private float baseKickBack = 0.035f;

    [SerializeField]
    private float baseKickUp = 4f;

    [SerializeField]
    private float baseSideKick = 1.2f;

    [Header("Stat Influence")]
    [SerializeField]
    private float damageInfluence = 0.75f;

    [SerializeField]
    private float defectInfluence = 0.65f;

    [SerializeField]
    private float stabilityReduction = 0.8f;

    [SerializeField]
    private float handlingReduction = 0.25f;

    [Header("Limits")]
    [SerializeField]
    private float maxBackOffset = 0.12f;

    [SerializeField]
    private float maxSideOffset = 0.035f;

    [SerializeField]
    private float maxPitchAngle = 18f;

    [SerializeField]
    private float maxYawAngle = 8f;

    [SerializeField]
    private float maxRollAngle = 6f;

    [Header("Recovery")]
    [SerializeField]
    private float positionRecoverySpeed = 14f;

    [SerializeField]
    private float rotationRecoverySpeed = 12f;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    private Vector3 positionOffset;

    private Vector3 rotationEuler;

    public Vector3 PositionOffset =>
        positionOffset;

    public Quaternion RotationOffset =>
        Quaternion.Euler(
            rotationEuler
        );

    void Update()
    {
        Recover();
    }

    public void ApplyRecoil(
        WeaponStatBlock stats)
    {
        float strength =
            CalculateStrength(
                stats
            );

        float side =
            Random.Range(
                -baseSideKick,
                baseSideKick
            ) * strength;

        float roll =
            Random.Range(
                -baseSideKick,
                baseSideKick
            ) * strength * 0.5f;

        positionOffset +=
            new Vector3(
                side * 0.0025f,
                0f,
                -baseKickBack * strength
            );

        rotationEuler +=
            new Vector3(
                -baseKickUp * strength,
                side,
                roll
            );

        ClampOffsets();

        if (debugLog)
        {
            Debug.Log(
                "[WeaponRecoil] Applied. Strength: " +
                strength.ToString("0.00"),
                this
            );
        }
    }

    public void ResetRecoil()
    {
        positionOffset =
            Vector3.zero;

        rotationEuler =
            Vector3.zero;
    }

    float CalculateStrength(
        WeaponStatBlock stats)
    {
        if (stats == null)
            return 1f;

        float damage01 =
            Mathf.Clamp01(
                stats.damage / 100f
            );

        float defect01 =
            Mathf.Clamp01(
                stats.defect / 100f
            );

        float stability01 =
            Mathf.Clamp01(
                stats.stability / 100f
            );

        float handling01 =
            Mathf.Clamp01(
                stats.handling / 100f
            );

        float value =
            1f +
            damage01 * damageInfluence +
            defect01 * defectInfluence -
            stability01 * stabilityReduction -
            handling01 * handlingReduction;

        return Mathf.Clamp(
            value,
            0.25f,
            2.2f
        );
    }

    void Recover()
    {
        float dt =
            Time.deltaTime;

        positionOffset =
            Vector3.Lerp(
                positionOffset,
                Vector3.zero,
                positionRecoverySpeed * dt
            );

        rotationEuler =
            Vector3.Lerp(
                rotationEuler,
                Vector3.zero,
                rotationRecoverySpeed * dt
            );

        if (positionOffset.sqrMagnitude <
            0.000001f)
        {
            positionOffset =
                Vector3.zero;
        }

        if (rotationEuler.sqrMagnitude <
            0.0001f)
        {
            rotationEuler =
                Vector3.zero;
        }
    }

    void ClampOffsets()
    {
        positionOffset.x =
            Mathf.Clamp(
                positionOffset.x,
                -maxSideOffset,
                maxSideOffset
            );

        positionOffset.z =
            Mathf.Clamp(
                positionOffset.z,
                -maxBackOffset,
                0f
            );

        rotationEuler.x =
            Mathf.Clamp(
                rotationEuler.x,
                -maxPitchAngle,
                maxPitchAngle
            );

        rotationEuler.y =
            Mathf.Clamp(
                rotationEuler.y,
                -maxYawAngle,
                maxYawAngle
            );

        rotationEuler.z =
            Mathf.Clamp(
                rotationEuler.z,
                -maxRollAngle,
                maxRollAngle
            );
    }
}