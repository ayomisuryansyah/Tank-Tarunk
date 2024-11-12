using UnityEngine;
using TMPro; // Pastikan ini ditambahkan untuk TMP
using System.Net;
using System.Net.Sockets;

public class HostNetworkIP : MonoBehaviour
{
    [SerializeField] private TMP_Text ipAddressText; // Ganti Text menjadi TMP_Text

    private void Start()
    {
        string localIP = GetLocalIPAddress();

        if (ipAddressText != null)
        {
            ipAddressText.text = "Your IP: " + localIP;
        }
        else
        {
            Debug.LogWarning("ipAddressText belum dihubungkan di Inspector.");
        }
    }

    private string GetLocalIPAddress()
    {
        string ipAddress = "IP Not Found";
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ipAddress = ip.ToString();
                break;
            }
        }
        return ipAddress;
    }
}
