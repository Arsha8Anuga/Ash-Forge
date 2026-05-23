using UnityEngine;

[CreateAssetMenu(
    menuName =
    "Items/Item Data"
)]
public class ItemData :
    ScriptableObject
{
    [Header("Identity")]
    public string itemId;

    public string itemName;

    [TextArea]
    public string description;

    [Header("Category")]
    public ItemCategory category;

    [Header("Visual")]
    public Sprite icon;

    public GameObject prefab;

    [Header("Physical")]
    public float mass = 1f;

    [Header("Stack")]
    public int maxStack = 1;

    [Header("Storage")]
    public bool canStore = true;

    public bool canBasket = true;

    [Header("Tags")]
    public ProcessTag[] processTags;
}