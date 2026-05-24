using UnityEngine;

public class StorageOutputSpawner :
    MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour storageBehaviour;

    [SerializeField]
    private StorageSpawnArea spawnArea;

    [SerializeField]
    private GrabMode grabMode =
        GrabMode.Physics;

    [SerializeField]
    private bool debugLog;

    private IItemStorage storage;

    void Awake()
    {
        storage =
            storageBehaviour as IItemStorage;

        if (storage == null)
            storage = GetComponentInParent<IItemStorage>();

        if (spawnArea == null)
            spawnArea = GetComponentInChildren<StorageSpawnArea>();
    }

    public bool SpawnOutput()
    {
        return SpawnOutputInternal(null);
    }

    public bool SpawnOutputAndGrab(
        XRHandInteractor hand)
    {
        if (hand == null)
            return false;

        return SpawnOutputInternal(hand);
    }

    bool SpawnOutputInternal(
        XRHandInteractor hand)
    {
        if (storage == null)
            return false;

        if (!storage.TryOutput(
            out StoredItemStack stack))
        {
            return false;
        }

        if (stack == null ||
            !stack.IsValid ||
            stack.ItemData == null)
        {
            return false;
        }

        GameObject prefab =
            stack.ItemData.prefab;

        if (prefab == null)
            return false;

        Vector3 position =
            spawnArea != null
            ? spawnArea.GetSpawnPosition()
            : transform.position;

        Quaternion rotation =
            spawnArea != null
            ? spawnArea.GetSpawnRotation()
            : transform.rotation;

        GameObject obj =
            Instantiate(
                prefab,
                position,
                rotation
            );

        PhysicalItem item =
            obj.GetComponent<PhysicalItem>();

        if (item != null)
        {
            item.SetAmount(
                stack.Amount
            );
        }

        if (hand != null)
        {
            TryGiveToHand(
                hand,
                obj
            );
        }

        Log(
            "Spawned: " +
            stack.ItemData.itemName
        );

        return true;
    }

    bool TryGiveToHand(
        XRHandInteractor hand,
        GameObject obj)
    {
        if (hand.Holding.HeldObject != null)
            return false;

        IGrabbable grabbable =
            obj.GetComponent<IGrabbable>();

        if (grabbable == null)
            return false;

        if (!grabbable
            .GetSupportedGrabMode()
            .HasFlag(grabMode))
        {
            return false;
        }

        bool success =
            grabbable.Grab(
                hand,
                grabMode
            );

        if (!success)
            return false;

        hand.Holding.ForceSetHeld(
            grabbable,
            grabMode
        );

        return true;
    }

    public bool CanSpawnOutput()
    {
        if (storage == null)
            return false;

        return storage.CanOutput();
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[StorageOutputSpawner] " +
            message,
            this
        );
    }
}