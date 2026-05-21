using UnityEngine;

public class SnapHost : MonoBehaviour
{
    [SerializeField]
    private int maxItems = 10;

    private int currentItems;

    public int CurrentItems =>
        currentItems;

    public int MaxItems =>
        maxItems;

    public int Remaining =>
        maxItems - currentItems;

    public bool CanAdd(
        int amount = 1)
    {
        return currentItems + amount
            <= maxItems;
    }

    public bool TryAdd(
        int amount = 1)
    {
        if (!CanAdd(amount))
            return false;

        currentItems += amount;
        return true;
    }

    public void Remove(
        int amount = 1)
    {
        currentItems =
            Mathf.Max(
                0,
                currentItems - amount
            );
    }
}