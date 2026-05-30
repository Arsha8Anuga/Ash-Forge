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
    private float defectLevel = 0f;

    private WeaponPartInstance instance;

    public WeaponPartInstance Instance =>
        instance;

    public WeaponPartData PartData =>
        partData;

    void Awake()
    {
        BuildInstance();
    }

    public void SetProductionResult(
        float quality,
        float defect)
    {
        productionQuality =
            Mathf.Clamp(
                quality,
                0f,
                100f
            );

        defectLevel =
            Mathf.Clamp(
                defect,
                0f,
                100f
            );

        BuildInstance();
    }

    public void BuildInstance()
    {
        PhysicalItem item =
            GetComponent<PhysicalItem>();

        if (item == null)
        {
            Debug.LogWarning(
                "[WeaponPartInstanceHolder] Cannot build instance. " +
                "PhysicalItem is missing.",
                this
            );

            instance = null;
            return;
        }

        if (partData == null)
        {
            Debug.LogWarning(
                "[WeaponPartInstanceHolder] Cannot build instance. " +
                "WeaponPartData is missing.",
                this
            );

            instance = null;
            return;
        }

        item.EnsureInstanceData();

        if (item.InstanceData == null)
        {
            Debug.LogWarning(
                "[WeaponPartInstanceHolder] Cannot build instance. " +
                "ItemInstanceData is still null after EnsureInstanceData.",
                this
            );

            instance = null;
            return;
        }

        instance =
            new WeaponPartInstance(
                partData,
                item.InstanceData,
                productionQuality,
                defectLevel
            );
    }
}