using UnityEngine;

public class TankStat : MonoBehaviour
{
    // Statistik dasar tank
    public float movementSpeed = 5.0f; // Kecepatan gerak tank
    public float fireSpeed = 1.0f;     // Kecepatan menembak (waktu antara tembakan)
    public float fireDamage = 10.0f;   // Damage dari peluru
    public float hp = 100.0f;          // Hit points atau health tank
    public float bulletspeed = 200;

    // Status untuk power-up speed boost
    public bool isSpeedBoosted = false; // Tambahkan ini

    // Fungsi untuk mengambil damage
    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            // Jika HP habis, hancurkan tank atau lakukan hal lain
            DestroyTank();
        }
    }

    // Fungsi untuk menghancurkan tank
    private void DestroyTank()
    {
        // Logika untuk menghancurkan atau menghilangkan tank
        Debug.Log("Tank Destroyed!");
        Destroy(gameObject); // Menghancurkan game object tank
    }

    // Fungsi ini dapat dipanggil oleh kode lain untuk menambah HP
    public void Heal(float amount)
    {
        hp += amount;
    }

    // Fungsi ini untuk mendapatkan nilai current HP
    public float GetCurrentHP()
    {
        return hp;
    }

    // Fungsi untuk mendeteksi tabrakan dengan peluru
    private void OnCollisionEnter(Collision collision)
    {
        // Jika objek yang bertabrakan memiliki tag "Bullet"
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Kurangi HP sebesar 20
            TakeDamage(20.0f);
            Debug.Log("Menerima Damage");
            // Bisa juga hancurkan peluru jika perlu
            //Destroy(collision.gameObject); // Menghancurkan peluru jika diperlukan
        }
    }
}
