using UnityEngine;

public class SmelterFuelTank : MonoBehaviour
{
    [SerializeField]
    private float storedBurnSeconds;

    [SerializeField]
    private float maxBurnSeconds = 120f;

    [SerializeField]
    private bool unlimitedCapacity;

    [SerializeField]
    private bool debugLog;

    public float StoredBurnSeconds => storedBurnSeconds;

    public bool HasFuel => storedBurnSeconds > 0.01f;

    public bool CanAdd(float seconds)
    {
        if (seconds <= 0f)
            return false;

        if (unlimitedCapacity)
            return true;

        return storedBurnSeconds + seconds <= maxBurnSeconds;
    }

    public bool TryAdd(float seconds)
    {
        if (!CanAdd(seconds))
            return false;

        storedBurnSeconds += seconds;

        if (!unlimitedCapacity)
        {
            storedBurnSeconds = Mathf.Min(
                storedBurnSeconds,
                maxBurnSeconds
            );
        }

        Log("Fuel added: " + seconds.ToString("0.##") + "s | total: " + storedBurnSeconds.ToString("0.##") + "s");
        return true;
    }

    public bool TryConsume(float seconds)
    {
        if (seconds <= 0f)
            return true;

        if (storedBurnSeconds < seconds)
            return false;

        storedBurnSeconds -= seconds;
        storedBurnSeconds = Mathf.Max(0f, storedBurnSeconds);
        return true;
    }

    public void ClearFuel()
    {
        storedBurnSeconds = 0f;
    }

    void Log(string message)
    {
        if (!debugLog)
            return;

        Debug.Log("[SmelterFuelTank] " + message, this);
    }
}
