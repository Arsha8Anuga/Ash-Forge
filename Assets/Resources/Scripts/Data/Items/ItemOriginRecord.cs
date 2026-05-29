using System;
using UnityEngine;

[Serializable]
public class ItemOriginRecord
{
    [SerializeField]
    private ItemData itemData;

    [SerializeField]
    private int amount = 1;

    [SerializeField]
    private ItemQualityStats qualitySnapshot;

    public ItemData ItemData =>
        itemData;

    public int Amount =>
        amount;

    public ItemQualityStats QualitySnapshot =>
        qualitySnapshot;

    public ItemOriginRecord(
        ItemData itemData,
        int amount,
        ItemQualityStats qualitySnapshot)
    {
        this.itemData = itemData;

        this.amount =
            Mathf.Max(
                1,
                amount
            );

        this.qualitySnapshot =
            qualitySnapshot != null
            ? qualitySnapshot.Clone()
            : new ItemQualityStats();
    }

    public ItemOriginRecord Clone()
    {
        return new ItemOriginRecord(
            itemData,
            amount,
            qualitySnapshot
        );
    }
}