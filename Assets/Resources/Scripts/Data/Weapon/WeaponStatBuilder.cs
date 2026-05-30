using System.Collections.Generic;
using UnityEngine;

public static class WeaponStatBuilder
{
    public static WeaponStatBlock BuildStats(
        List<WeaponPartInstance> parts)
    {
        WeaponStatBlock result =
            new WeaponStatBlock();

        if (parts == null ||
            parts.Count == 0)
        {
            result.Clamp();
            return result;
        }

        float totalWeight = 0f;

        WeaponStatBlock sum =
            new WeaponStatBlock();

        foreach (WeaponPartInstance part
            in parts)
        {
            if (part == null ||
                part.PartData == null)
            {
                continue;
            }

            WeaponStatBlock contribution =
                part.PartData.statContribution;

            if (contribution == null)
                continue;

            float qualityFactor =
                part.FinalQuality / 100f;

            float influence =
                Mathf.Max(
                    0.01f,
                    part.PartData.qualityInfluence
                );

            float weight =
                qualityFactor * influence;

            AddWeighted(
                sum,
                contribution,
                weight
            );

            sum.defect +=
                part.DefectLevel *
                part.PartData.defectInfluence;

            totalWeight += weight;
        }

        if (totalWeight <= 0f)
        {
            result.Clamp();
            return result;
        }

        result.damage =
            sum.damage / totalWeight;

        result.accuracy =
            sum.accuracy / totalWeight;

        result.stability =
            sum.stability / totalWeight;

        result.durability =
            sum.durability / totalWeight;

        result.fireRate =
            sum.fireRate / totalWeight;

        result.handling =
            sum.handling / totalWeight;

        result.reliability =
            sum.reliability / totalWeight;

        result.defect =
            sum.defect / parts.Count;

        ApplyRoleBonus(
            result,
            parts
        );

        result.Clamp();

        return result;
    }

    static void AddWeighted(
        WeaponStatBlock target,
        WeaponStatBlock source,
        float weight)
    {
        target.damage +=
            source.damage * weight;

        target.accuracy +=
            source.accuracy * weight;

        target.stability +=
            source.stability * weight;

        target.durability +=
            source.durability * weight;

        target.fireRate +=
            source.fireRate * weight;

        target.handling +=
            source.handling * weight;

        target.reliability +=
            source.reliability * weight;
    }

    static void ApplyRoleBonus(
        WeaponStatBlock stats,
        List<WeaponPartInstance> parts)
    {
        if (HasRole(parts, WeaponPartRole.Barrel))
            stats.accuracy += 5f;

        if (HasRole(parts, WeaponPartRole.Stock))
            stats.stability += 5f;

        if (HasRole(parts, WeaponPartRole.TriggerGroup))
            stats.fireRate += 3f;

        if (HasRole(parts, WeaponPartRole.Receiver))
            stats.reliability += 5f;
    }

    static bool HasRole(
        List<WeaponPartInstance> parts,
        WeaponPartRole role)
    {
        foreach (WeaponPartInstance part
            in parts)
        {
            if (part == null)
                continue;

            if (part.Role == role)
                return true;
        }

        return false;
    }
}