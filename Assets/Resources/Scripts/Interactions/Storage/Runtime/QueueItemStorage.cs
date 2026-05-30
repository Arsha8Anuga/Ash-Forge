using System.Collections.Generic;
using UnityEngine;

public class QueueItemStorage :
    MonoBehaviour,
    IItemStorage,
    IStackStorageInput
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

    public int Remaining =>
        Mathf.Max(
            0,
            capacity - CurrentCount
        );

    public bool CanInput(
        PhysicalItem item)
    {
        if (item == null)
            return false;

        if (!item.IsValid)
            return false;

        if (!item.CanStore)
            return false;

        if (item.StorageSize <= 0)
            return false;

        return CurrentCount +
            item.StorageSize <= capacity;
    }

    public bool Input(
        PhysicalItem item)
    {
        if (!CanInput(item))
            return false;

        WeaponInstance weaponInstance = null;

        WeaponInstanceHolder weaponHolder =
            item.GetComponent
            <WeaponInstanceHolder>();

        if (weaponHolder != null)
        {
            weaponInstance =
                weaponHolder.Instance;
        }

        StoredItemStack stack =
            new StoredItemStack(
                item.ItemData,
                item.Amount,
                item.InstanceData,
                weaponInstance
            );

        bool success =
            InputStack(stack);

        if (!success)
            return false;

        if (destroyInputObject)
        {
            ISnappable snap =
                item.GetComponent<ISnappable>();

            if (snap != null &&
                snap.IsSnapped)
            {
                snap.Unsnap();
            }

            Destroy(
                item.gameObject
            );
        }

        return true;
    }

    public bool CanInputStack(
        StoredItemStack stack)
    {
        if (stack == null)
            return false;

        if (!stack.IsValid)
            return false;

        if (stack.ItemData == null)
            return false;

        if (!stack.ItemData.canStore)
            return false;

        if (stack.StorageSize <= 0)
            return false;

        return CurrentCount +
            stack.StorageSize <= capacity;
    }

    public bool InputStack(
        StoredItemStack stack)
    {
        if (!CanInputStack(stack))
            return false;

        StoredItemStack copy =
            stack.Clone();

        queue.Enqueue(copy);

        CurrentCount +=
            copy.StorageSize;

        Log(
            "Input stack: " +
            copy.ItemData.itemName +
            " x" +
            copy.Amount +
            " | Used: " +
            CurrentCount +
            "/" +
            capacity
        );

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
                stack.StorageSize
            );

        Log(
            "Output: " +
            stack.ItemData.itemName +
            " x" +
            stack.Amount +
            " | Used: " +
            CurrentCount +
            "/" +
            capacity
        );

        return true;
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[QueueStorage] " +
            message,
            this
        );
    }
}