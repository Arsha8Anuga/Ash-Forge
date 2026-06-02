using UnityEngine;

public class WeaponMagazineEjectButton :
    MonoBehaviour,
    IActivatable
{
    [SerializeField]
    private WeaponMagazineWell magazineWell;

    [SerializeField]
    private bool debugLog;

    void Awake()
    {
        if (magazineWell == null)
        {
            magazineWell =
                GetComponentInParent
                <WeaponMagazineWell>();
        }
    }

    public void Activate(
        XRHandInteractor hand)
    {
        if (magazineWell == null)
            return;

        WeaponMagazine magazine =
            magazineWell.EjectMagazine();

        if (!debugLog)
            return;

        Debug.Log(
            "[WeaponMagazineEjectButton] Eject: " +
            (
                magazine != null
                ? magazine.name
                : "none"
            ),
            this
        );
    }
}