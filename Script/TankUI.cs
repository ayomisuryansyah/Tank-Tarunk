using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TankUI : MonoBehaviour
{
    public TankStat tankStat; // Referensi ke komponen TankStat
    public Slider hpSlider;    // Referensi ke UI Slider

    private void Start()
    {
        if (tankStat == null || hpSlider == null)
        {
            Debug.LogError("Referensi TankStat atau Slider tidak diatur!");
            return;
        }

        // Pastikan hanya memperbarui UI untuk tank yang dimiliki oleh pemain ini
        if (!tankStat.GetComponent<NetworkObject>().IsOwner)
        {
            hpSlider.gameObject.SetActive(false); // Matikan UI untuk pemain lain
            return;
        }

        // Inisialisasi slider dengan nilai HP awal
        hpSlider.maxValue = tankStat.hp; 
        hpSlider.value = tankStat.GetCurrentHP();
    }

    private void Update()
    {
        // Update slider setiap frame
        if (tankStat.GetComponent<NetworkObject>().IsOwner)
        {
            hpSlider.value = tankStat.GetCurrentHP();
        }
    }
}

