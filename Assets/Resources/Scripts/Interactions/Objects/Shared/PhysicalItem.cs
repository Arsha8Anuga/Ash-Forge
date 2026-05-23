using UnityEngine;

public class PhysicalItem : MonoBehaviour
{
    [SerializeField]
    private ItemData itemData;

    [SerializeField]
    private int amount = 1;

    public ItemData ItemData =>
        itemData;

    public int Amount =>
        amount;

    public bool IsValid =>
        itemData != null;

    public bool CanStore =>
        itemData != null &&
        itemData.canStore;

    public bool CanBasket =>
        itemData != null &&
        itemData.canBasket;

    public string ItemName =>
        itemData != null
        ? itemData.itemName
        : gameObject.name;

    void Awake()
    {
        ApplyPhysicalData();
    }

    public void SetAmount(
        int value)
    {
        amount =
            Mathf.Max(
                1,
                value
            );
    }

    public void AddAmount(
        int value)
    {
        if (value <= 0)
            return;

        amount += value;
    }

    public int RemoveAmount(
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

        if (amount <= 0)
        {
            Destroy(gameObject);
        }

        return removed;
    }

    public bool IsSameItem(
        PhysicalItem other)
    {
        if (other == null)
            return false;

        return itemData != null &&
            itemData == other.itemData;
    }

    public bool HasProcessTag(
        ProcessTag tag)
    {
        if (itemData == null ||
            itemData.processTags == null)
        {
            return false;
        }

        foreach (ProcessTag processTag
            in itemData.processTags)
        {
            if (processTag == tag)
                return true;
        }

        return false;
    }

    void ApplyPhysicalData()
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
                itemData.mass
            );
    }
}