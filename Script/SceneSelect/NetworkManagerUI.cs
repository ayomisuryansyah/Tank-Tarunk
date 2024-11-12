using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using TMPro;
using System.Net;
using UnityEngine.EventSystems;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Image hostImg;
    [SerializeField] private Image clientImg;
    [SerializeField] private TMP_Text hostIPText;
    [SerializeField] private TMP_InputField clientIPInput;

    [SerializeField] private Image whiteImg1;
    [SerializeField] private Image whiteImg2;
    [SerializeField] private Image redImg1;
    [SerializeField] private Image redImg2;
    [SerializeField] private Image blueImg1;
    [SerializeField] private Image blueImg2;
    [SerializeField] private Image yellowImg1;
    [SerializeField] private Image yellowImg2;

    private GameObject selectedPrefab;

    // Different spawn positions for connected players
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(26, 6, -25),
        new Vector3(8, 6, -25),
        new Vector3(-8, 6, -25),
        new Vector3(-26, 6, -25)
    };

    private void Awake() 
    {
        SelectPlayerPrefab("WhiteTank");

        AddClickListener(hostImg, StartHost);
        AddClickListener(clientImg, StartClient);

        AddClickListener(whiteImg1, () => SelectPlayerPrefab("WhiteTank"));
        AddClickListener(whiteImg2, () => SelectPlayerPrefab("WhiteTank"));

        AddClickListener(redImg1, () => SelectPlayerPrefab("RedTank"));
        AddClickListener(redImg2, () => SelectPlayerPrefab("RedTank"));

        AddClickListener(blueImg1, () => SelectPlayerPrefab("BlueTank"));
        AddClickListener(blueImg2, () => SelectPlayerPrefab("BlueTank"));

        AddClickListener(yellowImg1, () => SelectPlayerPrefab("YellowTank"));
        AddClickListener(yellowImg2, () => SelectPlayerPrefab("YellowTank"));

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void StartHost()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host has started successfully.");

            string localIP = GetLocalIPAddress();
            hostIPText.text = "Host IP: " + localIP;

            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("Failed to start host.");
        }
    }

    private void StartClient()
    {
        if (clientIPInput.text != "")
        {
            var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetConnectionData(clientIPInput.text, 7777);

            Debug.Log("Attempting to connect to IP: " + clientIPInput.text + " on port 7777");

            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("Client has started successfully.");
                StartCoroutine(CheckConnectionStatus());
            }
            else
            {
                Debug.LogError("Failed to start client.");
            }
        }
        else
        {
            Debug.LogWarning("Please enter the host IP address.");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // Only the server should spawn the player
            SpawnPlayer(clientId, NetworkManager.Singleton.ConnectedClientsIds.Count - 1);
        }
        else
        {
            Debug.Log("Client connected with ID: " + clientId);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.LogWarning("Local client failed to connect or was disconnected from host.");
        }
        else
        {
            Debug.Log("Another client disconnected with ID: " + clientId);
        }
    }

    private IEnumerator CheckConnectionStatus()
    {
        yield return new WaitForSeconds(5f);

        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.LogWarning("Failed to connect to the host. Please check the IP address and try again.");
        }
        else
        {
            Debug.Log("Client is connected to the host.");
        }
    }

    private void SelectPlayerPrefab(string prefabName)
    {
        selectedPrefab = Resources.Load<GameObject>("Prefabs/" + prefabName);

        if (selectedPrefab != null)
        {
            Debug.Log("Player prefab selected: " + selectedPrefab.name);
        }
        else
        {
            Debug.LogWarning("Prefab with name " + prefabName + " not found in Resources/Prefabs.");
        }
    }

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "IP Not Found";
    }

    private void AddClickListener(Image img, System.Action action)
    {
        EventTrigger trigger = img.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { action(); });
        trigger.triggers.Add(entry);
    }

    // Server-only method to spawn players
    private void SpawnPlayer(ulong clientId, int playerIndex)
    {
        if (selectedPrefab != null && NetworkManager.Singleton.IsServer)
        {
            // Calculate the position and rotation for the new player instance
            Vector3 position = spawnPositions[playerIndex % spawnPositions.Length];
            Quaternion rotation = Quaternion.identity; // Set to identity or any specific rotation

            // Instantiate the player prefab without initial position and rotation
            GameObject playerInstance = Instantiate(selectedPrefab);

            // Set the transform properties explicitly after instantiation
            playerInstance.transform.position = position;
            playerInstance.transform.rotation = rotation;

            // Ensure the instantiated object has a NetworkObject component
            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                // Spawn the player as a networked object for the specific client
                networkObject.SpawnAsPlayerObject(clientId);
                Debug.Log($"Player spawned at position: {position} with rotation: {rotation.eulerAngles}");
            }
            else
            {
                Debug.LogWarning("NetworkObject not found on the selected prefab.");
                Destroy(playerInstance); // Clean up if it lacks a NetworkObject
            }
        }
    }
}
