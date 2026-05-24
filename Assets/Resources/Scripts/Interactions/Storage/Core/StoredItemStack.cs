using UnityEngine;

[System.Serializable]
public class StoredItemStack
{
    [SerializeField]
    private ItemData itemData;

    [SerializeField]
    private int amount;

    public ItemData ItemData =>
        itemData;

    public int Amount =>
        amount;

    public bool IsValid =>
        itemData != null &&
        amount > 0;

    public StoredItemStack(
        ItemData itemData,
        int amount)
    {
        this.itemData = itemData;

        this.amount =
            Mathf.Max(
                1,
                amount
            );
    }

    public void Add(
        int value)
    {
        if (value <= 0)
            return;

        amount += value;
    }

    public int Remove(
        int value)
    {
        if (value <= 0)
            return 0;

        int removed =
            Mathf.Min(
                amount,
                value
            );

        amount -= removed;

        return removed;
    }

    public bool IsSameItem(
        ItemData other)
    {
        return itemData != null &&
            itemData == other;
    }
}