using UnityEngine;

public class WeaponShooter :
    MonoBehaviour
{
    [SerializeField]
    private Transform muzzlePoint;

    [SerializeField]
    private float fireDistance =
        100f;

    public void Fire()
    {
        if (muzzlePoint == null)
        {
            Debug.LogWarning(
                "[WeaponShooter] Cannot fire. Muzzle point is missing.",
                this
            );

            return;
        }

        Debug.Log(
            $"{name} Fired"
        );

        Debug.DrawRay(
            muzzlePoint.position,
            muzzlePoint.forward *
            fireDistance,
            Color.red,
            1f
        );
    }
}