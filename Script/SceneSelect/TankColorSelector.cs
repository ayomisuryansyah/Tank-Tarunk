using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TankColorSelector : MonoBehaviour
{
    // Referensi gambar warna di UI
    [SerializeField] private Image whiteImg;
    [SerializeField] private Image yellowImg;
    [SerializeField] private Image blueImg;
    [SerializeField] private Image redImg;

    // Referensi prefab untuk masing-masing warna
    [SerializeField] private GameObject whiteTankPrefab;
    [SerializeField] private GameObject yellowTankPrefab;
    [SerializeField] private GameObject blueTankPrefab;
    [SerializeField] private GameObject redTankPrefab;

    // Titik spawn utama untuk menampilkan tank
    [SerializeField] private Transform spawnPoint;

    private GameObject selectedPrefab;

    private void Start()
    {
        // Menambahkan event listener untuk setiap Image warna
        AddClickListener(whiteImg, () => SelectAndSpawnTank(whiteTankPrefab));
        AddClickListener(yellowImg, () => SelectAndSpawnTank(yellowTankPrefab));
        AddClickListener(blueImg, () => SelectAndSpawnTank(blueTankPrefab));
        AddClickListener(redImg, () => SelectAndSpawnTank(redTankPrefab));
    }

    private void SelectAndSpawnTank(GameObject prefab)
    {
        if (prefab != null && spawnPoint != null)
        {
            Debug.Log("Spawning tank of color: " + prefab.name);

            // Spawn tank di posisi spawnPoint dengan rotasi spawnPoint
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Prefab or spawn point is not assigned.");
        }
    }

    // Fungsi untuk menambahkan event klik pada Image
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
}
