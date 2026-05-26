using UnityEngine;

public class PhysicalItem :
    MonoBehaviour
{
    [SerializeField]
    private ItemData itemData;

    [SerializeField]
    private int amount = 1;

    [SerializeField]
    private ItemInstanceData instanceData;

    public ItemData ItemData =>
        itemData;

    public ItemInstanceData InstanceData =>
        instanceData;

    public ItemQualityStats Quality =>
        instanceData != null
        ? instanceData.Quality
        : null;

    public int Amount =>
        amount;

    public string ItemName =>
        itemData != null
        ? itemData.itemName
        : name;
    
    public int StorageSize =>
    itemData != null
    ? itemData.storageSize * amount
    : 0;

    public bool IsValid =>
        itemData != null;

    public bool CanStore =>
        itemData != null &&
        itemData.canStore;

    public bool CanBasket =>
        itemData != null &&
        itemData.canBasket;

    void Awake()
    {
        if (itemData != null &&
            instanceData == null)
        {
            instanceData =
                itemData.CreateInstance();
        }

        ApplyMass();
    }

    public void SetAmount(
        int value)
    {
        amount =
            Mathf.Max(1, value);
    }

    public void SetItemData(
        ItemData data)
    {
        itemData = data;

        if (itemData != null)
        {
            instanceData =
                itemData.CreateInstance();
        }

        ApplyMass();
    }

    public void SetInstanceData(
        ItemInstanceData data)
    {
        instanceData = data;
    }

    void ApplyMass()
    {
        if (itemData == null)
            return;

        Rigidbody rb =
            GetComponent<Rigidbody>();

        if (rb == null)
            return;

        rb.mass =
            itemData.mass *
            amount;
    }

    public bool IsSameItem(
        PhysicalItem other)
    {
        if (other == null)
            return false;

        return itemData != null &&
            itemData == other.ItemData;
    }
}