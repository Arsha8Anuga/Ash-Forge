using System.Collections.Generic;
using UnityEngine;

public static class ItemQualityCalculator
{
    public static ItemInstanceData CreateOutputInstance(
        List<PhysicalItem> inputItems,
        float productionQuality,
        float defectChance)
    {
        ItemQualityStats mixed =
            MixInputQuality(
                inputItems
            );

        ApplyProductionModifier(
            mixed,
            productionQuality,
            defectChance
        );

        ItemInstanceData instance =
            new ItemInstanceData(
                mixed
            );

        AddOrigins(
            instance,
            inputItems
        );

        return instance;
    }

    static ItemQualityStats MixInputQuality(
        List<PhysicalItem> inputItems)
    {
        if (inputItems == null ||
            inputItems.Count == 0)
        {
            return new ItemQualityStats();
        }

        ItemQualityStats total =
            new ItemQualityStats
            {
                purity = 0f,
                hardness = 0f,
                durability = 0f,
                conductivity = 0f,
                stability = 0f,
                defectResistance = 0f
            };

        float totalWeight = 0f;

        foreach (PhysicalItem item
            in inputItems)
        {
            if (item == null)
                continue;

            if (item.InstanceData == null)
                item.EnsureInstanceData();

            if (item.InstanceData == null ||
                item.InstanceData.Quality == null)
            {
                continue;
            }

            ItemQualityStats quality =
                item.InstanceData.Quality;

            float weight =
                Mathf.Max(
                    1,
                    item.Amount
                );

            total.purity +=
                quality.purity * weight;

            total.hardness +=
                quality.hardness * weight;

            total.durability +=
                quality.durability * weight;

            total.conductivity +=
                quality.conductivity * weight;

            total.stability +=
                quality.stability * weight;

            total.defectResistance +=
                quality.defectResistance * weight;

            totalWeight += weight;
        }

        if (totalWeight <= 0f)
        {
            return new ItemQualityStats();
        }

        total.purity /= totalWeight;
        total.hardness /= totalWeight;
        total.durability /= totalWeight;
        total.conductivity /= totalWeight;
        total.stability /= totalWeight;
        total.defectResistance /= totalWeight;

        total.Clamp();

        return total;
    }

    static void ApplyProductionModifier(
        ItemQualityStats quality,
        float productionQuality,
        float defectChance)
    {
        if (quality == null)
            return;

        productionQuality =
            Mathf.Clamp(
                productionQuality,
                0f,
                100f
            );

        defectChance =
            Mathf.Clamp(
                defectChance,
                0f,
                100f
            );

        float factor =
            productionQuality / 100f;

        quality.purity =
            Mathf.Lerp(
                quality.purity * 0.85f,
                quality.purity * 1.05f,
                factor
            );

        quality.hardness =
            Mathf.Lerp(
                quality.hardness * 0.85f,
                quality.hardness * 1.08f,
                factor
            );

        quality.durability =
            Mathf.Lerp(
                quality.durability * 0.8f,
                quality.durability * 1.08f,
                factor
            );

        quality.stability =
            Mathf.Lerp(
                quality.stability * 0.8f,
                quality.stability * 1.1f,
                factor
            );

        quality.conductivity =
            Mathf.Lerp(
                quality.conductivity * 0.9f,
                quality.conductivity * 1.03f,
                factor
            );

        float roll =
            UnityEngine.Random.Range(
                0f,
                100f
            );

        if (roll < defectChance)
        {
            quality.defectResistance -=
                UnityEngine.Random.Range(
                    5f,
                    20f
                );
        }
        else
        {
            quality.defectResistance +=
                productionQuality * 0.03f;
        }

        quality.Clamp();
    }

    static void AddOrigins(
        ItemInstanceData instance,
        List<PhysicalItem> inputItems)
    {
        if (instance == null ||
            inputItems == null)
        {
            return;
        }

        foreach (PhysicalItem item
            in inputItems)
        {
            if (item == null ||
                item.ItemData == null)
            {
                continue;
            }

            if (item.InstanceData == null)
                item.EnsureInstanceData();

            if (item.InstanceData == null)
                continue;

            ItemOriginRecord record =
                new ItemOriginRecord(
                    item.ItemData,
                    item.Amount,
                    item.InstanceData.Quality
                );

            instance.AddOrigin(record);
        }
    }
}