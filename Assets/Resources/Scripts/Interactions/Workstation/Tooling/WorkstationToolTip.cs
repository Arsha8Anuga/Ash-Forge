using UnityEngine;

public class WorkstationToolTip :
    MonoBehaviour
{
    [SerializeField]
    private WorkstationTool tool;

    [SerializeField]
    private bool debugLog;

    private WorkstationToolActionZone currentZone;

    private PhysicalItem currentItem;

    public WorkstationTool Tool =>
        tool;

    void Awake()
    {
        if (tool == null)
        {
            tool =
                GetComponentInParent
                <WorkstationTool>();
        }
    }

    void OnTriggerEnter(
        Collider other)
    {
        RegisterContact(other);

        TrySendItemHit();
    }

    void OnTriggerStay(
        Collider other)
    {
        RegisterContact(other);

        TrySendItemHit();
    }

    void OnTriggerExit(
        Collider other)
    {
        UnregisterContact(other);
    }

    void RegisterContact(
        Collider other)
    {
        WorkstationToolActionZone zone =
            other.GetComponentInParent
            <WorkstationToolActionZone>();

        if (zone != null)
        {
            currentZone = zone;

            Log(
                "Entered zone: " +
                zone.name
            );
        }

        PhysicalItem item =
            other.GetComponentInParent
            <PhysicalItem>();

        if (item != null)
        {
            currentItem = item;

            Log(
                "Touching item: " +
                item.ItemData.itemName
            );
        }
    }

    void UnregisterContact(
        Collider other)
    {
        WorkstationToolActionZone zone =
            other.GetComponentInParent
            <WorkstationToolActionZone>();

        if (zone != null &&
            currentZone == zone)
        {
            currentZone = null;

            Log("Exited zone.");
        }

        PhysicalItem item =
            other.GetComponentInParent
            <PhysicalItem>();

        if (item != null &&
            currentItem == item)
        {
            currentItem = null;

            Log("Stopped touching item.");
        }
    }

    void TrySendItemHit()
    {
        if (tool == null)
            return;

        if (currentZone == null)
            return;

        if (currentItem == null)
            return;

        WorkstationToolHit hit =
            new WorkstationToolHit
            {
                tool = tool,
                item = currentItem,
                hitItem = true,
                position = transform.position,
                velocity = tool.CurrentVelocity
            };

        currentZone.ReceiveToolHit(hit);
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[WorkstationToolTip] " +
            message,
            this
        );
    }
}