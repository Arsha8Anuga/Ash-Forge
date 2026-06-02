using UnityEngine;

public class WeaponMagazine :
    MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField]
    private string ammoTypeId =
        "crude_ammo";

    [SerializeField]
    [Min(1)]
    private int maxAmmo = 10;

    [SerializeField]
    [Min(0)]
    private int currentAmmo = 10;

    public string AmmoTypeId =>
        ammoTypeId;

    public int CurrentAmmo =>
        currentAmmo;

    public int MaxAmmo =>
        maxAmmo;

    public bool IsEmpty =>
        currentAmmo <= 0;

    void Awake()
    {
        ClampAmmo();
    }

    public bool IsCompatibleWith(
        string acceptedAmmoTypeId)
    {
        if (string.IsNullOrEmpty(
            acceptedAmmoTypeId))
        {
            return true;
        }

        return ammoTypeId ==
            acceptedAmmoTypeId;
    }

    public bool TryConsumeRound(
        string acceptedAmmoTypeId)
    {
        if (!IsCompatibleWith(
            acceptedAmmoTypeId))
        {
            return false;
        }

        if (currentAmmo <= 0)
            return false;

        currentAmmo--;

        return true;
    }

    public void SetAmmo(
        int amount)
    {
        currentAmmo =
            Mathf.Clamp(
                amount,
                0,
                maxAmmo
            );
    }

    public int AddAmmo(
        int amount)
    {
        if (amount <= 0)
            return 0;

        int before =
            currentAmmo;

        currentAmmo =
            Mathf.Clamp(
                currentAmmo + amount,
                0,
                maxAmmo
            );

        return currentAmmo - before;
    }

    void ClampAmmo()
    {
        maxAmmo =
            Mathf.Max(
                1,
                maxAmmo
            );

        currentAmmo =
            Mathf.Clamp(
                currentAmmo,
                0,
                maxAmmo
            );
    }
}