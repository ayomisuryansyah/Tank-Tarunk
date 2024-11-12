using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // Objek yang akan dihasilkan
    public float spawnInterval = 2.0f; // Interval waktu antara spawn
    public float spawnForce = 500.0f; // Gaya yang diterapkan pada objek yang dihasilkan

    private void Start()
    {
        // Mulai coroutine untuk spawn objek
        InvokeRepeating(nameof(SpawnObject), 0, spawnInterval);
    }

    private void SpawnObject()
    {
        // Buat objek baru pada posisi spawner
        GameObject spawnedObject = Instantiate(objectToSpawn, transform.position, transform.rotation);

        // Dapatkan komponen Rigidbody dari objek yang dihasilkan
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();

        // Jika objek memiliki Rigidbody, terapkan gaya padanya
        if (rb != null)
        {
            rb.AddForce(transform.forward * spawnForce); // Terapkan gaya ke arah depan spawner
        }
    }
}
