using UnityEngine;

[CreateAssetMenu(menuName = "Smelter/Fuel Data")]
public class SmelterFuelData : ScriptableObject
{
    [Header("Fuel Item")]
    public ItemData itemData;

    [Header("Burn")]
    [Min(0.1f)]
    public float burnSecondsPerItem = 8f;
}
