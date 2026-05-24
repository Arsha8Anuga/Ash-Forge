using UnityEngine;

public class StorageSpawnArea : MonoBehaviour
{
    [SerializeField]
    private BoxCollider area;

    void Awake()
    {
        if (area == null)
            area = GetComponent<BoxCollider>();
    }

    public Vector3 GetSpawnPosition()
    {
        if (area == null)
            return transform.position;

        Vector3 center =
            area.bounds.center;

        Vector3 size =
            area.bounds.size;

        return new Vector3(
            Random.Range(center.x - size.x * 0.5f, center.x + size.x * 0.5f),
            Random.Range(center.y - size.y * 0.5f, center.y + size.y * 0.5f),
            Random.Range(center.z - size.z * 0.5f, center.z + size.z * 0.5f)
        );
    }

    public Quaternion GetSpawnRotation()
    {
        return transform.rotation;
    }
}