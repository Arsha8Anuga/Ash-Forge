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

    [SerializeField]
    private WeaponInstance weaponInstance;

    public ItemData ItemData =>
        itemData;

    public int Amount =>
        amount;

    public ItemInstanceData InstanceData =>
        instanceData;

    public WeaponInstance WeaponInstance =>
        weaponInstance;

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

        weaponInstance = null;
    }

    public StoredItemStack(
        ItemData itemData,
        int amount,
        ItemInstanceData instanceData,
        WeaponInstance weaponInstance)
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

        this.weaponInstance =
            weaponInstance != null
            ? weaponInstance.Clone()
            : null;
    }

    public StoredItemStack Clone()
    {
        return new StoredItemStack(
            itemData,
            amount,
            instanceData,
            weaponInstance
        );
    }

    public StoredItemStack WithAmount(
        int newAmount)
    {
        return new StoredItemStack(
            itemData,
            newAmount,
            instanceData,
            weaponInstance
        );
    }
}