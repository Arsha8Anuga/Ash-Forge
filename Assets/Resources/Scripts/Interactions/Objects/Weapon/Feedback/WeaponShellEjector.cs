using UnityEngine;

public class WeaponShellEjector :
    MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform ejectPoint;

    [SerializeField]
    private GameObject shellPrefab;

    [SerializeField]
    private GameObjectPool shellPool;

    [Header("Force")]
    [SerializeField]
    private Vector3 localEjectDirection =
        Vector3.right;

    [SerializeField]
    private float ejectForce = 1.6f;

    [SerializeField]
    private float upwardForce = 0.35f;

    [SerializeField]
    private float torqueForce = 0.08f;

    [Header("Lifetime")]
    [SerializeField]
    private float shellLifetime = 8f;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    void Awake()
    {
        if (ejectPoint == null)
        {
            ejectPoint = transform;
        }
    }

    public void Eject()
    {
        if (shellPrefab == null ||
            ejectPoint == null)
        {
            Log(
                "Shell prefab or eject point missing."
            );

            return;
        }

        GameObject shell =
            SpawnShell();

        if (shell == null)
            return;

        Rigidbody rb =
            shell.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;

            Vector3 direction =
                ejectPoint.TransformDirection(
                    localEjectDirection.normalized
                );

            Vector3 velocity =
                direction * ejectForce +
                Vector3.up * upwardForce;

            rb.AddForce(
                velocity,
                ForceMode.VelocityChange
            );

            rb.AddTorque(
                Random.insideUnitSphere *
                torqueForce,
                ForceMode.Impulse
            );
        }

        if (shellLifetime > 0f)
        {
            GameObjectPool.DespawnOrDestroy(
                shell,
                shellLifetime
            );
        }
    }

    GameObject SpawnShell()
    {
        if (shellPool != null)
        {
            GameObject pooled =
                shellPool.Get(
                    ejectPoint.position,
                    ejectPoint.rotation
                );

            if (pooled != null)
                return pooled;
        }

        return GameObjectPool.Spawn(
            shellPrefab,
            ejectPoint.position,
            ejectPoint.rotation
        );
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[WeaponShellEjector] " +
            message,
            this
        );
    }
}
