using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerTeleportManager : NetworkBehaviour
{
    [SerializeField] private List<Transform> teleportTargets; // Teleport targets for each player
    [SerializeField] private Button teleportButton; // UI Button to trigger teleportation

    private Dictionary<ulong, GameObject> players = new Dictionary<ulong, GameObject>();

    private void Start()
    {
        if (IsHost)
        {
            // Manually add the host to the players list
            var hostClientId = NetworkManager.Singleton.LocalClientId;
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(hostClientId, out var networkClient))
            {
                players[hostClientId] = networkClient.PlayerObject.gameObject;
                Debug.Log("Host added to players list.");
            }

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            // Add event listener to the teleport button
            if (teleportButton != null)
            {
                teleportButton.onClick.AddListener(TeleportAndActivateComponents);
                Debug.Log("Teleport button listener added.");
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            GameObject playerObject = networkClient.PlayerObject.gameObject;
            players[clientId] = playerObject;
            Debug.Log($"Client with ID {clientId} added to players list.");
        }
    }

    public void TeleportAndActivateComponents()
    {
        if (IsHost)
        {
            int i = 0;
            foreach (var player in players)
            {
                ulong clientId = player.Key;
                GameObject playerObject = player.Value;

                // Determine teleport target position and rotation
                Transform targetTransform = teleportTargets[i % teleportTargets.Count];
                Vector3 targetPosition = targetTransform.position;
                Quaternion targetRotation = targetTransform.rotation;

                // Teleport and activate components via RPC
                TeleportPlayerServerRpc(clientId, targetPosition, targetRotation);

                i++;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TeleportPlayerServerRpc(ulong clientId, Vector3 targetPosition, Quaternion targetRotation)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            Debug.Log($"Teleporting player with Client ID: {clientId} to position {targetPosition} with rotation {targetRotation.eulerAngles}");
            GameObject playerObject = networkClient.PlayerObject.gameObject;
            TeleportAndActivateClientRpc(playerObject.GetComponent<NetworkObject>(), targetPosition, targetRotation);
        }
    }

    [ClientRpc]
    private void TeleportAndActivateClientRpc(NetworkObjectReference playerNetworkObjectRef, Vector3 targetPosition, Quaternion targetRotation)
    {
        if (playerNetworkObjectRef.TryGet(out NetworkObject playerNetworkObject))
        {
            playerNetworkObject.transform.position = targetPosition;
            playerNetworkObject.transform.rotation = targetRotation;
            Debug.Log($"Player teleported to position: {targetPosition} with rotation: {targetRotation.eulerAngles}");

            // Activate components on the client side
            ActivateComponents(playerNetworkObject.gameObject);
        }
    }

    // Method to activate components on the player object
    private void ActivateComponents(GameObject player)
    {
        MonoBehaviour[] components = player.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var component in components)
        {
            if (!component.enabled)
            {
                component.enabled = true;
                Debug.Log($"Activated component: {component.GetType().Name}");
            }
        }

        NetworkBehaviour[] networkBehaviours = player.GetComponentsInChildren<NetworkBehaviour>(true);
        foreach (var netBehaviour in networkBehaviours)
        {
            if (!netBehaviour.enabled)
            {
                netBehaviour.enabled = true;
                Debug.Log($"Activated NetworkBehaviour: {netBehaviour.GetType().Name}");
            }
        }

        AudioSource audioSource = player.GetComponentInChildren<AudioSource>(true);
        if (audioSource != null && !audioSource.enabled)
        {
            audioSource.enabled = true;
            Debug.Log("Activated AudioSource on player.");
        }

        Transform tankUITransform = player.transform.Find("TankUI");
        if (tankUITransform != null && !tankUITransform.gameObject.activeSelf)
        {
            tankUITransform.gameObject.SetActive(true);
            Debug.Log("Activated TankUI on player.");
        }
    }

    public override void OnDestroy()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;

            if (teleportButton != null)
            {
                teleportButton.onClick.RemoveListener(TeleportAndActivateComponents);
            }
        }

        base.OnDestroy();
    }
}
