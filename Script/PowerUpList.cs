using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpList : MonoBehaviour
{
    // Variabel publik untuk speed boost
    public float speedBoostMultiplier = 2.0f; // Multiplier untuk speed boost
    public float speedBoostDuration = 10.0f; // Durasi speed boost
    public float fireBoostMultiplier = 2.0f; // Multiplier untuk speed boost
    public float fireBoostDuration = 10.0f; // Durasi speed boost

    public float attackBoostMultiplier = 2.0f; // Multiplier untuk speed boost
    public float attackBoostDuration = 10.0f; // Durasi speed boost

    public float HPBoost = 50;

    // Referensi ke komponen TankStat
    private TankStat tankStat;

    private void Start()
    {
        // Ambil komponen TankStat dari objek ini
        tankStat = GetComponent<TankStat>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah objek yang bersentuhan adalah tank itu sendiri
        if (other.CompareTag("SpeedBoost"))
        {
            StartCoroutine(ApplySpeedBoost());
        }

        else if (other.CompareTag("FireBoost"))
        {
            StartCoroutine(ApplyFireBoost());
        }
        else if (other.CompareTag("HPBoost"))
        {
            ApplyHPBoost();
        }
        else if (other.CompareTag("AttackBoost"))
        {
            ApplyAttackBoost();
        }
    }

    private IEnumerator ApplySpeedBoost()
    {
        if (tankStat != null)
        {
            // Kalikan movement speed dengan multiplier
            tankStat.movementSpeed *= speedBoostMultiplier;
            Debug.Log("Speed Boost Activated!");

            // Tunggu selama speedBoostDuration detik
            yield return new WaitForSeconds(speedBoostDuration);

            // Kembalikan movement speed ke semula
            tankStat.movementSpeed /= speedBoostMultiplier;
            Debug.Log("Speed Boost Deactivated!");
            
        }  
    }

    private IEnumerator ApplyFireBoost()
    {
        if (tankStat != null)
        {
            // Kalikan movement speed dengan multiplier
            tankStat.fireSpeed /= fireBoostMultiplier;
            Debug.Log("Fire Boost Activated!");

            // Tunggu selama speedBoostDuration detik
            yield return new WaitForSeconds(fireBoostDuration);

            // Kembalikan movement speed ke semula
            tankStat.fireSpeed *= fireBoostMultiplier;
            Debug.Log("Fire Boost Deactivated!");
            
        }  
    }

     private void ApplyHPBoost()
    {
        if (tankStat != null)
        {
            // Hitung HP maksimal tank
            float maxHP = 100f; // Ambil nilai HP maksimal dari tankStat
            // Tambahkan HP dan pastikan tidak melebihi maxHP
            tankStat.hp = Mathf.Min(tankStat.hp + HPBoost, maxHP);
            Debug.Log("HP Boost Activated! Current HP: " + tankStat.hp);
            
        }
    }

    private IEnumerator ApplyAttackBoost()
    {
        if (tankStat != null)
        {

            tankStat.fireDamage *= attackBoostMultiplier;
            Debug.Log("Attack Boost Activated!");

            yield return new WaitForSeconds(speedBoostDuration);

            tankStat.fireDamage /= attackBoostMultiplier;
            Debug.Log("Attack Boost Activated!");
            
        }  
    }
}
