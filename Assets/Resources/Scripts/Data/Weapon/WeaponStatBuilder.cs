using System.Collections.Generic;
using UnityEngine;

public static class WeaponStatBuilder
{
    public static WeaponStatProfile Build(
        WeaponStatProfile baseWeaponStats,
        List<WeaponPartInstance> parts)
    {
        WeaponStatProfile result =
            baseWeaponStats != null
            ? baseWeaponStats.Clone()
            : new WeaponStatProfile();

        if (parts == null)
            return result;

        foreach (WeaponPartInstance part
            in parts)
        {
            ApplyPart(
                result,
                part
            );
        }

        ClampStats(result);

        return result;
    }

    static void ApplyPart(
        WeaponStatProfile result,
        WeaponPartInstance part)
    {
        if (result == null ||
            part == null ||
            part.PartData == null ||
            part.MaterialInstance == null)
        {
            return;
        }

        WeaponPartData data =
            part.PartData;

        ItemQualityStats q =
            part.MaterialInstance.Quality;

        if (q == null)
            return;

        float partQuality =
            part.GetPartQuality() / 100f;

        WeaponStatProfile baseStats =
            data.baseStats;

        result.damage +=
            baseStats.damage *
            partQuality *
            (
                q.hardness *
                data.hardnessInfluence
            ) / 100f;

        result.accuracy +=
            baseStats.accuracy *
            partQuality *
            (
                q.stability *
                data.stabilityInfluence
            ) / 100f;

        result.recoil +=
            baseStats.recoil *
            (
                1f -
                q.stability / 100f
            );

        result.durability +=
            baseStats.durability *
            partQuality *
            (
                q.durability *
                data.durabilityInfluence
            ) / 100f;

        result.fireRate +=
            baseStats.fireRate *
            partQuality *
            (
                q.stability / 100f
            );

        result.reloadSpeed +=
            baseStats.reloadSpeed *
            partQuality;

        result.heatResistance +=
            baseStats.heatResistance *
            partQuality *
            (
                q.hardness / 100f
            );

        result.jamChance +=
            baseStats.jamChance;

        result.jamChance -=
            q.defectResistance *
            data.defectResistanceInfluence *
            0.01f;

        result.jamChance +=
            part.DefectLevel *
            0.01f;

        result.weight +=
            baseStats.weight;
    }

    static void ClampStats(
        WeaponStatProfile stats)
    {
        stats.damage =
            Mathf.Max(0f, stats.damage);

        stats.accuracy =
            Mathf.Clamp(
                stats.accuracy,
                0f,
                100f
            );

        stats.recoil =
            Mathf.Max(0f, stats.recoil);

        stats.durability =
            Mathf.Max(0f, stats.durability);

        stats.fireRate =
            Mathf.Max(0f, stats.fireRate);

        stats.reloadSpeed =
            Mathf.Max(0f, stats.reloadSpeed);

        stats.heatResistance =
            Mathf.Max(0f, stats.heatResistance);

        stats.jamChance =
            Mathf.Clamp(
                stats.jamChance,
                0f,
                1f
            );

        stats.weight =
            Mathf.Max(0f, stats.weight);
    }
}