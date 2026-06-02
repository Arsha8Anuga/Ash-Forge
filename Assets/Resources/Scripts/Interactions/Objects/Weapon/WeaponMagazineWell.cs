using UnityEngine;
using System.Collections;
public class WeaponMagazineWell :
    MonoBehaviour
{
    [Header("Snap")]
    [SerializeField]
    private SnapSocket socket;

    [Header("Ammo")]
    [SerializeField]
    private string acceptedAmmoTypeId =
        "crude_ammo";

    [Header("Eject")]
    [SerializeField]
    private Transform ejectDirection;

    [SerializeField]
    private float ejectVelocity = 1.5f;

    [Header("Eject Safety")]
    [SerializeField]
    private bool hideMagazineDuringEject = true;

    [SerializeField]
    private float ejectInvisibleDuration = 0.08f;

    [SerializeField]
    private bool ignoreWeaponCollisionDuringEject = true;

    [SerializeField]
    private float ignoreWeaponCollisionDuration = 0.2f;

    [SerializeField]
    private float ejectStartOffset = 0.08f;

    [SerializeField]
    private CollisionDetectionMode ejectCollisionMode =
        CollisionDetectionMode.ContinuousDynamic;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    public WeaponMagazine CurrentMagazine
    {
        get
        {
            SnappableObject current =
                GetCurrentSnappable();

            if (current == null)
                return null;

            return current.GetComponent
                <WeaponMagazine>();
        }
    }

    public bool HasMagazine =>
        CurrentMagazine != null;

    public bool HasAmmo =>
        CurrentMagazine != null &&
        CurrentMagazine.CurrentAmmo > 0;

    public int CurrentAmmo =>
        CurrentMagazine != null
        ? CurrentMagazine.CurrentAmmo
        : 0;

    public int MaxAmmo =>
        CurrentMagazine != null
        ? CurrentMagazine.MaxAmmo
        : 0;

    public string AcceptedAmmoTypeId =>
        acceptedAmmoTypeId;

    void Awake()
    {
        if (socket == null)
        {
            socket =
                GetComponent<SnapSocket>();
        }

        if (ejectDirection == null)
        {
            ejectDirection = transform;
        }
    }

    public bool TryConsumeRound()
    {
        WeaponMagazine magazine =
            CurrentMagazine;

        if (magazine == null)
        {
            Log("No magazine.");
            return false;
        }

        bool success =
            magazine.TryConsumeRound(
                acceptedAmmoTypeId
            );

        if (!success)
        {
            Log("Cannot consume round.");
        }

        return success;
    }

   public WeaponMagazine EjectMagazine()
    {
        if (socket == null)
        {
            Log("Eject failed: socket null.");
            return null;
        }

        SnappableObject snap =
            socket.Current;

        if (snap == null)
        {
            Log("Eject failed: socket current null.");
            return null;
        }

        WeaponMagazine magazine =
            snap.GetComponent<WeaponMagazine>();

        if (magazine == null)
        {
            Log("Eject failed: current object is not magazine.");
            return null;
        }

        bool success =
            socket.TryUnsnapCurrent();

        if (!success)
        {
            Log("Eject failed: unsnap failed.");
            return null;
        }

        Vector3 direction =
            ejectDirection != null
            ? ejectDirection.forward
            : transform.forward;

        direction =
            direction.normalized;

        if (hideMagazineDuringEject)
        {
            StartCoroutine(
                TemporarilyHideMagazine(
                    magazine
                )
            );
        }

        if (ignoreWeaponCollisionDuringEject)
        {
            StartCoroutine(
                TemporarilyIgnoreWeaponCollision(
                    magazine
                )
            );
        }

        Rigidbody rb =
            magazine.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.collisionDetectionMode =
                ejectCollisionMode;

            rb.interpolation =
                RigidbodyInterpolation.Interpolate;

            rb.isKinematic = false;
            rb.useGravity = true;

            rb.velocity =
                Vector3.zero;

            rb.angularVelocity =
                Vector3.zero;

            if (ejectStartOffset > 0f)
            {
                rb.position +=
                    direction *
                    ejectStartOffset;

                magazine.transform.position =
                    rb.position;

                Physics.SyncTransforms();
            }

            rb.velocity =
                direction *
                ejectVelocity;

            rb.WakeUp();
        }
        else
        {
            Log("Eject warning: magazine has no Rigidbody after unsnap.");
        }

        Log("Magazine ejected.");

        return magazine;
    }

    IEnumerator TemporarilyHideMagazine(
        WeaponMagazine magazine)
    {
        if (magazine == null)
            yield break;

        Renderer[] renderers =
            magazine.GetComponentsInChildren
            <Renderer>(true);

        SetRenderersEnabled(
            renderers,
            false
        );

        yield return new WaitForSeconds(
            Mathf.Max(
                0f,
                ejectInvisibleDuration
            )
        );

        SetRenderersEnabled(
            renderers,
            true
        );
    }

    void SetRenderersEnabled(
        Renderer[] renderers,
        bool enabled)
    {
        if (renderers == null)
            return;

        foreach (Renderer renderer
            in renderers)
        {
            if (renderer == null)
                continue;

            renderer.enabled =
                enabled;
        }
    }

    IEnumerator TemporarilyIgnoreWeaponCollision(
        WeaponMagazine magazine)
    {
        if (magazine == null)
            yield break;

        Collider[] magazineColliders =
            magazine.GetComponentsInChildren
            <Collider>(true);

        Collider[] weaponColliders =
            GetComponentsInParent
            <Collider>(true);

        SetCollisionIgnore(
            magazineColliders,
            weaponColliders,
            true
        );

        yield return new WaitForSeconds(
            Mathf.Max(
                0f,
                ignoreWeaponCollisionDuration
            )
        );

        SetCollisionIgnore(
            magazineColliders,
            weaponColliders,
            false
        );
    }

    void SetCollisionIgnore(
        Collider[] a,
        Collider[] b,
        bool ignore)
    {
        if (a == null ||
            b == null)
        {
            return;
        }

        foreach (Collider first
            in a)
        {
            if (first == null)
                continue;

            foreach (Collider second
                in b)
            {
                if (second == null)
                    continue;

                if (first == second)
                    continue;

                Physics.IgnoreCollision(
                    first,
                    second,
                    ignore
                );
            }
        }
    }

    SnappableObject GetCurrentSnappable()
    {
        if (socket == null)
            return null;

        return socket.Current;
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[WeaponMagazineWell] " +
            message,
            this
        );
    }
}