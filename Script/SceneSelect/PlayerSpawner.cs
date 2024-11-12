using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints; // Titik spawn di Lobi

    private void Start()
    {
        // Pastikan callback untuk memanggil SpawnPlayer hanya dilakukan di Lobi
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
    }

    private void HandleClientConnected(ulong clientId)
    {
        StartCoroutine(SpawnPlayerCoroutine(clientId));
    }

    private IEnumerator SpawnPlayerCoroutine(ulong clientId)
    {
        // Tunggu hingga NetworkManager siap (contoh dengan sedikit delay)
        yield return new WaitForSeconds(0.5f); 

        if (NetworkManager.Singleton.ConnectedClients.Count <= spawnPoints.Length)
        {
            int spawnIndex = (int)(clientId % (ulong)spawnPoints.Length); // Tentukan spawn point berdasarkan clientId
            Vector3 spawnPosition = spawnPoints[spawnIndex].position;

            // Pastikan prefab Player berada di Resources dan memiliki NetworkObject
            GameObject playerPrefab = Instantiate(Resources.Load<GameObject>("Player"), spawnPosition, Quaternion.identity);
            
            // Pastikan objek di-spawn di network
            NetworkObject networkObject = playerPrefab.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.SpawnAsPlayerObject(clientId); // Spawn untuk pemain tertentu
            }
            else
            {
                Debug.LogError("Prefab Player tidak memiliki komponen NetworkObject.");
            }
        }
        else
        {
            Debug.LogWarning("Titik spawn penuh, pemain tidak dapat ditambahkan.");
        }
    }
}
