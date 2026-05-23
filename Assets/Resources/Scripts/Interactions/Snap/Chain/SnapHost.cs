using UnityEngine;

public class SnapHost : MonoBehaviour
{
    [SerializeField]
    private int maxItems = 10;

    [SerializeField]
    private bool includeSelfInCount = true;

    private int currentItems;

    private SnapHost rootHost;

    public SnapHost Root =>
        rootHost != null
        ? rootHost
        : this;

    public int CurrentItems =>
        Root.currentItems;

    public int MaxItems =>
        Root.maxItems;

    public int Remaining =>
        MaxItems - CurrentItems;

    void Awake()
    {
        if (rootHost == null)
            rootHost = this;

        currentItems =
            includeSelfInCount
            ? 1
            : 0;
    }

    public void SetRoot(
        SnapHost root)
    {
        rootHost =
            root != null
            ? root
            : this;
    }

    public bool CanAdd(
        int amount = 1)
    {
        return Root.currentItems + amount
            <= Root.maxItems;
    }

    public bool TryAdd(
        int amount = 1)
    {
        if (!CanAdd(amount))
            return false;

        Root.currentItems += amount;
        return true;
    }

    public void Remove(
        int amount = 1)
    {
        Root.currentItems =
            Mathf.Max(
                0,
                Root.currentItems - amount
            );
    }

    public void SetCurrentItems(
        int value)
    {
        currentItems =
            Mathf.Max(
                0,
                value
            );
    }
}