using UnityEngine;

[CreateAssetMenu(
    menuName = "Workstation/Refinement Profile"
)]
public class RefinementProfileData : ScriptableObject
{
    [Header("Identity")]
    public string profileName = "Balanced Refinement";

    [TextArea]
    public string description;

    [Header("Item Quality Delta")]
    public float purityDelta = -2f;

    public float hardnessDelta = 8f;

    public float durabilityDelta = 4f;

    public float conductivityDelta = -2f;

    public float stabilityDelta = -4f;

    public float defectResistanceDelta = -5f;

    [Header("Weapon Part Tradeoff")]
    public float productionQualityDelta = 12f;

    public float defectDelta = 3f;

    public ItemQualityStats ApplyTo(
        ItemQualityStats source)
    {
        ItemQualityStats result =
            source != null
            ? source.Clone()
            : new ItemQualityStats();

        result.purity += purityDelta;
        result.hardness += hardnessDelta;
        result.durability += durabilityDelta;
        result.conductivity += conductivityDelta;
        result.stability += stabilityDelta;
        result.defectResistance += defectResistanceDelta;

        result.Clamp();

        return result;
    }

    public float ApplyProductionQuality(
        float source)
    {
        return Mathf.Clamp(
            source + productionQualityDelta,
            0f,
            100f
        );
    }

    public float ApplyDefect(
        float source)
    {
        return Mathf.Clamp(
            source + defectDelta,
            0f,
            100f
        );
    }
}
