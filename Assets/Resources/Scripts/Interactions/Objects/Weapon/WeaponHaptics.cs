using UnityEngine;

public class WeaponHaptics :
    MonoBehaviour
{
    [Header("Fire")]
    [SerializeField]
    private float fireAmplitude = 0.55f;

    [SerializeField]
    private float fireDuration = 0.045f;

    [SerializeField]
    private float dryFireAmplitude = 0.18f;

    [SerializeField]
    private float dryFireDuration = 0.035f;

    [Header("Magazine")]
    [SerializeField]
    private float magazineInsertAmplitude = 0.28f;

    [SerializeField]
    private float magazineInsertDuration = 0.055f;

    [SerializeField]
    private float magazineEjectAmplitude = 0.22f;

    [SerializeField]
    private float magazineEjectDuration = 0.045f;

    [Header("Hit")]
    [SerializeField]
    private float hitAmplitude = 0.12f;

    [SerializeField]
    private float hitDuration = 0.025f;

    public void PlayFireResult(
        WeaponFireResult result,
        XRHandInteractor hand)
    {
        switch (result)
        {
            case WeaponFireResult.Fired:
                Send(
                    hand,
                    fireAmplitude,
                    fireDuration
                );
                break;

            case WeaponFireResult.NoMagazine:
            case WeaponFireResult.EmptyOrIncompatible:
            case WeaponFireResult.MissingMagazineWell:
                Send(
                    hand,
                    dryFireAmplitude,
                    dryFireDuration
                );
                break;
        }
    }

    public void PlayMagazineInserted(
        XRHandInteractor hand)
    {
        Send(
            hand,
            magazineInsertAmplitude,
            magazineInsertDuration
        );
    }

    public void PlayMagazineEjected(
        XRHandInteractor hand)
    {
        Send(
            hand,
            magazineEjectAmplitude,
            magazineEjectDuration
        );
    }

    public void PlayHit(
        XRHandInteractor hand)
    {
        Send(
            hand,
            hitAmplitude,
            hitDuration
        );
    }

    void Send(
        XRHandInteractor hand,
        float amplitude,
        float duration)
    {
        if (hand == null)
            return;

        hand.SendHaptic(
            amplitude,
            duration
        );
    }
}
