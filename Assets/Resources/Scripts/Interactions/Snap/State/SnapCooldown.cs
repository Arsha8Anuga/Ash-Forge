using UnityEngine;

public class SnapCooldown
{
    private float nextAllowedSnapTime;

    public bool CanSnap =>
        Time.time >= nextAllowedSnapTime;

    public void BlockFor(
        float duration)
    {
        nextAllowedSnapTime =
            Time.time + duration;
    }
}