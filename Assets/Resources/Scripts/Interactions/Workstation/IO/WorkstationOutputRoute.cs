using UnityEngine;

[System.Serializable]
public class WorkstationOutputRoute
{
    public ItemData itemData;

    public WorkstationOutputTarget target =
        WorkstationOutputTarget.SpawnArea;

    public MonoBehaviour storageBehaviour;

    public bool Matches(
        ItemData item)
    {
        if (itemData == null)
            return true;

        return itemData == item;
    }

    public IStackStorageInput GetStorage()
    {
        return storageBehaviour
            as IStackStorageInput;
    }
}