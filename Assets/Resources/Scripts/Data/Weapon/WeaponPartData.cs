using UnityEngine;

[CreateAssetMenu(
    menuName = "Weapon/Weapon Part Data"
)]
public class WeaponPartData :
    ScriptableObject
{
    [Header("Identity")]
    public string partId;

    public string partName;

    public WeaponPartRole role =
        WeaponPartRole.Other;

    [TextArea]
    public string description;

    [Header("Physical Output")]
    public ItemData outputItemData;

    [Header("Base Stat Contribution")]
    public WeaponStatBlock statContribution =
        new WeaponStatBlock();

    [Header("Importance")]
    [Range(0f, 2f)]
    public float qualityInfluence = 1f;

    [Range(0f, 2f)]
    public float defectInfluence = 1f;

    [Header("Assembly Rule")]
    public bool requiredForBasicWeapon = false;
}