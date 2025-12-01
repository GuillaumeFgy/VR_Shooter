using UnityEngine;
using Google.XR.Cardboard;

/// <summary>
/// Handles player movement for Cardboard VR.
/// Player moves forward in the direction they are facing when the Cardboard trigger button is pressed.
/// </summary>
public class CardboardVRMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed in units per second")]
    public float speed = 6.0f;

    private Transform cam;
    private Rigidbody playerRigidbody;
    private Vector3 movement;

    void Awake()
    {
        // Get references
        playerRigidbody = GetComponent<Rigidbody>();

        // Get the Main Camera (first child of Player GameObject)
        cam = transform.GetChild(0);

        if (cam == null)
        {
            Debug.LogError("CardboardVRMovement: Camera not found as child of Player!");
        }
    }

    void FixedUpdate()
    {
        // Check if Cardboard trigger button is pressed
        if (Api.IsTriggerHeldPressed)
        {
            // Get the camera's forward direction
            Vector3 moveDirection = cam.forward;

            // Zero out the Y component to keep movement on horizontal plane
            moveDirection.y = 0;

            // Normalize to maintain consistent speed regardless of look angle
            moveDirection = moveDirection.normalized;

            // Move the player
            Move(moveDirection);
        }
    }

    void Move(Vector3 direction)
    {
        // Calculate movement based on speed and deltaTime
        movement = direction * speed * Time.deltaTime;

        // Move using Rigidbody physics
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void OnCollisionEnter()
    {
        // Stop velocity on collision to prevent sliding
        playerRigidbody.velocity = Vector3.zero;
    }
}
