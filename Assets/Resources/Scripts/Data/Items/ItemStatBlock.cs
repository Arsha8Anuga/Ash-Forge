using System;
using UnityEngine;

[Serializable]
public class ItemStatBlock
{
    [Range(0f, 100f)]
    public float quality = 50f;

    [Range(0f, 100f)]
    public float purity = 50f;

    [Range(0f, 100f)]
    public float hardness = 50f;

    [Range(0f, 100f)]
    public float flexibility = 50f;

    [Range(0f, 100f)]
    public float precision = 50f;

    [Range(0f, 100f)]
    public float durability = 50f;

    [Range(0f, 100f)]
    public float defect = 0f;

    public float OverallQuality
    {
        get
        {
            float positive =
                quality * 0.35f +
                purity * 0.2f +
                hardness * 0.1f +
                flexibility * 0.1f +
                precision * 0.15f +
                durability * 0.1f;

            float penalty =
                defect * 0.6f;

            return Mathf.Clamp(
                positive - penalty,
                0f,
                100f
            );
        }
    }

    public ItemStatBlock Clone()
    {
        return new ItemStatBlock
        {
            quality = quality,
            purity = purity,
            hardness = hardness,
            flexibility = flexibility,
            precision = precision,
            durability = durability,
            defect = defect
        };
    }

    public void Clamp()
    {
        quality = ClampValue(quality);
        purity = ClampValue(purity);
        hardness = ClampValue(hardness);
        flexibility = ClampValue(flexibility);
        precision = ClampValue(precision);
        durability = ClampValue(durability);
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