using System.Collections.Generic;
using UnityEngine;

public class QueueItemStorage :
    MonoBehaviour,
    IItemStorage
{
    [SerializeField]
    private int capacity = 20;

    [SerializeField]
    private bool destroyInputObject = true;

    [SerializeField]
    private bool debugLog;

    private readonly Queue<StoredItemStack> queue =
        new Queue<StoredItemStack>();

    public int CurrentCount
    {
        get;
        private set;
    }

    public int Capacity =>
        capacity;

    public bool CanInput(
        PhysicalItem item)
    {
        if (item == null)
            return false;

        if (!item.IsValid)
            return false;

        if (!item.CanStore)
            return false;

        if (CurrentCount + item.Amount >
            capacity)
        {
            return false;
        }

        return true;
    }

    public bool Input(
        PhysicalItem item)
    {
        if (!CanInput(item))
            return false;

        queue.Enqueue(
            new StoredItemStack(
                item.ItemData,
                item.Amount
            )
        );

        CurrentCount += item.Amount;

        if (debugLog)
        {
            Debug.Log(
                "[QueueStorage] Input: " +
                item.ItemName +
                " x" +
                item.Amount,
                this
            );
        }

        if (destroyInputObject)
        {
            ISnappable snap =
                item.GetComponent<ISnappable>();

            if (snap != null &&
                snap.IsSnapped)
            {
                snap.Unsnap();
            }

            Destroy(item.gameObject);
        }

        return true;
    }

    public bool CanOutput()
    {
        return queue.Count > 0 &&
            CurrentCount > 0;
    }

    public bool TryOutput(
        out StoredItemStack stack)
    {
        stack = null;

        if (!CanOutput())
            return false;

        stack =
            queue.Dequeue();

        CurrentCount =
            Mathf.Max(
                0,
                CurrentCount -
                stack.Amount
            );

        if (debugLog)
        {
            Debug.Log(
                "[QueueStorage] Output: " +
                stack.ItemData.itemName +
                " x" +
                stack.Amount,
                this
            );
        }

        return true;
    }
}