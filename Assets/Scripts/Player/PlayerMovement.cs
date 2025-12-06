using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Basic movement")]
    public float speed = 3f;
    public Transform lookDirection;       

    [Header("Physics (optional)")]
    [Tooltip("Assign the player's Rigidbody (or leave empty to auto-find in children).")]
    public Rigidbody playerBody;

    void Start()
    {
        if (lookDirection == null && Camera.main != null)
        {
            lookDirection = Camera.main.transform;
        }

        if (lookDirection == null)
        {
            Debug.LogWarning("PlayerMovement: No lookDirection set and no MainCamera found. Please assign your VR camera.");
        }

        if (playerBody == null)
        {
            playerBody = GetComponent<Rigidbody>();
            if (playerBody == null)
            {
                playerBody = GetComponentInChildren<Rigidbody>();
            }
        }
    }

    void Update()
    {
        if (lookDirection == null) return;
        string pressed = GetPressedButtonName();
        bool isMoving = !string.IsNullOrEmpty(pressed);

        if (isMoving)
        {
            if (playerBody != null)
            {
                playerBody.velocity = Vector3.zero;
                playerBody.angularVelocity = Vector3.zero;
            }

            Debug.Log("Moving forward because: " + pressed);

            Vector3 forward = lookDirection.forward;
            forward.y = 0f;
            forward.Normalize();

            transform.position += forward * speed * Time.deltaTime;
        }
        else
        {
            // No input: stop drifting if something pushed the Rigidbody
            if (playerBody != null)
            {
                playerBody.velocity = Vector3.zero;
                playerBody.angularVelocity = Vector3.zero;
            }
        }
    }


    string GetPressedButtonName()
    {
        // 1. Mouse button 0 (often used for Cardboard on mobile)
        if (Input.GetMouseButton(0))
            return "Mouse0 (GetMouseButton(0))";

        // 2. Unity default "Fire1" axis (check in Edit > Project Settings > Input Manager)
        if (Input.GetButton("Fire1"))
            return "Fire1 (GetButton(\"Fire1\"))";

        // 3. Some other common defaults
        if (Input.GetButton("Fire2"))
            return "Fire2 (GetButton(\"Fire2\"))";

        if (Input.GetButton("Submit"))
            return "Submit (GetButton(\"Submit\"))";

        // 4. Spacebar (useful for testing in editor)
        if (Input.GetKey(KeyCode.Space))
            return "Space key";

        // 5. Touch on screen (for mobile builds)
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began ||
                t.phase == TouchPhase.Moved ||
                t.phase == TouchPhase.Stationary)
                return "Touch (Input.touchCount > 0)";
        }

        // Nothing pressed
        return null;
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        float originalSpeed = speed;
        speed = originalSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        speed = originalSpeed;
    }
}
