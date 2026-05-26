using System;
using UnityEngine;

[Serializable]
public class ItemQualityStats
{
    [Range(0f, 100f)]
    public float purity = 50f;

    [Range(0f, 100f)]
    public float hardness = 50f;

    [Range(0f, 100f)]
    public float durability = 50f;

    [Range(0f, 100f)]
    public float conductivity = 50f;

    [Range(0f, 100f)]
    public float stability = 50f;

    [Range(0f, 100f)]
    public float defectResistance = 50f;

    public float AverageQuality()
    {
        return
            (
                purity +
                hardness +
                durability +
                conductivity +
                stability +
                defectResistance
            ) / 6f;
    }

    public ItemQualityStats Clone()
    {
        return new ItemQualityStats
        {
            purity = purity,
            hardness = hardness,
            durability = durability,
            conductivity = conductivity,
            stability = stability,
            defectResistance = defectResistance
        };
    }
}