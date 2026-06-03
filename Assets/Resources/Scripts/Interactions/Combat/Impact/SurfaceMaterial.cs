using UnityEngine;

public class SurfaceMaterial :
    MonoBehaviour
{
    [SerializeField]
    private SurfaceType surfaceType =
        SurfaceType.Default;

    public SurfaceType SurfaceType =>
        surfaceType;
}
