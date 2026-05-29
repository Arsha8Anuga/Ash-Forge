using System;
using UnityEngine;

[Serializable]
public class WorkstationOutput
{
    public ItemData itemData;

    [Min(1)]
    public int amount = 1;
}