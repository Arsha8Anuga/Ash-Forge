using System.Collections.Generic;
using UnityEngine;

public static class ItemReverseCalculator
{
    public static List<StoredItemStack> Reverse(
        PhysicalItem item,
        WorkstationReversibility mode)
    {
        List<StoredItemStack> result =
            new List<StoredItemStack>();

        if (item == null ||
            item.InstanceData == null)
        {
            return result;
        }

        if (mode == WorkstationReversibility.None ||
            mode == WorkstationReversibility.Irreversible)
        {
            return result;
        }

        IReadOnlyList<ItemOriginRecord> origins =
            item.InstanceData.Origins;

        if (origins == null ||
            origins.Count == 0)
        {
            return result;
        }

        foreach (ItemOriginRecord origin
            in origins)
        {
            if (origin == null ||
                origin.ItemData == null)
            {
                continue;
            }

            int amount =
                GetReturnAmount(
                    item,
                    origin,
                    mode
                );

            if (amount <= 0)
                continue;

            ItemInstanceData instance =
                BuildReverseInstance(
                    item,
                    origin,
                    mode
                );

            StoredItemStack stack =
                new StoredItemStack(
                    origin.ItemData,
                    amount,
                    instance
                );

            result.Add(stack);
        }

        return result;
    }

    static int GetReturnAmount(
        PhysicalItem item,
        ItemOriginRecord origin,
        WorkstationReversibility mode)
    {
        if (mode == WorkstationReversibility.Full)
        {
            return Mathf.Max(
                1,
                origin.Amount
            );
        }

        if (mode == WorkstationReversibility.Partial)
        {
            float ratio =
                item.ItemData != null
                ? item.ItemData.partialReverseReturnRatio
                : 0.5f;

            int amount =
                Mathf.FloorToInt(
                    origin.Amount * ratio
                );

            return Mathf.Max(
                0,
                amount
            );
        }

        return 0;
    }

    static ItemInstanceData BuildReverseInstance(
        PhysicalItem item,
        ItemOriginRecord origin,
        WorkstationReversibility mode)
    {
        ItemQualityStats quality =
            origin.QualitySnapshot != null
            ? origin.QualitySnapshot.Clone()
            : new ItemQualityStats();

        if (mode == WorkstationReversibility.Partial)
        {
            float loss =
                item.ItemData != null
                ? item.ItemData.partialReverseQualityLoss
                : 10f;

            quality.purity -=
                loss * 0.5f;

            quality.hardness -=
                loss * 0.35f;

            quality.durability -=
                loss;

            quality.stability -=
                loss;

            quality.defectResistance -=
                loss * 1.25f;

            quality.Clamp();
        }

        return new ItemInstanceData(
            quality
        );
    }
}