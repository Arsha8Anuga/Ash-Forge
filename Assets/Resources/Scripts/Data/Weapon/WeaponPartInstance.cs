using System;
using UnityEngine;

[Serializable]
public class WeaponPartInstance
{
    [SerializeField]
    private WeaponPartData partData;

    [SerializeField]
    private ItemInstanceData itemInstance;

    [SerializeField]
    private float productionQuality;

    [SerializeField]
    private float defectLevel;

    public WeaponPartData PartData =>
        partData;

    public ItemInstanceData ItemInstance =>
        itemInstance;

    public WeaponPartRole Role =>
        partData != null
        ? partData.role
        : WeaponPartRole.Other;

    public float ProductionQuality =>
        productionQuality;

    public float DefectLevel =>
        defectLevel;

    public float MaterialQuality =>
        itemInstance != null
        ? itemInstance.OverallQuality
        : 50f;

    public float FinalQuality
    {
        get
        {
            float value =
                MaterialQuality * 0.75f +
                productionQuality * 0.25f -
                defectLevel * 0.5f;

            return Mathf.Clamp(
                value,
                0f,
                100f
            );
        }
    }

    public WeaponPartInstance(
        WeaponPartData partData,
        ItemInstanceData itemInstance,
        float productionQuality,
        float defectLevel)
    {
        this.partData = partData;

        this.itemInstance =
            itemInstance != null
            ? itemInstance.Clone()
            : null;

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

    public WeaponPartInstance Clone()
    {
        return new WeaponPartInstance(
            partData,
            itemInstance,
            productionQuality,
            defectLevel
        );
    }
}