using UnityEngine;

public class InfiniteSourceStorage :
    MonoBehaviour,
    IItemStorage
{
    [SerializeField]
    private ItemData outputItem;

    [SerializeField]
    private int outputAmount = 1;

    [SerializeField]
    private bool debugLog;

    public int CurrentCount =>
        -1;

    public int Capacity =>
        -1;

    public bool CanInput(
        PhysicalItem item)
    {
        return false;
    }

    public bool Input(
        PhysicalItem item)
    {
        return false;
    }

    public bool CanOutput()
    {
        return outputItem != null &&
            outputAmount > 0;
    }

    public bool TryOutput(
        out StoredItemStack stack)
    {
        stack = null;

        if (!CanOutput())
            return false;

        stack =
            new StoredItemStack(
                outputItem,
                outputAmount
            );

        if (debugLog)
        {
            Debug.Log(
                "[InfiniteStorage] Output: " +
                outputItem.itemName +
                " x" +
                outputAmount,
                this
            );
        }

        return true;
    }
}