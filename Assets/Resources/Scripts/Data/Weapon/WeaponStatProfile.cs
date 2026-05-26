using System;
using UnityEngine;

[Serializable]
public class WeaponStatProfile
{
    public float damage;
    public float accuracy;
    public float recoil;
    public float durability;
    public float fireRate;
    public float reloadSpeed;
    public float heatResistance;
    public float jamChance;
    public float weight;

    public WeaponStatProfile Clone()
    {
        return new WeaponStatProfile
        {
            damage = damage,
            accuracy = accuracy,
            recoil = recoil,
            durability = durability,
            fireRate = fireRate,
            reloadSpeed = reloadSpeed,
            heatResistance = heatResistance,
            jamChance = jamChance,
            weight = weight
        };
    }

    public void Add(
        WeaponStatProfile other)
    {
        if (other == null)
            return;

        damage += other.damage;
        accuracy += other.accuracy;
        recoil += other.recoil;
        durability += other.durability;
        fireRate += other.fireRate;
        reloadSpeed += other.reloadSpeed;
        heatResistance += other.heatResistance;
        jamChance += other.jamChance;
        weight += other.weight;
    }
}