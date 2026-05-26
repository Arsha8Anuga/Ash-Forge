using System;
using UnityEngine;

[Serializable]
public class ItemStatRange
{
    [Range(0f, 100f)]
    public float min = 40f;

    [Range(0f, 100f)]
    public float max = 70f;

    public float Roll()
    {
        return UnityEngine.Random.Range(
            min,
            max
        );
    }
}