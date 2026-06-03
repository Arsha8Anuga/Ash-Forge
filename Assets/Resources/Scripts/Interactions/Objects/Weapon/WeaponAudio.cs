using UnityEngine;

public class WeaponAudio :
    MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private AudioSource audioSource;

    [Header("Clips")]
    [SerializeField]
    private AudioClip fireClip;

    [SerializeField]
    private AudioClip dryFireClip;

    [SerializeField]
    private AudioClip magazineInsertClip;

    [SerializeField]
    private AudioClip magazineEjectClip;

    [Header("Pitch")]
    [SerializeField]
    private bool randomizePitch = true;

    [SerializeField]
    private Vector2 pitchRange =
        new Vector2(
            0.96f,
            1.04f
        );

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource =
                GetComponent<AudioSource>();
        }
    }

    public void PlayFireResult(
        WeaponFireResult result)
    {
        switch (result)
        {
            case WeaponFireResult.Fired:
                PlayFire();
                break;

            case WeaponFireResult.NoMagazine:
            case WeaponFireResult.EmptyOrIncompatible:
                PlayDryFire();
                break;
        }
    }

    public void PlayFire()
    {
        PlayClip(
            fireClip
        );
    }

    public void PlayDryFire()
    {
        PlayClip(
            dryFireClip
        );
    }

    public void PlayMagazineInsert()
    {
        PlayClip(
            magazineInsertClip
        );
    }

    public void PlayMagazineEject()
    {
        PlayClip(
            magazineEjectClip
        );
    }

    void PlayClip(
        AudioClip clip)
    {
        if (audioSource == null ||
            clip == null)
        {
            return;
        }

        float originalPitch =
            audioSource.pitch;

        if (randomizePitch)
        {
            audioSource.pitch =
                Random.Range(
                    pitchRange.x,
                    pitchRange.y
                );
        }

        audioSource.PlayOneShot(
            clip
        );

        audioSource.pitch =
            originalPitch;
    }
}
