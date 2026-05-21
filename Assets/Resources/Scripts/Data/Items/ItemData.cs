using UnityEngine;

[CreateAssetMenu(
    menuName =
    "Items/Item Data"
)]
public class ItemData :
    ScriptableObject
{
    [Header("Info")]
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

    [Header("Processing")]
    public ProcessTag[] processTags;
}