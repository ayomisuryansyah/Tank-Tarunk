using UnityEngine;

public class PowerUpBehaviour : MonoBehaviour
{
    public float moveDistance = 1f; // Jarak naik turun
    public float moveSpeed = 2f; // Kecepatan gerakan naik turun
    public float rotateSpeed = 50f; // Kecepatan rotasi

    private float originalY; // Simpan nilai Y awal untuk naik turun

    void Start()
    {
        // Simpan posisi Y awal objek
        originalY = transform.localPosition.y;
    }

    void Update()
    {
        // Hitung waktu untuk membuat gerakan naik turun
        float newY = originalY + Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        // Update posisi lokal objek
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);

        // Putar objek di sekitar sumbu lokal Y
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self);
    }
}
