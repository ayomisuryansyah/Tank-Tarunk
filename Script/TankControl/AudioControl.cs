using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public AudioSource idleAudioSource;     // Sumber Audio untuk idle
    public AudioClip clickAudioClip;        // AudioClip untuk klik kiri
    public float pitchIncreaseAmount = 0.5f; // Nilai peningkatan pitch ketika tombol W, A, S, atau D ditekan
    public float maxPitch = 2.0f;           // Batas atas pitch audio idle

    private AudioSource clickAudioSource;   // Sumber Audio untuk klik kiri
    private bool isClickAudioPlaying = false; // Mengecek apakah audio klik kiri sedang dimainkan
    private TankStat tankStat;              // Reference to TankStat

    void Start()
    {
        // Memulai audio idle saat game dimulai
        idleAudioSource.Play();

        // Buat AudioSource terpisah untuk suara klik kiri
        clickAudioSource = gameObject.AddComponent<AudioSource>();
        clickAudioSource.clip = clickAudioClip;

        // Get TankStat component
        tankStat = GetComponent<TankStat>();
        if (tankStat == null)
        {
            Debug.LogError("TankStat component not found on this GameObject.");
        }
    }

    void Update()
    {
        // Mengatur pitch dari idleAudioSource berdasarkan input
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            // Tingkatkan pitch selama tombol W, A, S, atau D ditekan
            idleAudioSource.pitch = Mathf.Clamp(idleAudioSource.pitch + pitchIncreaseAmount * Time.deltaTime, 1.0f, maxPitch);
        }
        else
        {
            // Kembalikan pitch ke 1.0 (default) saat tidak ada tombol yang ditekan
            idleAudioSource.pitch = Mathf.Lerp(idleAudioSource.pitch, 1.0f, Time.deltaTime * 2.0f);
        }

        // Memainkan audio ketika klik kiri ditekan
        if (Input.GetMouseButtonDown(0) && !isClickAudioPlaying)
        {
            PlayClickAudio();
        }
    }

    // Method untuk memainkan suara klik kiri dengan durasi tertentu
    void PlayClickAudio()
    {
        // Mulai memainkan audio klik kiri
        clickAudioSource.Play();
        isClickAudioPlaying = true;

        // Gunakan fireSpeed dari TankStat sebagai durasi audio
        float clickAudioDuration = tankStat.fireSpeed;

        // Jalankan coroutine untuk menghentikan audio setelah durasi yang diatur
        StartCoroutine(StopClickAudioAfterDuration(clickAudioDuration));
    }

    // Coroutine untuk menghentikan audio setelah durasi yang ditentukan
    IEnumerator StopClickAudioAfterDuration(float duration)
    {
        // Tunggu selama durasi yang diatur
        yield return new WaitForSeconds(duration);

        // Hentikan audio klik kiri dan reset status
        clickAudioSource.Stop();
        isClickAudioPlaying = false;
    }
}
