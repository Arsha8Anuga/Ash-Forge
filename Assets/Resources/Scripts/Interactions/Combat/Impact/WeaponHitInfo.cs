using UnityEngine;

public struct WeaponHitInfo
{
    public float damage;

    public Vector3 point;

    public Vector3 normal;

    public Vector3 direction;

    public GameObject source;

    public GameObject projectile;

    public Collider collider;

    public SurfaceType surfaceType;
}
