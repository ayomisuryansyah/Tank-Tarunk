using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class MultiplayerCamera : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;

    private void Start()
    {
        // Cek apakah FreeLook Camera sudah diatur, jika belum, assign kamera yang ada di scene
        if (freeLookCamera == null)
            freeLookCamera = GetComponent<CinemachineFreeLook>();

        // Panggil metode untuk mengatur target setelah player di-spawn
        Invoke("SetCameraTarget", 1.0f); // Tambahkan jeda untuk memastikan player sudah di-spawn
    }

    void SetCameraTarget()
    {
        // Cari semua pemain di scene
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            // Cek apakah objek ini adalah player lokal
            if (player.GetComponent<NetworkObject>().IsOwner)
            {
                // Cari Camera Pivot di dalam prefab player
                Transform cameraPivot = player.transform.Find("Camera_Pivot");

                if (cameraPivot != null)
                {
                    // Set Follow ke player dan LookAt ke Camera Pivot
                    freeLookCamera.Follow = player.transform;
                    freeLookCamera.LookAt = cameraPivot;
                }
                else
                {
                    Debug.LogWarning("Camera Pivot tidak ditemukan di dalam prefab Player. Pastikan objek 'Camera Pivot' ada di dalam player prefab.");
                }
                break; // Hentikan loop setelah menemukan player lokal
            }
        }
    }
}
