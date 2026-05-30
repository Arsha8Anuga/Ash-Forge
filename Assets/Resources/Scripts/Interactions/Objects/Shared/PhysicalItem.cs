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
        EnsureInstanceData();

        ApplyMass();
    }

    public void Initialize(
        ItemData data,
        int amount,
        ItemInstanceData instance)
    {
        itemData = data;

        this.amount =
            Mathf.Max(
                1,
                amount
            );

        if (instance != null)
        {
            instanceData =
                instance.Clone();
        }
        else
        {
            instanceData =
                itemData != null
                ? itemData.CreateInstance()
                : null;
        }

        ApplyMass();
    }

    public void SetAmount(
        int value)
    {
        amount =
            Mathf.Max(
                1,
                value
            );

        ApplyMass();
    }
    public bool CanReduceAmount(
        int value)
    {
        if (value <= 0)
            return false;

        return amount - value >= 1;
    }

    public void AddAmount(
        int value)
    {
        if (value <= 0)
            return;

        amount += value;

        ApplyMass();
    }

    public void SetItemData(
        ItemData data)
    {
        itemData = data;

        instanceData =
            itemData != null
            ? itemData.CreateInstance()
            : null;

        ApplyMass();
    }

    public void SetInstanceData(
        ItemInstanceData data)
    {
        instanceData =
            data != null
            ? data.Clone()
            : null;
    }

    public void EnsureInstanceData()
    {
        if (instanceData != null)
            return;

        if (itemData == null)
            return;

        instanceData =
            itemData.CreateInstance();
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
            Mathf.Max(
                0.01f,
                itemData.mass * amount
            );
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