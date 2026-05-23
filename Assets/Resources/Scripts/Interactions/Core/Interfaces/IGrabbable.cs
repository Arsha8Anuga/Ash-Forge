using UnityEngine;

public interface IGrabbable
{
    bool IsHeld { get; }
    bool Grab(
        XRHandInteractor hand,
        GrabMode mode
    );
    void Release(
        XRHandInteractor hand
    );
    GrabMode GetSupportedGrabMode();
    Transform GetGrabPoint();
}