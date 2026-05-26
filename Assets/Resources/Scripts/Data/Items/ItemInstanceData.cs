using System;
using UnityEngine;

[Serializable]
public class ItemInstanceData
{
    [SerializeField]
    private ItemQualityStats quality;

    public ItemQualityStats Quality =>
        quality;

    public float OverallQuality =>
        quality != null
        ? quality.AverageQuality()
        : 0f;

    public ItemInstanceData(
        ItemQualityStats quality)
    {
        this.quality =
            quality != null
            ? quality.Clone()
            : new ItemQualityStats();
    }

    public float GetOverallQuality()
    {
        return OverallQuality;
    }
}