using UnityEngine;

public class WeaponShooter :
    MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform muzzlePoint;

    [SerializeField]
    private WeaponMagazineWell magazineWell;

    [SerializeField]
    private WeaponInstanceHolder weaponInstanceHolder;

    [Header("Projectile")]
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private GameObjectPool projectilePool;

    [SerializeField]
    private bool requireMagazine = true;

    [SerializeField]
    private LayerMask projectileLayerMask = ~0;

    [Header("Damage")]
    [SerializeField]
    private float baseDamage = 10f;

    [SerializeField]
    private float damageStatFactor = 0.35f;

    [SerializeField]
    private float defectDamagePenalty = 0.08f;

    [SerializeField]
    private float minDamage = 1f;

    [SerializeField]
    private float maxDamage = 100f;

    [Header("Velocity")]
    [SerializeField]
    private float baseMuzzleVelocity = 25f;

    [SerializeField]
    private float damageVelocityFactor = 0.2f;

    [SerializeField]
    private float accuracyVelocityFactor = 0.08f;

    [SerializeField]
    private float defectVelocityPenalty = 0.08f;

    [SerializeField]
    private float minMuzzleVelocity = 10f;

    [SerializeField]
    private float maxMuzzleVelocity = 80f;

    [Header("Range")]
    [SerializeField]
    private float baseRange = 25f;

    [SerializeField]
    private float accuracyRangeFactor = 0.4f;

    [SerializeField]
    private float durabilityRangeFactor = 0.15f;

    [SerializeField]
    private float reliabilityRangeFactor = 0.15f;

    [SerializeField]
    private float defectRangePenalty = 0.3f;

    [SerializeField]
    private float minRange = 5f;

    [SerializeField]
    private float maxRange = 150f;

    [Header("Spread")]
    [SerializeField]
    private float maxSpreadAngle = 8f;

    [SerializeField]
    private float minSpreadAngle = 0.3f;

    [SerializeField]
    private float accuracySpreadReduction = 0.04f;

    [SerializeField]
    private float stabilitySpreadReduction = 0.025f;

    [SerializeField]
    private float defectSpreadPenalty = 0.05f;

    [Header("Cooldown")]
    [SerializeField]
    private float fallbackCooldown = 0.45f;

    [SerializeField]
    private float minFireCooldown = 0.12f;

    [SerializeField]
    private float maxFireCooldown = 0.8f;

    [SerializeField]
    private float handlingCooldownBonus = 0.001f;

    [SerializeField]
    private float defectCooldownPenalty = 0.002f;

    [Header("Lifetime")]
    [SerializeField]
    private float projectileLifetimePadding = 1f;

    [SerializeField]
    private float minProjectileLifetime = 0.5f;

    [SerializeField]
    private float maxProjectileLifetime = 10f;

    [SerializeField]
    private WeaponRecoil recoil;

    [Header("Feedback")]
    [SerializeField]
    private WeaponMuzzleFlash muzzleFlash;

    [SerializeField]
    private WeaponShellEjector shellEjector;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog = true;

    private float nextFireTime;

    public float CurrentCooldownRemaining =>
        Mathf.Max(
            0f,
            nextFireTime - Time.time
        );

    public bool IsCoolingDown =>
        CurrentCooldownRemaining > 0f;

    void Awake()
    {
        if (magazineWell == null)
        {
            magazineWell =
                GetComponentInChildren
                <WeaponMagazineWell>();
        }

        if (weaponInstanceHolder == null)
        {
            weaponInstanceHolder =
                GetComponent<WeaponInstanceHolder>();
        }

        if (recoil == null)
        {
            recoil =
                GetComponent<WeaponRecoil>();
        }

        if (muzzleFlash == null)
        {
            muzzleFlash =
                GetComponentInChildren
                <WeaponMuzzleFlash>();
        }

        if (shellEjector == null)
        {
            shellEjector =
                GetComponentInChildren
                <WeaponShellEjector>();
        }
    }

    public WeaponFireResult Fire()
    {
        WeaponFireResult startResult =
            CanStartFire();

        if (startResult !=
            WeaponFireResult.None)
        {
            return startResult;
        }

        ShotProfile shot =
            BuildShotProfile();

        WeaponFireResult ammoResult =
            TryConsumeAmmo();

        if (ammoResult !=
            WeaponFireResult.None)
        {
            return ammoResult;
        }

        nextFireTime =
            Time.time +
            shot.cooldown;

        SpawnProjectile(
            shot
        );

        ApplyRecoil();

        PlayShotFeedback();

        Log(
            "Fired | Ammo: " +
            GetAmmoText() +
            " | Cooldown: " +
            shot.cooldown.ToString("0.00") +
            " | Range: " +
            shot.range.ToString("0.0") +
            " | Velocity: " +
            shot.velocity.ToString("0.0") +
            " | Spread: " +
            shot.spreadAngle.ToString("0.0")
        );

        return WeaponFireResult.Fired;
    }

    void ApplyRecoil()
    {
        if (recoil == null)
            return;

        WeaponStatBlock stats =
            weaponInstanceHolder != null
            ? weaponInstanceHolder.Stats
            : null;

        recoil.ApplyRecoil(
            stats
        );
    }

    void PlayShotFeedback()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (shellEjector != null)
        {
            shellEjector.Eject();
        }
    }

    WeaponFireResult CanStartFire()
    {
        if (muzzlePoint == null)
        {
            Log("Cannot fire: muzzle missing.");
            return WeaponFireResult.MissingMuzzle;
        }

        if (IsCoolingDown)
        {
            Log(
                "Cannot fire: cooldown " +
                CurrentCooldownRemaining
                .ToString("0.00")
            );

            return WeaponFireResult.Cooldown;
        }

        return WeaponFireResult.None;
    }

    WeaponFireResult TryConsumeAmmo()
    {
        if (!requireMagazine)
            return WeaponFireResult.None;

        if (magazineWell == null)
        {
            Log("Cannot fire: magazine well missing.");
            return WeaponFireResult.MissingMagazineWell;
        }

        if (!magazineWell.HasMagazine)
        {
            Log("Cannot fire: no magazine.");
            return WeaponFireResult.NoMagazine;
        }

        bool consumed =
            magazineWell.TryConsumeRound();

        if (!consumed)
        {
            Log("Cannot fire: magazine empty or incompatible.");
            return WeaponFireResult.EmptyOrIncompatible;
        }

        return WeaponFireResult.None;
    }

    void SpawnProjectile(
        ShotProfile shot)
    {
        Vector3 direction =
            ApplySpread(
                muzzlePoint.forward,
                shot.spreadAngle
            );

        if (projectilePrefab == null)
        {
            Debug.DrawRay(
                muzzlePoint.position,
                direction * shot.range,
                Color.red,
                1f
            );

            Log("Projectile prefab missing. Drew debug ray instead.");
            return;
        }

        Quaternion rotation =
            Quaternion.LookRotation(
                direction,
                muzzlePoint.up
            );

        GameObject projectileObject =
            SpawnProjectileObject(
                muzzlePoint.position,
                rotation
            );

        if (projectileObject == null)
        {
            Log("Projectile spawn failed.");
            return;
        }

        WeaponProjectile projectile =
            projectileObject.GetComponent
            <WeaponProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(
                shot.damage,
                shot.range,
                gameObject,
                shot.lifetime,
                projectileLayerMask
            );
        }

        Rigidbody rb =
            projectileObject.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Log("Projectile spawned without Rigidbody.");
            return;
        }

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        rb.velocity =
            direction *
            shot.velocity;

        rb.WakeUp();
    }

    GameObject SpawnProjectileObject(
        Vector3 position,
        Quaternion rotation)
    {
        if (projectilePool != null)
        {
            GameObject pooled =
                projectilePool.Get(
                    position,
                    rotation
                );

            if (pooled != null)
                return pooled;
        }

        return GameObjectPool.Spawn(
            projectilePrefab,
            position,
            rotation
        );
    }

    Vector3 ApplySpread(
        Vector3 forward,
        float spreadAngle)
    {
        if (spreadAngle <= 0f)
            return forward.normalized;

        float spreadRadians =
            spreadAngle *
            Mathf.Deg2Rad;

        Vector2 random =
            Random.insideUnitCircle *
            Mathf.Tan(
                spreadRadians
            );

        Vector3 localDirection =
            new Vector3(
                random.x,
                random.y,
                1f
            ).normalized;

        Quaternion aimRotation =
            Quaternion.LookRotation(
                forward.normalized,
                muzzlePoint.up
            );

        return
            aimRotation *
            localDirection;
    }

    ShotProfile BuildShotProfile()
    {
        WeaponStatBlock stats =
            weaponInstanceHolder != null
            ? weaponInstanceHolder.Stats
            : null;

        if (stats == null)
        {
            return BuildFallbackShot();
        }

        float damage =
            baseDamage +
            stats.damage *
            damageStatFactor -
            stats.defect *
            defectDamagePenalty;

        damage =
            Mathf.Clamp(
                damage,
                minDamage,
                maxDamage
            );

        float velocity =
            baseMuzzleVelocity +
            stats.damage *
            damageVelocityFactor +
            stats.accuracy *
            accuracyVelocityFactor -
            stats.defect *
            defectVelocityPenalty;

        velocity =
            Mathf.Clamp(
                velocity,
                minMuzzleVelocity,
                maxMuzzleVelocity
            );

        float range =
            baseRange +
            stats.accuracy *
            accuracyRangeFactor +
            stats.durability *
            durabilityRangeFactor +
            stats.reliability *
            reliabilityRangeFactor -
            stats.defect *
            defectRangePenalty;

        range =
            Mathf.Clamp(
                range,
                minRange,
                maxRange
            );

        float spread =
            maxSpreadAngle -
            stats.accuracy *
            accuracySpreadReduction -
            stats.stability *
            stabilitySpreadReduction +
            stats.defect *
            defectSpreadPenalty;

        spread =
            Mathf.Clamp(
                spread,
                minSpreadAngle,
                maxSpreadAngle
            );

        float fireRate01 =
            Mathf.Clamp01(
                stats.fireRate / 100f
            );

        float cooldown =
            Mathf.Lerp(
                maxFireCooldown,
                minFireCooldown,
                fireRate01
            );

        cooldown -=
            stats.handling *
            handlingCooldownBonus;

        cooldown +=
            stats.defect *
            defectCooldownPenalty;

        cooldown =
            Mathf.Clamp(
                cooldown,
                minFireCooldown,
                maxFireCooldown
            );

        float lifetime =
            range /
            Mathf.Max(
                0.1f,
                velocity
            ) +
            projectileLifetimePadding;

        lifetime =
            Mathf.Clamp(
                lifetime,
                minProjectileLifetime,
                maxProjectileLifetime
            );

        return new ShotProfile
        {
            damage = damage,
            velocity = velocity,
            range = range,
            spreadAngle = spread,
            cooldown = cooldown,
            lifetime = lifetime
        };
    }

    ShotProfile BuildFallbackShot()
    {
        float lifetime =
            baseRange /
            Mathf.Max(
                0.1f,
                baseMuzzleVelocity
            ) +
            projectileLifetimePadding;

        lifetime =
            Mathf.Clamp(
                lifetime,
                minProjectileLifetime,
                maxProjectileLifetime
            );

        return new ShotProfile
        {
            damage =
                Mathf.Clamp(
                    baseDamage,
                    minDamage,
                    maxDamage
                ),

            velocity =
                Mathf.Clamp(
                    baseMuzzleVelocity,
                    minMuzzleVelocity,
                    maxMuzzleVelocity
                ),

            range =
                Mathf.Clamp(
                    baseRange,
                    minRange,
                    maxRange
                ),

            spreadAngle =
                Mathf.Clamp(
                    maxSpreadAngle * 0.5f,
                    minSpreadAngle,
                    maxSpreadAngle
                ),

            cooldown =
                Mathf.Clamp(
                    fallbackCooldown,
                    minFireCooldown,
                    maxFireCooldown
                ),

            lifetime = lifetime
        };
    }

    public string GetAmmoText()
    {
        if (magazineWell == null ||
            !magazineWell.HasMagazine)
        {
            return "none";
        }

        return
            magazineWell.CurrentAmmo +
            "/" +
            magazineWell.MaxAmmo;
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[WeaponShooter] " +
            message,
            this
        );
    }

    struct ShotProfile
    {
        public float damage;

        public float velocity;

        public float range;

        public float spreadAngle;

        public float cooldown;

        public float lifetime;
    }
}
