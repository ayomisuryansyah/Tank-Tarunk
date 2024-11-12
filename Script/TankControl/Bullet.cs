using System.Collections; // Tambahkan ini untuk menggunakan IEnumerator
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    public float damage; // Damage yang akan di-set oleh TankControl ketika peluru ditembakkan
    public GameObject particlePrefab; // Prefab partikel yang akan dipasang
    public AudioClip soundEffect; // Efek suara yang akan dimainkan
    public float fadeOutDuration = 0.5f; // Durasi fade out
    private AudioSource audioSource; // Komponen AudioSource
    private Rigidbody rb; // Komponen Rigidbody
    private Collider bulletCollider; // Komponen Collider
    private MeshRenderer meshRenderer; // Komponen MeshRenderer

    // Array untuk menyimpan objek yang akan diabaikan
    public GameObject[] ignoreObjects;

    private void Start()
    {
        // Mendapatkan komponen AudioSource dari objek ini
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundEffect; // Mengatur efek suara
        audioSource.playOnAwake = false; // Menonaktifkan pemutaran otomatis saat objek dibuat

        // Mendapatkan komponen Rigidbody
        rb = GetComponent<Rigidbody>();
        
        // Mendapatkan komponen Collider
        bulletCollider = GetComponent<Collider>();

        // Mendapatkan komponen MeshRenderer
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Mengecek apakah objek yang ditabrak ada di dalam array ignoreObjects
        foreach (GameObject ignoreObject in ignoreObjects)
        {
            if (collision.gameObject == ignoreObject)
            {
                // Jika objek diabaikan, maka tidak melakukan apa-apa
                return;
            }
        }

        // Spawn partikel di lokasi tabrakan pada semua client
        SpawnParticleClientRpc(collision.contacts[0].point);
        
        // Mainkan suara sebelum menghancurkan peluru
        PlaySound();

        // Hentikan pergerakan peluru dengan menonaktifkan Rigidbody
        if (rb != null)
        {
            rb.isKinematic = true; // Menonaktifkan fisika
            rb.velocity = Vector3.zero; // Mengatur kecepatan menjadi nol
        }

        // Nonaktifkan collider
        if (bulletCollider != null)
        {
            bulletCollider.enabled = false; // Nonaktifkan collider
        }

        // Nonaktifkan mesh renderer
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false; // Nonaktifkan mesh renderer
        }

        // Mulai fade out dan menghancurkan objek setelah delay
        StartCoroutine(FadeOutAndDestroy(fadeOutDuration)); // Menghancurkan objek dengan fade out
    }

    private IEnumerator FadeOutAndDestroy(float duration)
    {
        float startVolume = audioSource.volume;

        // Fade out suara
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration); // Mengurangi volume
            yield return null; // Tunggu satu frame
        }

        audioSource.volume = 0; // Pastikan volume menjadi 0
        Destroy(gameObject); // Hancurkan objek setelah fade out
    }

    [ClientRpc]
    private void SpawnParticleClientRpc(Vector3 position)
    {
        // Instantiate partikel di posisi yang diberikan untuk semua client
        if (particlePrefab != null)
        {
            Instantiate(particlePrefab, position, Quaternion.identity);
        }
    }

    private void PlaySound()
    {
        if (soundEffect != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
    }
}
