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