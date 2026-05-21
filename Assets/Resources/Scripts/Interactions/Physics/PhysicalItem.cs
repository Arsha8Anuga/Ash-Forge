using UnityEngine;

public class PhysicalItem :
    MonoBehaviour
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

    public void SetAmount(
        int value)
    {
        amount =
            Mathf.Max(1, value);
    }
}