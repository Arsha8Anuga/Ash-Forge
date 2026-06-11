using UnityEngine;

public class SmelterLidSensor : MonoBehaviour
{
    [Header("Snap Lid")]
    [SerializeField]
    private SnapSocket lidSocket;

    [Tooltip("Optional. Isi kalau hanya lid tertentu yang boleh dianggap tertutup.")]
    [SerializeField]
    private SnappableObject requiredLid;

    [SerializeField]
    private bool debugLog;

    private bool lastClosed;

    public bool IsClosed
    {
        get
        {
            if (lidSocket == null)
                return false;

            if (lidSocket.Current == null)
                return false;

            if (requiredLid == null)
                return true;

            return lidSocket.Current == requiredLid;
        }
    }

    void Awake()
    {
        if (lidSocket == null)
            lidSocket = GetComponentInChildren<SnapSocket>();

        lastClosed = IsClosed;
    }

    void Update()
    {
        if (!debugLog)
            return;

        bool closed = IsClosed;

        if (closed == lastClosed)
            return;

        lastClosed = closed;
        Debug.Log("[SmelterLidSensor] Lid " + (closed ? "closed" : "opened"), this);
    }
}
