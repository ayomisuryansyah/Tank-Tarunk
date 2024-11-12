using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement; // Tambahkan ini untuk mengakses SceneManager

public class Shooting : NetworkBehaviour
{
    public GameObject projectilePrefab; // prefab peluru
    public Transform muzzle; // posisi GameObject tempat peluru keluar (muzzle)
    private Camera playerCamera; // kamera pemain (akan diambil dari Main Camera atau kamera Cinemachine)
    private TankStat tankStat; // Referensi ke TankStat
    private float lastShotTime; // Waktu terakhir tembakan dilakukan

    void Start()
    {
        // Mendaftarkan metode ke event OnSceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Ambil kamera saat start
        FindPlayerCamera();
        
        // Mendapatkan referensi ke komponen TankStat
        tankStat = GetComponent<TankStat>();

        // Menyembunyikan kursor dan mengunci kursor di tengah layar
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnDestroy()
    {
        // Unregister event OnSceneLoaded saat object dihancurkan untuk menghindari error
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Panggil OnDestroy() bawaan dari NetworkBehaviour
        base.OnDestroy();
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Panggil metode untuk mencari kamera setiap kali scene di-load
        FindPlayerCamera();
    }

    void FindPlayerCamera()
    {
        // Coba dapatkan kamera utama
        playerCamera = Camera.main;

        // Jika Main Camera masih tidak ditemukan, coba cari kamera yang mungkin dikontrol oleh Cinemachine
        if (playerCamera == null)
        {
            playerCamera = GameObject.FindObjectOfType<Camera>(); // Cari object kamera lain di scene
        }

        if (playerCamera == null)
        {
            Debug.LogWarning("Main Camera tidak ditemukan di scene. Pastikan ada kamera yang sesuai.");
        }
    }

    void Update()
    {
        // Perbarui referensi kamera jika null
        if (playerCamera == null)
        {
            FindPlayerCamera();
            if (playerCamera == null)
            {
                Debug.LogWarning("Main Camera masih tidak ditemukan.");
                return; // Hentikan eksekusi Update jika kamera belum ditemukan
            }
        }

        // Pastikan hanya owner yang bisa menembak
        if (IsOwner && Input.GetButtonDown("Fire1") && Time.time >= lastShotTime + tankStat.fireSpeed)
        {
            // Memanggil ServerRpc untuk memastikan peluru diluncurkan dari server
            ShootServerRpc(playerCamera.transform.position, playerCamera.transform.forward);
            lastShotTime = Time.time;
        }

        // Kembali mengaktifkan kursor jika diperlukan
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    [ServerRpc]
    void ShootServerRpc(Vector3 cameraPosition, Vector3 cameraForward)
    {
        // Hitung target berdasarkan posisi dan arah kamera yang dikirim dari client
        Ray ray = new Ray(cameraPosition, cameraForward);
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.collider.CompareTag("Tank") ? ray.GetPoint(1000) : hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000);
        }

        // Membuat peluru di posisi muzzle dan arahkan ke target
        GameObject projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        projectile.GetComponent<NetworkObject>().Spawn(); // Spawn peluru di semua client

        // Tambahkan gaya ke peluru untuk meluncurkannya ke arah target
        Vector3 direction = (targetPoint - muzzle.position).normalized;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(direction * tankStat.bulletspeed, ForceMode.Impulse);
    }
}
