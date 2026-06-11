using UnityEngine;

public class ImpactEffectSpawner :
    MonoBehaviour
{
    [Header("Profile")]
    [SerializeField]
    private SurfaceImpactProfile profile;

    [Header("Spawn")]
    [SerializeField]
    private float surfaceOffset = 0.004f;

    [SerializeField]
    private bool parentDecalToHitObject = true;

    [SerializeField]
    private bool randomizeDecalRoll = true;

    [Header("Audio")]
    [SerializeField]
    private bool usePlayClipAtPoint = true;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    public void SpawnImpact(
        WeaponHitInfo hitInfo)
    {
        SurfaceImpactEffect effect =
            profile != null
            ? profile.GetEffect(
                hitInfo.surfaceType)
            : null;

        if (effect == null)
        {
            Log(
                "No impact effect for surface: " +
                hitInfo.surfaceType
            );

            return;
        }

        SpawnParticle(
            effect,
            hitInfo
        );

        SpawnDecal(
            effect,
            hitInfo
        );

        PlayImpactSound(
            effect,
            hitInfo
        );
    }

    void SpawnParticle(
        SurfaceImpactEffect effect,
        WeaponHitInfo hitInfo)
    {
        if (effect.ParticlePrefab == null)
            return;

        Instantiate(
            effect.ParticlePrefab,
            GetOffsetPoint(hitInfo),
            GetImpactRotation(hitInfo)
        );
    }

    void SpawnDecal(
        SurfaceImpactEffect effect,
        WeaponHitInfo hitInfo)
    {
        if (effect.DecalPrefab == null)
            return;

        if (hitInfo.collider == null)
            return;

        BulletHoleReceiver receiver =
            hitInfo.collider.GetComponentInParent<BulletHoleReceiver>();

        if (receiver != null &&
            !receiver.CanReceiveBulletHole())
        {
            Log("This object does not accept bullet holes: " +
                hitInfo.collider.name);

            return;
        }

        Transform parent = null;

        if (receiver != null)
        {
            parent =
                receiver.GetBulletHoleParent(hitInfo.collider);
        }
        else if (parentDecalToHitObject)
        {
            parent =
                hitInfo.collider.transform;
        }

        GameObject decal =
            Instantiate(
                effect.DecalPrefab,
                GetOffsetPoint(hitInfo),
                GetImpactRotation(hitInfo)
            );

        if (parent != null)
        {
            decal.transform.SetParent(
                parent,
                true
            );
        }

        float lifetime =
            effect.DecalLifetime;

        if (lifetime > 0f)
        {
            Destroy(
                decal,
                lifetime
            );
        }
    }

    void PlayImpactSound(
        SurfaceImpactEffect effect,
        WeaponHitInfo hitInfo)
    {
        AudioClip clip =
            PickClip(
                effect.ImpactClips
            );

        if (clip == null)
            return;

        if (usePlayClipAtPoint)
        {
            AudioSource.PlayClipAtPoint(
                clip,
                hitInfo.point,
                Mathf.Max(
                    0f,
                    effect.Volume
                )
            );
        }
    }

    AudioClip PickClip(
        AudioClip[] clips)
    {
        if (clips == null ||
            clips.Length == 0)
        {
            return null;
        }

        int index =
            Random.Range(
                0,
                clips.Length
            );

        return clips[index];
    }

    Vector3 GetOffsetPoint(
        WeaponHitInfo hitInfo)
    {
        return
            hitInfo.point +
            hitInfo.normal.normalized *
            surfaceOffset;
    }

    Quaternion GetImpactRotation(
        WeaponHitInfo hitInfo)
    {
        Vector3 normal =
            hitInfo.normal.sqrMagnitude > 0.001f
            ? hitInfo.normal.normalized
            : Vector3.up;

        Quaternion rotation =
            Quaternion.LookRotation(
                normal
            );

        if (randomizeDecalRoll)
        {
            rotation *=
                Quaternion.Euler(
                    0f,
                    0f,
                    Random.Range(
                        0f,
                        360f
                    )
                );
        }

        return rotation;
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[ImpactEffectSpawner] " +
            message,
            this
        );
    }
}
