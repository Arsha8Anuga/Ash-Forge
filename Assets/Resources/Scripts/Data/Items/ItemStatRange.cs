using System;
using UnityEngine;

[Serializable]
public class ItemStatRange
{
    [SerializeField]
    private float min = 40f;

    [SerializeField]
    private float max = 70f;

    public float Roll()
    {
        float low =
            Mathf.Min(
                min,
                max
            );

        float high =
            Mathf.Max(
                min,
                max
            );

        return UnityEngine.Random.Range(
            low,
            high
        );
    }
}