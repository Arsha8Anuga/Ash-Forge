using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponInstance
{
    [SerializeField]
    private List<WeaponPartInstance> parts =
        new List<WeaponPartInstance>();

    [SerializeField]
    private WeaponStatBlock stats;

    public IReadOnlyList<WeaponPartInstance> Parts =>
        parts;

    public WeaponStatBlock Stats =>
        stats;

    public float OverallQuality =>
        stats != null
        ? stats.OverallQuality
        : 0f;

    public WeaponInstance(
        List<WeaponPartInstance> parts,
        WeaponStatBlock stats)
    {
        this.parts =
            CloneParts(parts);

        this.stats =
            stats != null
            ? stats.Clone()
            : new WeaponStatBlock();
    }

    public WeaponInstance Clone()
    {
        return new WeaponInstance(
            parts,
            stats
        );
    }

    static List<WeaponPartInstance> CloneParts(
        List<WeaponPartInstance> source)
    {
        List<WeaponPartInstance> result =
            new List<WeaponPartInstance>();

        if (source == null)
            return result;

        foreach (WeaponPartInstance part
            in source)
        {
            if (part == null)
                continue;

            result.Add(
                part.Clone()
            );
        }

        return result;
    }
}