using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class WeaponFireResultUnityEvent :
    UnityEvent<WeaponFireResult>
{
}

[Serializable]
public class WeaponMagazineUnityEvent :
    UnityEvent<WeaponMagazine>
{
}

[Serializable]
public class WeaponAmmoUnityEvent :
    UnityEvent<int, int>
{
}

[Serializable]
public class WeaponHitUnityEvent :
    UnityEvent<WeaponHitInfo>
{
}

public class WeaponEvents :
    MonoBehaviour
{
    [Header("Fire")]
    [SerializeField]
    private WeaponFireResultUnityEvent fireResult;

    [SerializeField]
    private UnityEvent fired;

    [SerializeField]
    private WeaponFireResultUnityEvent fireFailed;

    [Header("Magazine")]
    [SerializeField]
    private WeaponMagazineUnityEvent magazineInserted;

    [SerializeField]
    private WeaponMagazineUnityEvent magazineEjected;

    [SerializeField]
    private WeaponAmmoUnityEvent ammoChanged;

    [Header("Hit")]
    [SerializeField]
    private WeaponHitUnityEvent hit;

    public event Action<WeaponFireResult, XRHandInteractor> FireResult;
    public event Action<XRHandInteractor> Fired;
    public event Action<WeaponFireResult, XRHandInteractor> FireFailed;
    public event Action<WeaponMagazine> MagazineInserted;
    public event Action<WeaponMagazine> MagazineEjected;
    public event Action<int, int> AmmoChanged;
    public event Action<WeaponHitInfo> Hit;

    public void RaiseFireResult(
        WeaponFireResult result,
        XRHandInteractor hand)
    {
        fireResult?.Invoke(
            result
        );

        FireResult?.Invoke(
            result,
            hand
        );

        if (result == WeaponFireResult.Fired)
        {
            fired?.Invoke();
            Fired?.Invoke(hand);
            return;
        }

        if (result != WeaponFireResult.None &&
            result != WeaponFireResult.Cooldown)
        {
            fireFailed?.Invoke(
                result
            );

            FireFailed?.Invoke(
                result,
                hand
            );
        }
    }

    public void RaiseMagazineInserted(
        WeaponMagazine magazine)
    {
        magazineInserted?.Invoke(
            magazine
        );

        MagazineInserted?.Invoke(
            magazine
        );
    }

    public void RaiseMagazineEjected(
        WeaponMagazine magazine)
    {
        magazineEjected?.Invoke(
            magazine
        );

        MagazineEjected?.Invoke(
            magazine
        );
    }

    public void RaiseAmmoChanged(
        int currentAmmo,
        int maxAmmo)
    {
        ammoChanged?.Invoke(
            currentAmmo,
            maxAmmo
        );

        AmmoChanged?.Invoke(
            currentAmmo,
            maxAmmo
        );
    }

    public void RaiseHit(
        WeaponHitInfo hitInfo)
    {
        hit?.Invoke(
            hitInfo
        );

        Hit?.Invoke(
            hitInfo
        );
    }
}
