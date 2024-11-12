using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CameraActivator : NetworkBehaviour
{
    [SerializeField] private Button toggleButton; // Button to trigger the camera switch
    [SerializeField] private GameObject cameraToDeactivate; // The camera to deactivate
    [SerializeField] private GameObject cameraToActivate;   // The camera to activate

    private void Awake()
    {
        // Add listener to button if it exists
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(OnToggleButtonClicked);
        }
    }

    // This method is triggered when the button is clicked
    private void OnToggleButtonClicked()
    {
        // Only the host can initiate the toggle action
        if (IsHost)
        {
            Debug.Log("Host is toggling the camera states.");
            ToggleCameraStateServerRpc();
        }
        else
        {
            Debug.LogWarning("Only the host can toggle the camera states.");
        }
    }

    // ServerRpc to initiate the camera toggle, ensuring it only runs on the server
    [ServerRpc(RequireOwnership = false)]
    private void ToggleCameraStateServerRpc()
    {
        Debug.Log("ServerRpc: Toggling camera states on the server.");

        // Toggle the cameras' active state on the server
        ToggleCameraState();

        // Inform all clients to update their camera states to match the host's
        UpdateCameraStateClientRpc(cameraToDeactivate.activeSelf, cameraToActivate.activeSelf);
    }

    // ClientRpc to synchronize the camera states across all clients
    [ClientRpc]
    private void UpdateCameraStateClientRpc(bool deactivateCameraState, bool activateCameraState)
    {
        Debug.Log("ClientRpc: Synchronizing camera states on all clients.");

        // Apply the received states on the client
        if (cameraToDeactivate != null && cameraToActivate != null)
        {
            cameraToDeactivate.SetActive(deactivateCameraState);
            cameraToActivate.SetActive(activateCameraState);
            Debug.Log("Client has updated camera states: " +
                      $"cameraToDeactivate active = {deactivateCameraState}, " +
                      $"cameraToActivate active = {activateCameraState}");
        }
        else
        {
            Debug.LogWarning("One or both cameras are not assigned on the client.");
        }
    }

    // Method to toggle the active states of the cameras
    private void ToggleCameraState()
    {
        if (cameraToDeactivate != null && cameraToActivate != null)
        {
            // Deactivate the specified camera and activate the other one
            cameraToDeactivate.SetActive(!cameraToDeactivate.activeSelf);
            cameraToActivate.SetActive(!cameraToActivate.activeSelf);

            Debug.Log("Cameras have been toggled on the server.");
        }
        else
        {
            Debug.LogWarning("One or both cameras are not assigned on the server.");
        }
    }
}
