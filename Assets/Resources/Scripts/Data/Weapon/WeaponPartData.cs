using UnityEngine;

[CreateAssetMenu(
    menuName = "Weapons/Weapon Part Data"
)]
public class WeaponPartData :
    ScriptableObject
{
    [Header("Identity")]
    public string partId;

    public string partName;

    public WeaponPartRole role;

    [Header("Base Stats")]
    public WeaponStatProfile baseStats =
        new WeaponStatProfile();

    [Header("Quality Influence")]
    [Range(0f, 2f)]
    public float purityInfluence = 0.5f;

    [Range(0f, 2f)]
    public float hardnessInfluence = 1f;

    [Range(0f, 2f)]
    public float durabilityInfluence = 1f;

    [Range(0f, 2f)]
    public float conductivityInfluence = 0.3f;

    [Range(0f, 2f)]
    public float stabilityInfluence = 1f;

    [Range(0f, 2f)]
    public float defectResistanceInfluence = 1f;

    [Header("Production")]
    [Range(0f, 1f)]
    public float materialWeight = 0.8f;

    [Range(0f, 1f)]
    public float productionWeight = 0.2f;
}