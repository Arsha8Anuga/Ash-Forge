using System;
using UnityEngine;

[Serializable]
public class WorkstationToolStep
{
    [Header("Tool")]
    public WorkstationToolType toolType;

    [Header("Contact")]
    public WorkstationToolContactMode contactMode =
        WorkstationToolContactMode.ZoneOnly;

    [Header("Hold")]
    public bool requiresHold;

    public float holdDuration = 1f;

    [Header("Velocity")]
    public bool useVelocityCheck;

    public float minVelocity = 0.5f;

    public float maxVelocity = 15f;

    [Header("Timing")]
    public float stepCooldown = 0.15f;
}