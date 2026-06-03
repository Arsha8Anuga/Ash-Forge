using UnityEngine;
using System.Collections.Generic;

public class GameObjectPool :
    MonoBehaviour
{
    private static readonly List<GameObjectPool> pools =
        new List<GameObjectPool>();

    [Header("Pool")]
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int prewarmCount = 8;

    [SerializeField]
    private Transform pooledParent;

    [SerializeField]
    private bool expandWhenEmpty = true;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    private readonly Queue<GameObject> available =
        new Queue<GameObject>();

    public GameObject Prefab =>
        prefab;

    void Awake()
    {
        if (pooledParent == null)
            pooledParent = transform;

        Register();
        Prewarm();
    }

    void OnEnable()
    {
        Register();
    }

    void OnDisable()
    {
        pools.Remove(this);
    }

    void Register()
    {
        if (!pools.Contains(this))
            pools.Add(this);
    }

    void Prewarm()
    {
        int count =
            Mathf.Max(
                0,
                prewarmCount
            );

        for (int i = 0; i < count; i++)
        {
            GameObject instance =
                CreateInstance();

            if (instance == null)
                return;

            Release(
                instance
            );
        }
    }

    GameObject CreateInstance()
    {
        if (prefab == null)
        {
            Log("Cannot create instance: prefab missing.");
            return null;
        }

        GameObject instance =
            Instantiate(
                prefab,
                pooledParent
            );

        PooledObject pooled =
            instance.GetComponent<PooledObject>();

        if (pooled == null)
        {
            pooled =
                instance.AddComponent<PooledObject>();
        }

        pooled.SetPool(this);

        instance.SetActive(false);

        return instance;
    }

    public GameObject Get(
        Vector3 position,
        Quaternion rotation,
        Transform parent = null)
    {
        GameObject instance = null;

        while (available.Count > 0 &&
            instance == null)
        {
            instance = available.Dequeue();
        }

        if (instance == null)
        {
            if (!expandWhenEmpty)
                return null;

            instance = CreateInstance();
        }

        if (instance == null)
            return null;

        Transform targetTransform =
            instance.transform;

        targetTransform.SetParent(
            parent,
            false
        );

        targetTransform.SetPositionAndRotation(
            position,
            rotation
        );

        PooledObject pooled =
            instance.GetComponent<PooledObject>();

        if (pooled != null)
            pooled.CancelReturn();

        instance.SetActive(true);

        return instance;
    }

    public void Release(
        GameObject instance)
    {
        if (instance == null)
            return;

        PooledObject pooled =
            instance.GetComponent<PooledObject>();

        if (pooled != null)
            pooled.CancelReturn();

        Rigidbody rb =
            instance.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        instance.transform.SetParent(
            pooledParent,
            false
        );

        instance.SetActive(false);

        available.Enqueue(
            instance
        );
    }

    public static GameObject Spawn(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation,
        Transform parent = null)
    {
        GameObjectPool pool =
            FindPoolForPrefab(
                prefab
            );

        if (pool == null)
        {
            return Instantiate(
                prefab,
                position,
                rotation,
                parent
            );
        }

        GameObject instance =
            pool.Get(
                position,
                rotation,
                parent
            );

        if (instance != null)
            return instance;

        return Instantiate(
            prefab,
            position,
            rotation,
            parent
        );
    }

    public static void DespawnOrDestroy(
        GameObject instance)
    {
        if (instance == null)
            return;

        PooledObject pooled =
            instance.GetComponent<PooledObject>();

        if (pooled != null &&
            pooled.HasPool)
        {
            pooled.ReturnNow();
            return;
        }

        Destroy(
            instance
        );
    }

    public static void DespawnOrDestroy(
        GameObject instance,
        float delay)
    {
        if (instance == null)
            return;

        delay =
            Mathf.Max(
                0f,
                delay
            );

        PooledObject pooled =
            instance.GetComponent<PooledObject>();

        if (pooled != null &&
            pooled.HasPool)
        {
            pooled.ReturnAfter(
                delay
            );

            return;
        }

        Destroy(
            instance,
            delay
        );
    }

    static GameObjectPool FindPoolForPrefab(
        GameObject prefab)
    {
        if (prefab == null)
            return null;

        for (int i = pools.Count - 1; i >= 0; i--)
        {
            GameObjectPool pool = pools[i];

            if (pool == null)
            {
                pools.RemoveAt(i);
                continue;
            }

            if (!pool.isActiveAndEnabled)
                continue;

            if (pool.prefab == prefab)
                return pool;
        }

        return null;
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[GameObjectPool] " +
            message,
            this
        );
    }
}
