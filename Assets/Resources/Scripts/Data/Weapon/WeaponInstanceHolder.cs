using System.Collections.Generic;
using UnityEngine;

public class WeaponInstanceHolder :
    MonoBehaviour
{
    [SerializeField]
    private WeaponInstance instance;

    public WeaponInstance Instance =>
        instance;

    public WeaponStatBlock Stats =>
        instance != null
        ? instance.Stats
        : null;

    public void BuildFromParts(
        List<WeaponPartInstance> parts)
    {
        WeaponStatBlock stats =
            WeaponStatBuilder.BuildStats(
                parts
            );

        instance =
            new WeaponInstance(
                parts,
                stats
            );

        Debug.Log(
            "[WeaponInstanceHolder] Built weapon. Quality: " +
            instance.OverallQuality,
            this
        );
    }

    public void SetInstance(
        WeaponInstance value)
    {
        instance =
            value != null
            ? value.Clone()
            : null;
    }
}