using UnityEngine;
using Unity.Netcode;

public class TurretRotation : NetworkBehaviour
{
    public Transform targetObject;  // Object to follow the camera's Y rotation
    public Transform xFollower;     // Object to follow the camera's X rotation
    private Transform cameraTransform;

    [Range(0.1f, 5f)]
    public float rotationSmoothTime = 3f; // Higher values make the rotation faster

    void Start()
    {
        FindCamera();

        if (targetObject == null)
        {
            Debug.LogWarning("Target object not set. Please assign a target object in the Inspector.");
        }

        if (xFollower == null)
        {
            Debug.LogWarning("X Follower not set. Please assign an X Follower object in the Inspector.");
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (cameraTransform == null)
        {
            FindCamera();
        }

        if (cameraTransform != null)
        {
            if (targetObject != null)
            {
                // Fast Y-axis rotation to match camera's Y rotation
                Vector3 targetEulerAngles = targetObject.eulerAngles;
                targetEulerAngles.y = Mathf.LerpAngle(targetObject.eulerAngles.y, cameraTransform.eulerAngles.y, rotationSmoothTime);
                targetObject.eulerAngles = targetEulerAngles;
            }

            if (xFollower != null)
            {
                // Fast X-axis rotation to match camera's X rotation
                Vector3 followerEulerAngles = xFollower.eulerAngles;
                followerEulerAngles.x = Mathf.LerpAngle(xFollower.eulerAngles.x, cameraTransform.eulerAngles.x, rotationSmoothTime);
                xFollower.eulerAngles = followerEulerAngles;
            }
        }
    }

    private void FindCamera()
    {
        cameraTransform = Camera.main?.transform;
        if (cameraTransform == null)
        {
            Debug.LogWarning("Main Camera not found. Please ensure there is a camera tagged as 'MainCamera' in the scene.");
        }
    }
}
