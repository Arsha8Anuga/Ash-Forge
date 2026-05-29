using UnityEngine;

public class WorkstationToolActionZone :
    MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour receiverBehaviour;

    [SerializeField]
    private bool sendZoneOnlyHit = true;

    [SerializeField]
    private bool debugLog;

    private IWorkstationToolReceiver receiver;

    void Awake()
    {
        receiver =
            receiverBehaviour as IWorkstationToolReceiver;
    }

    void OnTriggerEnter(
        Collider other)
    {
        TrySendZoneOnlyHit(other);
    }

    void OnTriggerStay(
        Collider other)
    {
        TrySendZoneOnlyHit(other);
    }

    public void ReceiveToolHit(
        WorkstationToolHit hit)
    {
        if (receiver == null)
        {
            Log("No receiver.");
            return;
        }

        if (hit.tool == null)
        {
            Log("Rejected: tool null.");
            return;
        }

        Log(
            "Hit received | Tool: " +
            hit.tool.ToolType +
            " | Item: " +
            (
                hit.item != null
                ? hit.item.ItemData.itemName
                : "none"
            )
        );

        receiver.ReceiveToolHit(hit);
    }

    void TrySendZoneOnlyHit(
        Collider other)
    {
        if (!sendZoneOnlyHit)
            return;

        WorkstationToolTip tip =
            other.GetComponent<WorkstationToolTip>();

        if (tip == null)
        {
            tip =
                other.GetComponentInParent
                <WorkstationToolTip>();
        }

        if (tip == null)
            return;

        WorkstationTool tool =
            tip.Tool;

        if (tool == null)
            return;

        WorkstationToolHit hit =
            new WorkstationToolHit
            {
                tool = tool,
                item = null,
                hitItem = false,
                position = tip.transform.position,
                velocity = tool.CurrentVelocity
            };

        ReceiveToolHit(hit);
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[WorkstationToolActionZone] " +
            message,
            this
        );
    }
}