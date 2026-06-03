using UnityEngine;

public class TargetHitReceiver :
    MonoBehaviour,
    IWeaponHitReceiver
{
    [Header("Health")]
    [SerializeField]
    private float maxHealth = 100f;

    [SerializeField]
    private bool destroyOnDeath;

    [SerializeField]
    private bool resetOnEnable = true;

    [Header("Hit Marker")]
    [SerializeField]
    private GameObject hitMarkerPrefab;

    [SerializeField]
    private Transform hitMarkerParent;

    [SerializeField]
    private float hitMarkerLifetime = 0.35f;

    [SerializeField]
    private float hitMarkerSurfaceOffset = 0.01f;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog = true;

    private float currentHealth;

    public float CurrentHealth =>
        currentHealth;

    public float MaxHealth =>
        maxHealth;

    void Awake()
    {
        currentHealth =
            Mathf.Max(
                1f,
                maxHealth
            );
    }

    void OnEnable()
    {
        if (!resetOnEnable)
            return;

        currentHealth =
            Mathf.Max(
                1f,
                maxHealth
            );
    }

    public void ReceiveWeaponHit(
        WeaponHitInfo hitInfo)
    {
        float damage =
            Mathf.Max(
                0f,
                hitInfo.damage
            );

        currentHealth =
            Mathf.Max(
                0f,
                currentHealth - damage
            );

        SpawnHitMarker(
            hitInfo
        );

        Log(
            "Hit for " +
            damage.ToString("0.0") +
            " | HP: " +
            currentHealth.ToString("0.0") +
            "/" +
            maxHealth.ToString("0.0")
        );

        if (currentHealth <= 0f &&
            destroyOnDeath)
        {
            Destroy(
                gameObject
            );
        }
    }

    void SpawnHitMarker(
        WeaponHitInfo hitInfo)
    {
        if (hitMarkerPrefab == null)
            return;

        Vector3 normal =
            hitInfo.normal.sqrMagnitude > 0.001f
            ? hitInfo.normal.normalized
            : Vector3.up;

        Vector3 position =
            hitInfo.point +
            normal *
            hitMarkerSurfaceOffset;

        Quaternion rotation =
            Quaternion.LookRotation(
                normal
            );

        GameObject marker =
            Instantiate(
                hitMarkerPrefab,
                position,
                rotation,
                hitMarkerParent
            );

        if (hitMarkerLifetime > 0f)
        {
            Destroy(
                marker,
                hitMarkerLifetime
            );
        }
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[TargetHitReceiver] " +
            message,
            this
        );
    }
}
