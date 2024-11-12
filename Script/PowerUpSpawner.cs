using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PowerUpSpawner : NetworkBehaviour
{
    public List<GameObject> powerUpPrefabs; // List prefab power-up yang akan di-spawn
    public float spawnInterval = 5f; // Interval waktu antara spawn
    public float spawnCooldown = 10f; // Waktu cooldown sebelum spawn berikutnya
    public float spawnHeight = -5f; // Ketinggian relatif untuk spawn power-up dari posisi spawner
    public float targetHeight = 1f; // Ketinggian target di mana power-up akan berhenti bergerak
    public float moveSpeed = 1f; // Kecepatan bergerak power-up

    private GameObject currentPowerUp; // Menyimpan referensi power-up yang sedang aktif

    public override void OnNetworkSpawn()
    {
        if (IsServer) // Hanya server yang bisa menjalankan spawn
        {
            Debug.Log("Server started: Spawning coroutine initiated after network spawn.");
            StartCoroutine(SpawnPowerUp());
        }
        else
        {
            Debug.Log("Client mode - Spawning coroutine not initiated.");
        }
    }

    private IEnumerator SpawnPowerUp()
    {
        Debug.Log("Attempting to spawn power-up...");
        while (true)
        {
            if (currentPowerUp == null)
            {
                // Tentukan posisi spawn relatif terhadap posisi spawner
                Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + spawnHeight, transform.position.z);

                // Debug untuk memastikan spawnPosition memiliki ketinggian relatif yang benar
                Debug.Log($"Spawning power-up at relative position: {spawnPosition}");

                // Pilih prefab secara acak dari daftar
                GameObject selectedPowerUpPrefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Count)];

                // Spawn power-up di server
                GameObject powerUpInstance = Instantiate(selectedPowerUpPrefab, spawnPosition, Quaternion.identity);
                currentPowerUp = powerUpInstance;

                // Pastikan power-up memiliki NetworkObject agar bisa di-network
                NetworkObject networkObject = powerUpInstance.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    // Spawn power-up di semua klien
                    networkObject.Spawn(true);
                    Debug.Log("Power-up spawned successfully.");
                }
                else
                {
                    Debug.LogError("NetworkObject component missing on power-up prefab.");
                }

                // Mulai coroutine untuk memindahkan power-up
                StartCoroutine(MovePowerUp(powerUpInstance, transform.position.y + targetHeight));

                // Tunggu cooldown sebelum spawn berikutnya
                yield return new WaitForSeconds(spawnCooldown);
            }
            else
            {
                // Jika power-up masih ada, cek lagi setelah interval spawn
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    private IEnumerator MovePowerUp(GameObject powerUp, float targetHeight)
    {
        Vector3 initialPosition = powerUp.transform.position;
        Vector3 targetPosition = new Vector3(initialPosition.x, targetHeight, initialPosition.z);

        while (powerUp != null && powerUp.transform.position.y < targetHeight)
        {
            if (powerUp != null)
            {
                powerUp.transform.position = Vector3.MoveTowards(powerUp.transform.position, targetPosition, moveSpeed * Time.deltaTime);
                Debug.Log("Moving power-up to target height...");
            }
            yield return null;
        }

        if (powerUp != null)
        {
            powerUp.transform.position = targetPosition;
            Debug.Log("Power-up reached target height.");
        }
    }
}
