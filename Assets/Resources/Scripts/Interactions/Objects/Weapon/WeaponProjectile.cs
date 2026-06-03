using UnityEngine;
using System;

public class WeaponProjectile :
    MonoBehaviour
{
    [Header("Hit")]
    [SerializeField]
    private bool destroyOnHit = true;

    [SerializeField]
    private bool sendTargetHit = true;

    [SerializeField]
    private SurfaceType fallbackSurfaceType =
        SurfaceType.Default;

    [Header("Cast Hit Detection")]
    [SerializeField]
    private bool useCastHitDetection = true;

    [SerializeField]
    private float castRadius = 0.025f;

    [SerializeField]
    private LayerMask hitLayerMask = ~0;

    [SerializeField]
    private QueryTriggerInteraction triggerInteraction =
        QueryTriggerInteraction.Ignore;

    [Header("Impact")]
    [SerializeField]
    private ImpactEffectSpawner impactSpawner;

    [SerializeField]
    private bool findImpactSpawnerIfMissing = true;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    private Vector3 startPosition;
    private Vector3 previousPosition;

    private float damage;
    private float maxRange = 30f;
    private float destroyTime;

    private GameObject owner;
    private WeaponEvents ownerEvents;

    private bool initialized;
    private bool hasHit;

    private Rigidbody rb;

    void Awake()
    {
        rb =
            GetComponent<Rigidbody>();

        startPosition =
            transform.position;

        previousPosition =
            transform.position;

        destroyTime =
            Time.time + 5f;

        ResolveImpactSpawner();
    }

    void OnEnable()
    {
        hasHit = false;
        previousPosition = transform.position;
    }

    public void Initialize(
        float damage,
        float maxRange,
        GameObject owner,
        float lifetime,
        LayerMask hitLayerMask)
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

        ownerEvents =
            owner != null
            ? owner.GetComponent<WeaponEvents>()
            : null;

        this.hitLayerMask =
            hitLayerMask;

        startPosition =
            transform.position;

        previousPosition =
            transform.position;

        destroyTime =
            Time.time +
            Mathf.Max(
                0.1f,
                lifetime
            );

        initialized = true;
        hasHit = false;

        ResolveImpactSpawner();
        IgnoreOwnerCollision();
    }

    public void Initialize(
        float damage,
        float maxRange,
        GameObject owner,
        float lifetime)
    {
        Initialize(
            damage,
            maxRange,
            owner,
            lifetime,
            hitLayerMask
        );
    }

    void Update()
    {
        if (!initialized ||
            hasHit)
        {
            return;
        }

        if (useCastHitDetection)
        {
            CheckCastHit();
        }

        if (hasHit)
            return;

        float distance =
            Vector3.Distance(
                startPosition,
                transform.position
            );

        if (distance >= maxRange)
        {
            Despawn();
            return;
        }

        if (Time.time >= destroyTime)
        {
            Despawn();
            return;
        }

        previousPosition =
            transform.position;
    }

    void CheckCastHit()
    {
        Vector3 currentPosition =
            transform.position;

        Vector3 delta =
            currentPosition -
            previousPosition;

        float distance =
            delta.magnitude;

        if (distance <= 0.0001f)
            return;

        Vector3 direction =
            delta / distance;

        RaycastHit hit;

        bool found =
            TryFindCastHit(
                previousPosition,
                direction,
                distance,
                out hit
            );

        if (!found)
            return;

        HandleHit(
            hit.collider,
            hit.point,
            hit.normal
        );
    }

    bool TryFindCastHit(
        Vector3 origin,
        Vector3 direction,
        float distance,
        out RaycastHit bestHit)
    {
        bestHit = default;

        RaycastHit[] hits;

        if (castRadius > 0f)
        {
            hits =
                Physics.SphereCastAll(
                    origin,
                    castRadius,
                    direction,
                    distance,
                    hitLayerMask,
                    triggerInteraction
                );
        }
        else
        {
            hits =
                Physics.RaycastAll(
                    origin,
                    direction,
                    distance,
                    hitLayerMask,
                    triggerInteraction
                );
        }

        if (hits == null ||
            hits.Length == 0)
        {
            return false;
        }

        Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(
                    b.distance
                )
        );

        foreach (RaycastHit hit in hits)
        {
            if (ShouldIgnoreCollider(
                hit.collider))
            {
                continue;
            }

            bestHit = hit;
            return true;
        }

        return false;
    }

    void OnCollisionEnter(
        Collision collision)
    {
        if (hasHit ||
            collision == null)
        {
            return;
        }

        Collider hitCollider =
            collision.collider;

        if (ShouldIgnoreCollider(
            hitCollider))
        {
            return;
        }

        ContactPoint contact =
            collision.contactCount > 0
            ? collision.GetContact(0)
            : default;

        Vector3 point =
            collision.contactCount > 0
            ? contact.point
            : transform.position;

        Vector3 normal =
            collision.contactCount > 0
            ? contact.normal
            : -GetTravelDirection();

        HandleHit(
            hitCollider,
            point,
            normal
        );
    }

    void OnTriggerEnter(
        Collider other)
    {
        if (hasHit ||
            other == null)
        {
            return;
        }

        if (ShouldIgnoreCollider(
            other))
        {
            return;
        }

        Vector3 direction =
            GetTravelDirection();

        Vector3 point =
            other.ClosestPoint(
                transform.position
            );

        Vector3 normal =
            -direction;

        HandleHit(
            other,
            point,
            normal
        );
    }

    void HandleHit(
        Collider hitCollider,
        Vector3 point,
        Vector3 normal)
    {
        if (hasHit)
            return;

        hasHit = true;

        SurfaceType surfaceType =
            ResolveSurfaceType(
                hitCollider
            );

        WeaponHitInfo hitInfo =
            new WeaponHitInfo
            {
                damage = damage,
                point = point,
                normal = normal,
                direction = GetTravelDirection(),
                source = owner,
                projectile = gameObject,
                collider = hitCollider,
                surfaceType = surfaceType
            };

        if (debugLog)
        {
            Debug.Log(
                "[WeaponProjectile] Hit: " +
                hitCollider.name +
                " | Surface: " +
                surfaceType +
                " | Damage: " +
                damage.ToString("0.0"),
                this
            );
        }

        if (impactSpawner != null)
        {
            impactSpawner.SpawnImpact(
                hitInfo
            );
        }

        if (sendTargetHit)
        {
            IWeaponHitReceiver receiver =
                FindHitReceiver(
                    hitCollider
                );

            if (receiver != null)
            {
                receiver.ReceiveWeaponHit(
                    hitInfo
                );
            }
        }

        if (ownerEvents != null)
        {
            ownerEvents.RaiseHit(
                hitInfo
            );
        }

        if (destroyOnHit)
        {
            Despawn();
        }
    }

    bool ShouldIgnoreCollider(
        Collider hitCollider)
    {
        if (hitCollider == null)
            return true;

        if (!IsLayerAllowed(
            hitCollider.gameObject.layer))
        {
            return true;
        }

        if (owner == null)
            return false;

        return hitCollider.transform.IsChildOf(
            owner.transform
        );
    }

    bool IsLayerAllowed(
        int layer)
    {
        return
            (hitLayerMask.value &
            (1 << layer)) != 0;
    }

    SurfaceType ResolveSurfaceType(
        Collider hitCollider)
    {
        if (hitCollider == null)
            return fallbackSurfaceType;

        SurfaceMaterial surface =
            hitCollider.GetComponentInParent
            <SurfaceMaterial>();

        if (surface == null)
            return fallbackSurfaceType;

        return surface.SurfaceType;
    }

    IWeaponHitReceiver FindHitReceiver(
        Collider hitCollider)
    {
        if (hitCollider == null)
            return null;

        MonoBehaviour[] behaviours =
            hitCollider.GetComponentsInParent
            <MonoBehaviour>();

        foreach (MonoBehaviour behaviour
            in behaviours)
        {
            if (behaviour == null)
                continue;

            IWeaponHitReceiver receiver =
                behaviour as IWeaponHitReceiver;

            if (receiver != null)
                return receiver;
        }

        return null;
    }

    Vector3 GetTravelDirection()
    {
        if (rb != null &&
            rb.velocity.sqrMagnitude > 0.001f)
        {
            return rb.velocity.normalized;
        }

        return transform.forward;
    }

    void ResolveImpactSpawner()
    {
        if (impactSpawner != null)
            return;

        if (!findImpactSpawnerIfMissing)
            return;

        impactSpawner =
            FindObjectOfType<ImpactEffectSpawner>();
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

    void Despawn()
    {
        initialized = false;
        hasHit = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        GameObjectPool.DespawnOrDestroy(
            gameObject
        );
    }
}
