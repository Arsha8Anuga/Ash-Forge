using UnityEngine;

[CreateAssetMenu(
    menuName = "Items/Item Data"
)]
public class ItemData :
    ScriptableObject
{
    [Header("Identity")]
    public string itemId;

    public string itemName;

    public ItemCategory category;

    [TextArea]
    public string description;

    [Header("Visual")]
    public Sprite icon;

    public GameObject prefab;

    [Header("Stack")]
    [Min(1)]
    public int maxStack = 1;

    [Header("Physical")]
    [Min(0.01f)]
    public float mass = 1f;

    [Header("Storage")]
    public bool canStore = true;

    public bool canBasket = true;

    [Min(1)]
    public int storageSize = 1;

    [Header("Reverse")]
    [Range(0f, 1f)]
    public float partialReverseReturnRatio = 0.5f;

    [Range(0f, 100f)]
    public float partialReverseQualityLoss = 10f;

    [Header("Processing")]
    public ProcessTag[] processTags;

    [Header("Default Quality Range")]
    public ItemStatRange purity =
        new ItemStatRange();

    public ItemStatRange hardness =
        new ItemStatRange();

    public ItemStatRange durability =
        new ItemStatRange();

    public ItemStatRange conductivity =
        new ItemStatRange();

    public ItemStatRange stability =
        new ItemStatRange();

    public ItemStatRange defectResistance =
        new ItemStatRange();

    public ItemInstanceData CreateInstance()
    {
        ItemQualityStats stats =
            new ItemQualityStats
            {
                purity =
                    purity != null
                    ? purity.Roll()
                    : 50f,

                hardness =
                    hardness != null
                    ? hardness.Roll()
                    : 50f,

                durability =
                    durability != null
                    ? durability.Roll()
                    : 50f,

                conductivity =
                    conductivity != null
                    ? conductivity.Roll()
                    : 50f,

                stability =
                    stability != null
                    ? stability.Roll()
                    : 50f,

                defectResistance =
                    defectResistance != null
                    ? defectResistance.Roll()
                    : 50f
            };

        stats.Clamp();

        return new ItemInstanceData(
            stats
        );
    }

    public bool HasProcessTag(
        ProcessTag tag)
    {
        if (processTags == null)
            return false;

        foreach (ProcessTag processTag
            in processTags)
        {
            if (processTag == tag)
                return true;
        }

        return false;
    }
}