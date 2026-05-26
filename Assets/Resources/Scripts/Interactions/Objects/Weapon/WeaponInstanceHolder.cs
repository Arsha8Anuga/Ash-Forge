using UnityEngine;

public class WeaponPartInstanceHolder :
    MonoBehaviour
{
    [SerializeField]
    private WeaponPartData partData;

    [SerializeField]
    [Range(0f, 100f)]
    private float productionQuality = 80f;

    [SerializeField]
    [Range(0f, 100f)]
    private float defectLevel;

    private WeaponPartInstance instance;

    public WeaponPartInstance Instance =>
        instance;

    public WeaponPartData PartData =>
        partData;

    public WeaponPartRole Role =>
        partData != null
        ? partData.role
        : WeaponPartRole.InternalMechanism;

    void Awake()
    {
        BuildInstance();
    }

    public void BuildInstance()
    {
        PhysicalItem item =
            GetComponent<PhysicalItem>();

        if (item == null)
            return;

        if (item.InstanceData == null)
            return;

        if (partData == null)
            return;

        instance =
            new WeaponPartInstance(
                partData,
                item.InstanceData,
                productionQuality,
                defectLevel
            );
    }

    public void Rebuild()
    {
        BuildInstance();
    }

    public void SetProductionResult(
        float productionQuality,
        float defectLevel)
    {
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

        BuildInstance();
    }
}