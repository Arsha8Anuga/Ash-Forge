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
    public int maxStack = 1;

    [Header("Physical")]
    public float mass = 1f;

    [Header("Storage")]
    public bool canStore = true;

    public bool canBasket = true;

    public int storageSize = 1;

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
                purity = purity.Roll(),
                hardness = hardness.Roll(),
                durability = durability.Roll(),
                conductivity = conductivity.Roll(),
                stability = stability.Roll(),
                defectResistance = defectResistance.Roll()
            };

        return new ItemInstanceData(stats);
    }
}