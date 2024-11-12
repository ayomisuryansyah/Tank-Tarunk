using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ParticleManager : NetworkBehaviour
{
    public GameObject particlePrefab;  // Prefab dari Particle System yang akan di-spawn
    public Transform particleSlot;     // Tempat atau slot untuk spawn particle

    private GameObject currentParticleInstance; // Untuk menyimpan instance particle yang sedang aktif
    private TankStat tankStat;          // Referensi ke TankStat
    private float nextParticleTime;      // Waktu untuk trigger partikel berikutnya

    public float particleScale = 1f;     // Variabel untuk skala partikel

    void Start()
    {
        tankStat = GetComponent<TankStat>(); // Mendapatkan komponen TankStat
        if (tankStat == null)
        {
            Debug.LogError("TankStat component not found on this GameObject.");
        }
        
        // Initialize nextParticleTime
        nextParticleTime = 0f; 
    }

    void Update()
    {
        // Pastikan hanya owner yang bisa memicu partikel
        if (!IsOwner) return;

        // Jika tombol kiri mouse ditekan dan waktu sudah sesuai
        if (Input.GetMouseButtonDown(0) && Time.time >= nextParticleTime)
        {
            TriggerParticle();
            // Update the nextParticleTime berdasarkan fireSpeed
            nextParticleTime = Time.time + tankStat.fireSpeed;
        }
    }

    // Fungsi untuk memicu particle system di posisi slot
    void TriggerParticle()
    {
        // Hapus instance particle sebelumnya jika masih ada
        if (currentParticleInstance != null)
        {
            Destroy(currentParticleInstance);
        }

        // Spawn particle di posisi slot
        currentParticleInstance = Instantiate(particlePrefab, particleSlot.position, particleSlot.rotation);
        
        // Mengatur skala partikel
        currentParticleInstance.transform.localScale = Vector3.one * particleScale;

        // Opsional: Hapus instance particle setelah selesai
        var particleSystem = currentParticleInstance.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            Destroy(currentParticleInstance, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);
        }
    }
}
