using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah player yang mengambil power-up
        if (other.CompareTag("Player"))
        {
            // Lakukan aksi ketika power-up diambil
            Debug.Log("Power-up diambil!");

            // Hapus objek power-up dari scene
            Destroy(gameObject);
        }
    }
}
