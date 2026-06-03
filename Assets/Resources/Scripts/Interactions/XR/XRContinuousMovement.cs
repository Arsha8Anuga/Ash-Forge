using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(CharacterController))]
public class XRContinuousMovement : MonoBehaviour
{
    [Header("Input")]
    public InputActionProperty moveAction;
    public InputActionProperty turnAction;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float turnSpeed = 60f;
    public float gravity = -9.81f;

    private CharacterController character;
    private XROrigin xrOrigin;

    private bool referencesValid;

    private bool missingReferencesLogged;

    private float verticalVelocity;

    void Start()
    {
        ResolveReferences();

        referencesValid =
            HasRequiredReferences();

        if (!referencesValid)
        {
            LogMissingReferences();
        }
    }

    void Update()
    {
        if (!referencesValid)
        {
            ResolveReferences();

            referencesValid =
                HasRequiredReferences();

            if (!referencesValid)
            {
                LogMissingReferences();
                return;
            }
        }

        UpdateCharacterController();

        MovePlayer();

        RotatePlayer();
    }

    void ResolveReferences()
    {
        if (character == null)
        {
            character =
                GetComponent<CharacterController>();
        }

        if (xrOrigin == null)
        {
            xrOrigin =
                GetComponent<XROrigin>();
        }
    }

    bool HasRequiredReferences()
    {
        if (character == null)
            return false;

        if (xrOrigin == null)
            return false;

        if (xrOrigin.Camera == null)
            return false;

        return true;
    }

    void LogMissingReferences()
    {
        if (missingReferencesLogged)
            return;

        missingReferencesLogged = true;

        Debug.LogError(
            "[XRContinuousMovement] Missing required reference. " +
            "Required: CharacterController, XROrigin, and XROrigin Camera.",
            this
        );
    }

    void UpdateCharacterController()
    {
        float headHeight =
            Mathf.Clamp(
                xrOrigin.CameraInOriginSpaceHeight,
                1f,
                2f
            );

        character.height = headHeight;

        Vector3 center =
            xrOrigin.CameraInOriginSpacePos;

        center.y =
            character.height / 2
            + character.skinWidth;

        character.center = center;
    }

    void MovePlayer()
    {
        if (character == null ||
            xrOrigin == null ||
            xrOrigin.Camera == null)
        {
            return;
        }

        Vector2 input =
            moveAction.action != null
            ? moveAction.action.ReadValue<Vector2>()
            : Vector2.zero;

        Transform cameraTransform =
            xrOrigin.Camera.transform;

        Vector3 forward =
            cameraTransform.forward;

        Vector3 right =
            cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 move =
            (forward * input.y
            + right * input.x)
            * moveSpeed;

        if (character.isGrounded)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity +=
                gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;

        character.Move(
            move * Time.deltaTime
        );
    }

    void RotatePlayer()
    {
        if (turnAction.action == null)
            return;

        float turn =
            turnAction.action
            .ReadValue<Vector2>().x;

        transform.Rotate(
            Vector3.up,
            turn * turnSpeed
            * Time.deltaTime
        );
    }
}