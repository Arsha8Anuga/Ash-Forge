using UnityEngine;

public class WeaponProjectile :
    MonoBehaviour
{
    [Header("Hit")]
    [SerializeField]
    private bool destroyOnHit = true;

    [SerializeField]
    private bool debugLog;

    private Vector3 startPosition;

    private float damage;

    private float maxRange = 30f;

    private float destroyTime;

    private GameObject owner;

    private bool initialized;

    void Awake()
    {
        startPosition =
            transform.position;

        destroyTime =
            Time.time + 5f;
    }

    public void Initialize(
        float damage,
        float maxRange,
        GameObject owner,
        float lifetime)
    {
        this.damage =
            Mathf.Max(
                0f,
                damage
            );

        this.maxRange =
            Mathf.Max(
                0.1f,
                maxRange
            );

        this.owner = owner;

        startPosition =
            transform.position;

        destroyTime =
            Time.time +
            Mathf.Max(
                0.1f,
                lifetime
            );

        initialized = true;

        IgnoreOwnerCollision();
    }

    void Update()
    {
        if (!initialized)
            return;

        float distance =
            Vector3.Distance(
                startPosition,
                transform.position
            );

        if (distance >= maxRange)
        {
            Destroy(
                gameObject
            );

            return;
        }

        if (Time.time >= destroyTime)
        {
            Destroy(
                gameObject
            );
        }
    }

    void OnCollisionEnter(
        Collision collision)
    {
        if (collision == null)
            return;

        if (owner != null &&
            collision.transform.IsChildOf(
                owner.transform))
        {
            return;
        }

        if (debugLog)
        {
            Debug.Log(
                "[WeaponProjectile] Hit: " +
                collision.collider.name +
                " | Damage: " +
                damage.ToString("0.0"),
                this
            );
        }

        if (destroyOnHit)
        {
            Destroy(
                gameObject
            );
        }
    }

    void OnTriggerEnter(
        Collider other)
    {
        if (other == null)
            return;

        if (owner != null &&
            other.transform.IsChildOf(
                owner.transform))
        {
            return;
        }

        if (debugLog)
        {
            Debug.Log(
                "[WeaponProjectile] Trigger hit: " +
                other.name +
                " | Damage: " +
                damage.ToString("0.0"),
                this
            );
        }

        if (destroyOnHit)
        {
            Destroy(
                gameObject
            );
        }
    }

    void IgnoreOwnerCollision()
    {
        if (owner == null)
            return;

        Collider[] projectileColliders =
            GetComponentsInChildren
            <Collider>();

        Collider[] ownerColliders =
            owner.GetComponentsInChildren
            <Collider>();

        foreach (Collider projectileCollider
            in projectileColliders)
        {
            if (projectileCollider == null)
                continue;

            foreach (Collider ownerCollider
                in ownerColliders)
            {
                if (ownerCollider == null)
                    continue;

                if (projectileCollider ==
                    ownerCollider)
                {
                    continue;
                }

                Physics.IgnoreCollision(
                    projectileCollider,
                    ownerCollider,
                    true
                );
            }
        }
    }
}