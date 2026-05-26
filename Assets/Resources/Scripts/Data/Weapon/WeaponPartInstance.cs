using System;
using UnityEngine;

[Serializable]
public class WeaponPartInstance
{
    [SerializeField]
    private WeaponPartData partData;

    [SerializeField]
    private ItemInstanceData materialInstance;

    [SerializeField]
    [Range(0f, 100f)]
    private float productionQuality = 80f;

    [SerializeField]
    [Range(0f, 100f)]
    private float defectLevel;

    public WeaponPartData PartData =>
        partData;

    public ItemInstanceData MaterialInstance =>
        materialInstance;

    public float ProductionQuality =>
        productionQuality;

    public float DefectLevel =>
        defectLevel;

    public WeaponPartRole Role =>
        partData != null
        ? partData.role
        : WeaponPartRole.InternalMechanism;

    public WeaponPartInstance(
        WeaponPartData partData,
        ItemInstanceData materialInstance,
        float productionQuality,
        float defectLevel)
    {
        this.partData = partData;
        this.materialInstance = materialInstance;
        this.productionQuality =
            Mathf.Clamp(
                productionQuality,
                0f,
                100f
            );

        this.defectLevel =
            Mathf.Clamp(
                defectLevel,
                0f,
                100f
            );
    }

    public float GetPartQuality()
    {
        if (partData == null ||
            materialInstance == null)
        {
            return 0f;
        }

        float materialQuality =
            materialInstance.OverallQuality;

        float result =
            materialQuality *
            partData.materialWeight +
            productionQuality *
            partData.productionWeight;

        result -= defectLevel;

        return Mathf.Clamp(
            result,
            0f,
            100f
        );
    }
}