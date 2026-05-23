using System.Collections.Generic;
using UnityEngine;

public class SnapSocketTriggerHandler
{
    private readonly SnapSocket socket;
    private readonly SnapSocketValidator validator;

    private readonly Dictionary<SnappableObject, float>
        candidateEnterTimes =
        new Dictionary<SnappableObject, float>();

    private readonly HashSet<SnappableObject>
        blockedUntilExit =
        new HashSet<SnappableObject>();

    public SnapSocketTriggerHandler(
        SnapSocket socket,
        SnapSocketValidator validator)
    {
        this.socket = socket;
        this.validator = validator;
    }

    public void OnEnter(Collider other)
    {
        SnappableObject snap =
            FindSnappable(other);

        if (snap == null)
            return;

        if (validator.ShouldIgnore(snap))
            return;

        if (!validator.IsHeld(snap) &&
            validator.IsMovingTooFast(snap))
        {
            blockedUntilExit.Add(snap);

            socket.Log(
                "Blocked until exit: entered too fast " +
                snap.name
            );

            return;
        }

        RegisterCandidate(snap);
    }

    public void OnStay(Collider other)
    {
        SnappableObject snap =
            FindSnappable(other);

        if (snap == null)
            return;

        if (validator.ShouldIgnore(snap))
            return;

        if (blockedUntilExit.Contains(snap))
            return;

        if (!candidateEnterTimes.ContainsKey(snap))
            RegisterCandidate(snap);

        if (validator.IsHeld(snap))
            return;

        if (validator.IsMovingTooFast(snap))
            return;

        float enterTime =
            candidateEnterTimes[snap];

        if (Time.time - enterTime <
            socket.SnapDwellTime)
        {
            return;
        }

        bool success =
            socket.TrySnap(snap);

        if (!success)
            return;

        Cleanup(snap);

        snap.BlockSnapFor(
            socket.SnapCooldown
        );

        socket.Log(
            "Snap success: " +
            snap.name
        );
    }

    public void OnExit(Collider other)
    {
        SnappableObject snap =
            FindSnappable(other);

        if (snap == null)
            return;

        Cleanup(snap);
    }

    public void Cleanup(
        SnappableObject snap)
    {
        if (snap == null)
            return;

        candidateEnterTimes.Remove(snap);
        blockedUntilExit.Remove(snap);
    }

    SnappableObject FindSnappable(
        Collider other)
    {
        return other.GetComponentInParent
            <SnappableObject>();
    }

    void RegisterCandidate(
        SnappableObject snap)
    {
        if (snap == null)
            return;

        if (candidateEnterTimes
            .ContainsKey(snap))
        {
            return;
        }

        candidateEnterTimes.Add(
            snap,
            Time.time
        );
    }
}