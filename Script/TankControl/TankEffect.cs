using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class TankEffect : NetworkBehaviour
{
    // Getaran (Vibration)
    public GameObject vibratingObject;  // GameObject to apply vibration
    public float amplitude = 1.0f;      // Amplitude of the vibration
    public float frequency = 1.0f;      // Frequency of the vibration
    private float randomOffset;         // Random offset for vibration
    private float originalFrequency;    // Store the original frequency
    private float initialY;             // Store the initial Y position

    // Turret (Move Back and Return)
    public GameObject turretObject;     // Target GameObject to move back and forth
    public float moveDistance = 5.0f;   // Distance to move back
    public float moveDuration = 1.0f;   // Time required to move back
    public float returnDuration = 1.0f; // Time required to return to the original position
    private Vector3 originalLocalPosition;  // Original local position of the turret object
    private bool isMoving = false;      // Check if the turret is moving
    private TankStat tankStat;          // Reference to the TankStat class

    // WheelRotator (Rotation of Wheels)
    public GameObject[] leftObjects;    // Array for left-side objects (wheels)
    public GameObject[] rightObjects;   // Array for right-side objects (wheels)
    public float rotationSpeed = 100f;  // Maximum rotation speed
    public float acceleration = 50f;    // Acceleration rate
    public float deceleration = 30f;    // Deceleration rate
    private float currentLeftSpeed = 0f;  // Current rotation speed for the left wheels
    private float currentRightSpeed = 0f; // Current rotation speed for the right wheels

    void Start()
    {
        if (!IsOwner) return;  // Ensure only owner executes Start()

        // Vibration (Getaran)
        if (vibratingObject != null)
        {
            randomOffset = Random.Range(0f, 100f);
            originalFrequency = frequency;
            initialY = vibratingObject.transform.localPosition.y;
        }

        // Turret (Move Back and Return)
        if (turretObject != null)
        {
            originalLocalPosition = turretObject.transform.localPosition;
        }
        else
        {
            Debug.LogError("Turret Object is not assigned.");
        }

        tankStat = GetComponent<TankStat>();
        if (tankStat == null)
        {
            Debug.LogError("TankStat component is not found on this GameObject.");
        }
    }

    void Update()
    {
        if (!IsOwner) return;  // Ensure only owner can control effects in Update

        // Handle Vibration (Getaran)
        HandleVibration();

        // Handle Turret Movement
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            StartCoroutine(MoveBackAndReturn());
        }

        // Handle Wheel Rotation
        RotateLeftObjects();
        RotateRightObjects();
    }

    void HandleVibration()
    {
        if (vibratingObject != null)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                frequency = originalFrequency * 2; // Double the frequency
            }
            else
            {
                frequency = originalFrequency; // Restore original frequency
            }

            float newY = Mathf.Sin(Time.time * frequency + randomOffset) * amplitude;
            vibratingObject.transform.localPosition = new Vector3(
                vibratingObject.transform.localPosition.x, initialY + newY, vibratingObject.transform.localPosition.z);
        }
    }

    IEnumerator MoveBackAndReturn()
    {
        isMoving = true;
        Vector3 targetPosition = originalLocalPosition - turretObject.transform.localRotation * Vector3.forward * moveDistance;

        yield return StartCoroutine(MoveObject(turretObject.transform.localPosition, targetPosition, moveDuration * tankStat.fireSpeed));
        yield return StartCoroutine(MoveObject(turretObject.transform.localPosition, originalLocalPosition, returnDuration * tankStat.fireSpeed));

        isMoving = false;
    }

    IEnumerator MoveObject(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            turretObject.transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        turretObject.transform.localPosition = endPosition;
    }

    // Rotate left objects (wheels)
    void RotateLeftObjects()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))
        {
            currentLeftSpeed = Mathf.MoveTowards(currentLeftSpeed, rotationSpeed, acceleration * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentLeftSpeed = Mathf.MoveTowards(currentLeftSpeed, -rotationSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentLeftSpeed = Mathf.MoveTowards(currentLeftSpeed, 0f, deceleration * Time.deltaTime);
        }

        if (leftObjects != null && leftObjects.Length > 0)
        {
            foreach (GameObject obj in leftObjects)
            {
                if (obj != null)
                {
                    obj.transform.Rotate(Vector3.right * currentLeftSpeed * Time.deltaTime, Space.Self);
                }
            }
        }
    }

    // Rotate right objects (wheels)
    void RotateRightObjects()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W))
        {
            currentRightSpeed = Mathf.MoveTowards(currentRightSpeed, rotationSpeed, acceleration * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentRightSpeed = Mathf.MoveTowards(currentRightSpeed, -rotationSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentRightSpeed = Mathf.MoveTowards(currentRightSpeed, 0f, deceleration * Time.deltaTime);
        }

        if (rightObjects != null && rightObjects.Length > 0)
        {
            foreach (GameObject obj in rightObjects)
            {
                if (obj != null)
                {
                    obj.transform.Rotate(Vector3.right * currentRightSpeed * Time.deltaTime, Space.Self);
                }
            }
        }
    }
}
