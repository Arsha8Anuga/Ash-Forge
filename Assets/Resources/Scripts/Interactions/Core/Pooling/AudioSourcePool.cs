using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSourcePool :
    MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]
    private AudioSource sourcePrefab;

    [SerializeField]
    private int prewarmCount = 12;

    [SerializeField]
    private Transform pooledParent;

    [SerializeField]
    private bool spatialByDefault = true;

    private readonly Queue<AudioSource> available =
        new Queue<AudioSource>();

    void Awake()
    {
        if (pooledParent == null)
            pooledParent = transform;

        Prewarm();
    }

    void Prewarm()
    {
        int count =
            Mathf.Max(
                0,
                prewarmCount
            );

        for (int i = 0; i < count; i++)
        {
            AudioSource source =
                CreateSource();

            Release(
                source
            );
        }
    }

    AudioSource CreateSource()
    {
        AudioSource source = null;

        if (sourcePrefab != null)
        {
            source =
                Instantiate(
                    sourcePrefab,
                    pooledParent
                );
        }
        else
        {
            GameObject obj =
                new GameObject(
                    "Pooled Audio Source"
                );

            obj.transform.SetParent(
                pooledParent,
                false
            );

            source =
                obj.AddComponent<AudioSource>();

            source.playOnAwake = false;

            if (spatialByDefault)
                source.spatialBlend = 1f;
        }

        source.gameObject.SetActive(false);

        return source;
    }

    public void PlayClipAt(
        AudioClip clip,
        Vector3 position,
        float volume = 1f,
        float pitch = 1f)
    {
        if (clip == null)
            return;

        AudioSource source =
            GetSource();

        if (source == null)
            return;

        source.transform.position = position;
        source.clip = clip;
        source.volume = Mathf.Max(0f, volume);
        source.pitch = Mathf.Max(0.01f, pitch);
        source.gameObject.SetActive(true);
        source.Play();

        float lifetime =
            clip.length /
            Mathf.Max(
                0.01f,
                source.pitch
            ) +
            0.05f;

        StartCoroutine(
            ReleaseAfter(
                source,
                lifetime
            )
        );
    }

    AudioSource GetSource()
    {
        AudioSource source = null;

        while (available.Count > 0 &&
            source == null)
        {
            source = available.Dequeue();
        }

        if (source == null)
            source = CreateSource();

        return source;
    }

    IEnumerator ReleaseAfter(
        AudioSource source,
        float delay)
    {
        yield return new WaitForSeconds(
            Mathf.Max(
                0f,
                delay
            )
        );

        Release(
            source
        );
    }

    void Release(
        AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
        source.clip = null;
        source.transform.SetParent(
            pooledParent,
            false
        );
        source.gameObject.SetActive(false);

        available.Enqueue(
            source
        );
    }
}
