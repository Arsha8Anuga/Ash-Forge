using System;
using UnityEngine;

[Serializable]
public class WeaponStatProfile
{
    [Range(0f, 100f)]
    public float damage;

    [Range(0f, 100f)]
    public float accuracy;

    [Range(0f, 100f)]
    public float stability;

    [Range(0f, 100f)]
    public float durability;

    [Range(0f, 100f)]
    public float recoilControl;

    [Range(0f, 100f)]
    public float fireRate;

    [Range(0f, 100f)]
    public float reliability;

    public void Clamp()
    {
        damage = ClampValue(damage);
        accuracy = ClampValue(accuracy);
        stability = ClampValue(stability);
        durability = ClampValue(durability);
        recoilControl = ClampValue(recoilControl);
        fireRate = ClampValue(fireRate);
        reliability = ClampValue(reliability);
    }

    public float Overall()
    {
        return Mathf.Clamp(
            damage * 0.15f +
            accuracy * 0.18f +
            stability * 0.15f +
            durability * 0.17f +
            recoilControl * 0.12f +
            fireRate * 0.08f +
            reliability * 0.15f,
            0f,
            100f
        );
    }

    public WeaponStatProfile Clone()
    {
        return new WeaponStatProfile
        {
            damage = damage,
            accuracy = accuracy,
            stability = stability,
            durability = durability,
            recoilControl = recoilControl,
            fireRate = fireRate,
            reliability = reliability
        };
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