using UnityEngine;
using System.Collections;

public class WeaponMuzzleFlash :
    MonoBehaviour
{
    [Header("Particles")]
    [SerializeField]
    private ParticleSystem[] flashParticles;

    [Header("Light")]
    [SerializeField]
    private Light flashLight;

    [SerializeField]
    private float lightDuration = 0.035f;

    [Header("Optional Prefab")]
    [SerializeField]
    private GameObject flashPrefab;

    [SerializeField]
    private Transform flashPoint;

    [SerializeField]
    private float prefabLifetime = 0.08f;

    private Coroutine lightRoutine;

    void Awake()
    {
        if (flashPoint == null)
        {
            flashPoint = transform;
        }

        if (flashParticles == null ||
            flashParticles.Length == 0)
        {
            flashParticles =
                GetComponentsInChildren
                <ParticleSystem>(true);
        }

        if (flashLight != null)
        {
            flashLight.enabled = false;
        }
    }

    public void Play()
    {
        PlayParticles();
        PlayLight();
        SpawnPrefab();
    }

    void PlayParticles()
    {
        if (flashParticles == null)
            return;

        foreach (ParticleSystem particle
            in flashParticles)
        {
            if (particle == null)
                continue;

            particle.Play(true);
        }
    }

    void PlayLight()
    {
        if (flashLight == null)
            return;

        if (lightRoutine != null)
        {
            StopCoroutine(
                lightRoutine
            );
        }

        lightRoutine =
            StartCoroutine(
                FlashLightRoutine()
            );
    }

    IEnumerator FlashLightRoutine()
    {
        flashLight.enabled = true;

        yield return new WaitForSeconds(
            Mathf.Max(
                0f,
                lightDuration
            )
        );

        flashLight.enabled = false;
        lightRoutine = null;
    }

    void SpawnPrefab()
    {
        if (flashPrefab == null)
            return;

        GameObject instance =
            GameObjectPool.Spawn(
                flashPrefab,
                flashPoint.position,
                flashPoint.rotation,
                flashPoint
            );

        if (prefabLifetime > 0f)
        {
            GameObjectPool.DespawnOrDestroy(
                instance,
                prefabLifetime
            );
        }
    }
}
