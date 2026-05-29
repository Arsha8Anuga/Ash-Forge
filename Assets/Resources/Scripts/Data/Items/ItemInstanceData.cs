using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemInstanceData
{
    [SerializeField]
    private ItemQualityStats quality;

    [SerializeField]
    private List<ItemOriginRecord> origins =
        new List<ItemOriginRecord>();

    public ItemQualityStats Quality =>
        quality;

    public IReadOnlyList<ItemOriginRecord> Origins =>
        origins;

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

    public ItemInstanceData Clone()
    {
        ItemInstanceData clone =
            new ItemInstanceData(
                quality
            );

        clone.AddOrigins(
            origins
        );

        return clone;
    }

    public void SetQuality(
        ItemQualityStats newQuality)
    {
        quality =
            newQuality != null
            ? newQuality.Clone()
            : new ItemQualityStats();
    }

    public void AddOrigin(
        ItemOriginRecord origin)
    {
        if (origin == null)
            return;

        origins.Add(
            origin.Clone()
        );
    }

    public void AddOrigins(
        IEnumerable<ItemOriginRecord> records)
    {
        if (records == null)
            return;

        foreach (ItemOriginRecord record
            in records)
        {
            AddOrigin(record);
        }
    }

    public void ClearOrigins()
    {
        origins.Clear();
    }
}