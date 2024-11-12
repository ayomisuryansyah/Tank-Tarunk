using UnityEngine;

public class MatchCameraDirection : MonoBehaviour
{
    public GameObject objectX; // Objek yang akan mengikuti arah kamera pada sumbu X
    public GameObject objectY; // Objek yang akan mengikuti arah kamera pada sumbu Y

    private Camera targetCamera; // Kamera yang menjadi referensi (Main Camera)

    void Start()
    {
        // Mengambil Main Camera
        targetCamera = Camera.main;

        // Pengecekan jika Main Camera tidak ditemukan
        if (targetCamera == null)
        {
            Debug.LogError("Main Camera tidak ditemukan. Pastikan ada kamera yang ditandai sebagai 'MainCamera'.");
        }
    }

    void Update()
    {
        if (targetCamera == null) return;

        // Mengatur objectX agar mengikuti arah rotasi kamera hanya pada sumbu X
        if (objectX != null)
        {
            Quaternion rotationX = Quaternion.Euler(targetCamera.transform.eulerAngles.x, objectX.transform.eulerAngles.y, objectX.transform.eulerAngles.z);
            objectX.transform.rotation = rotationX;
        }

        // Mengatur objectY agar mengikuti arah rotasi kamera hanya pada sumbu Y
        if (objectY != null)
        {
            Quaternion rotationY = Quaternion.Euler(objectY.transform.eulerAngles.x, targetCamera.transform.eulerAngles.y, objectY.transform.eulerAngles.z);
            objectY.transform.rotation = rotationY;
        }
    }
}
