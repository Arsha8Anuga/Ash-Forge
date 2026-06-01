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

        if (mode == WorkstationReversibility.Full)
        {
            BuildFullReverse(
                item,
                origins,
                result
            );

            return result;
        }

        if (mode == WorkstationReversibility.Partial)
        {
            BuildPartialReverse(
                item,
                origins,
                result
            );

            return result;
        }

        return result;
    }

    static void BuildFullReverse(
        PhysicalItem item,
        IReadOnlyList<ItemOriginRecord> origins,
        List<StoredItemStack> result)
    {
        foreach (ItemOriginRecord origin
            in origins)
        {
            if (origin == null ||
                origin.ItemData == null)
            {
                continue;
            }

            AddReverseStack(
                item,
                origin,
                origin.Amount,
                WorkstationReversibility.Full,
                result
            );
        }
    }

    static void BuildPartialReverse(
        PhysicalItem item,
        IReadOnlyList<ItemOriginRecord> origins,
        List<StoredItemStack> result)
    {
        int totalUnits =
            CountOriginUnits(
                origins
            );

        if (totalUnits <= 0)
            return;

        float ratio =
            item.ItemData != null
            ? item.ItemData.partialReverseReturnRatio
            : 0.5f;

        ratio =
            Mathf.Clamp01(
                ratio
            );

        int unitsToReturn =
            Mathf.RoundToInt(
                totalUnits * ratio
            );

        unitsToReturn =
            Mathf.Clamp(
                unitsToReturn,
                0,
                totalUnits
            );

        if (unitsToReturn <= 0)
            return;

        int remaining =
            unitsToReturn;

        foreach (ItemOriginRecord origin
            in origins)
        {
            if (remaining <= 0)
                break;

            if (origin == null ||
                origin.ItemData == null)
            {
                continue;
            }

            int amount =
                Mathf.Min(
                    origin.Amount,
                    remaining
                );

            AddReverseStack(
                item,
                origin,
                amount,
                WorkstationReversibility.Partial,
                result
            );

            remaining -= amount;
        }
    }

    static int CountOriginUnits(
        IReadOnlyList<ItemOriginRecord> origins)
    {
        if (origins == null)
            return 0;

        int total = 0;

        foreach (ItemOriginRecord origin
            in origins)
        {
            if (origin == null ||
                origin.ItemData == null)
            {
                continue;
            }

            total +=
                Mathf.Max(
                    0,
                    origin.Amount
                );
        }

        return total;
    }

    static void AddReverseStack(
        PhysicalItem item,
        ItemOriginRecord origin,
        int amount,
        WorkstationReversibility mode,
        List<StoredItemStack> result)
    {
        if (origin == null ||
            origin.ItemData == null ||
            result == null)
        {
            return;
        }

        if (amount <= 0)
            return;

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

        result.Add(
            stack
        );
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