using UnityEngine;

public class SmithingAnvil : MonoBehaviour
{
    [SerializeField]
    private WorkstationToolActionZone actionZone;

    [SerializeField]
    private AreaItemScanner itemScanner;

    [SerializeField]
    private bool debugLog;

    public WorkstationToolActionZone ActionZone =>
        actionZone;

    public AreaItemScanner ItemScanner =>
        itemScanner;

    public bool IsReady =>
        actionZone != null;

    void Awake()
    {
        ResolveReferences();
    }

    void ResolveReferences()
    {
        if (actionZone == null)
        {
            actionZone =
                GetComponentInChildren
                <WorkstationToolActionZone>();
        }

        if (itemScanner == null)
        {
            itemScanner =
                GetComponentInChildren
                <AreaItemScanner>();
        }

        Log(
            "Anvil ready: " +
            IsReady
        );
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[SmithingAnvil] " +
            message,
            this
        );
    }
}
