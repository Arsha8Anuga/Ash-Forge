using UnityEngine;

public class StorageDebug :
    MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour storageBehaviour;

    private IItemStorage storage;

    void Awake()
    {
        storage =
            storageBehaviour as IItemStorage;

        if (storage == null)
        {
            storage =
                GetComponent<IItemStorage>();
        }
    }

    void OnGUI()
    {
        if (storage == null)
            return;

        GUILayout.Label(
            "Storage Count: " +
            storage.CurrentCount +
            " / " +
            storage.Capacity
        );
    }
}