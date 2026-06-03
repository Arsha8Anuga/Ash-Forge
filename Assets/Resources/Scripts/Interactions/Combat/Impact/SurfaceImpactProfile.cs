using UnityEngine;

[CreateAssetMenu(
    fileName = "SurfaceImpactProfile",
    menuName = "Combat/Surface Impact Profile")]
public class SurfaceImpactProfile :
    ScriptableObject
{
    [SerializeField]
    private SurfaceImpactEffect defaultEffect;

    [SerializeField]
    private SurfaceImpactEffect[] effects;

    public SurfaceImpactEffect GetEffect(
        SurfaceType surfaceType)
    {
        if (effects != null)
        {
            foreach (SurfaceImpactEffect effect
                in effects)
            {
                if (effect == null)
                    continue;

                if (effect.SurfaceType == surfaceType)
                    return effect;
            }
        }

        return defaultEffect;
    }
}

[System.Serializable]
public class SurfaceImpactEffect
{
    [SerializeField]
    private SurfaceType surfaceType =
        SurfaceType.Default;

    [SerializeField]
    private GameObject particlePrefab;

    [SerializeField]
    private GameObject decalPrefab;

    [SerializeField]
    private AudioClip[] impactClips;

    [SerializeField]
    private float volume = 1f;

    [SerializeField]
    private float decalLifetime = 20f;

    public SurfaceType SurfaceType =>
        surfaceType;

    public GameObject ParticlePrefab =>
        particlePrefab;

    public GameObject DecalPrefab =>
        decalPrefab;

    public AudioClip[] ImpactClips =>
        impactClips;

    public float Volume =>
        volume;

    public float DecalLifetime =>
        decalLifetime;
}
