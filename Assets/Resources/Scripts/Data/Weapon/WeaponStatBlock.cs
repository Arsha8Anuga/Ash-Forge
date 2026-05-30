using System;
using UnityEngine;

[Serializable]
public class WeaponStatBlock
{
    [Range(0f, 100f)]
    public float damage = 50f;

    [Range(0f, 100f)]
    public float accuracy = 50f;

    [Range(0f, 100f)]
    public float stability = 50f;

    [Range(0f, 100f)]
    public float durability = 50f;

    [Range(0f, 100f)]
    public float fireRate = 50f;

    [Range(0f, 100f)]
    public float handling = 50f;

    [Range(0f, 100f)]
    public float reliability = 50f;

    [Range(0f, 100f)]
    public float defect = 0f;

    public float OverallQuality
    {
        get
        {
            float positive =
                damage * 0.15f +
                accuracy * 0.18f +
                stability * 0.14f +
                durability * 0.16f +
                fireRate * 0.10f +
                handling * 0.12f +
                reliability * 0.15f;

            float penalty =
                defect * 0.65f;

            return Mathf.Clamp(
                positive - penalty,
                0f,
                100f
            );
        }
    }

    public WeaponStatBlock Clone()
    {
        return new WeaponStatBlock
        {
            damage = damage,
            accuracy = accuracy,
            stability = stability,
            durability = durability,
            fireRate = fireRate,
            handling = handling,
            reliability = reliability,
            defect = defect
        };
    }

    public void Clamp()
    {
        damage = ClampValue(damage);
        accuracy = ClampValue(accuracy);
        stability = ClampValue(stability);
        durability = ClampValue(durability);
        fireRate = ClampValue(fireRate);
        handling = ClampValue(handling);
        reliability = ClampValue(reliability);
        defect = ClampValue(defect);
    }

    static float ClampValue(
        float value)
    {
        return Mathf.Clamp(
            value,
            0f,
            100f
        );
    }
}