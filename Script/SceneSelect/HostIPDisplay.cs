using UnityEngine;
using Unity.Netcode;
using System.Net;
using UnityEngine.UI;
using TMPro;

public class HostIPDisplay : NetworkBehaviour
{
    [SerializeField] private TMP_Text ipText;        // Referensi untuk TMP teks IP di Inspector
    [SerializeField] private Button hideButton;      // Referensi untuk tombol di Inspector

    private string localIP = "";

    public override void OnNetworkSpawn()
    {
        // Hanya host yang menjalankan kode ini
        if (IsHost)
        {
            // Dapatkan IP lokal dan tampilkan di TMP teks
            localIP = GetLocalIPAddress();
            ipText.text = "Host IP: " + localIP;
            ipText.gameObject.SetActive(true); // Tampilkan teks IP
            hideButton.gameObject.SetActive(true); // Tampilkan tombol
            Debug.Log("Host IP: " + localIP);

            // Tambahkan listener ke tombol untuk menyembunyikan UI saat ditekan
            hideButton.onClick.AddListener(HideUI);
        }
        else
        {
            // Jika bukan host, sembunyikan teks IP dan tombol
            ipText.gameObject.SetActive(false);
            hideButton.gameObject.SetActive(false);
        }
    }

    // Fungsi untuk mendapatkan IP lokal
    private string GetLocalIPAddress()
    {
        string ipAddress = "IP Not Found";

        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ipAddress = ip.ToString();
                break;
            }
        }
        return ipAddress;
    }

    // Fungsi untuk menyembunyikan UI (teks IP dan tombol)
    private void HideUI()
    {
        ipText.gameObject.SetActive(false);
        hideButton.gameObject.SetActive(false);
    }
}
