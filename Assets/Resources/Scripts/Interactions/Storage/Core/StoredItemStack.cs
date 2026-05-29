using UnityEngine;

[System.Serializable]
public class StoredItemStack
{
    [SerializeField]
    private ItemData itemData;

    [SerializeField]
    private int amount;

    [SerializeField]
    private ItemInstanceData instanceData;

    public ItemData ItemData =>
        itemData;

    public int Amount =>
        amount;

    public ItemInstanceData InstanceData =>
        instanceData;

    public bool IsValid =>
        itemData != null &&
        amount > 0;

    public int StorageSize =>
        itemData != null
        ? itemData.storageSize * amount
        : 0;

    public StoredItemStack(
        ItemData itemData,
        int amount,
        ItemInstanceData instanceData)
    {
        this.itemData = itemData;

        this.amount =
            Mathf.Max(
                1,
                amount
            );

        this.instanceData =
            instanceData != null
            ? instanceData.Clone()
            : null;
    }

    public StoredItemStack Clone()
    {
        return new StoredItemStack(
            itemData,
            amount,
            instanceData
        );
    }

    public StoredItemStack WithAmount(
        int newAmount)
    {
        return new StoredItemStack(
            itemData,
            newAmount,
            instanceData
        );
    }
}