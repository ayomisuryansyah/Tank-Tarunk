using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuDefault : MonoBehaviour
{
    [System.Serializable]
    public class ButtonObjectPair
    {
        public Button button;      // Tombol UI yang akan diklik
        public GameObject targetOn;  // GameObject yang akan diaktifkan
        public GameObject targetOff; // GameObject yang akan dimatikan
    }

    public List<ButtonObjectPair> buttonObjectPairs = new List<ButtonObjectPair>();

    private void Start()
    {
        // Tambahkan listener untuk setiap tombol dalam daftar
        foreach (var pair in buttonObjectPairs)
        {
            pair.button.onClick.AddListener(() => ToggleObjects(pair));
        }
    }

    private void ToggleObjects(ButtonObjectPair pair)
    {
        if (pair.targetOff != null)
        {
            pair.targetOff.SetActive(false); // Matikan objek targetOff
        }

        if (pair.targetOn != null)
        {
            pair.targetOn.SetActive(true); // Aktifkan objek targetOn
        }
    }

    // Fungsi untuk menambahkan tombol dan objek secara dinamis
    public void AddButtonObjectPair(Button button, GameObject targetOn, GameObject targetOff)
    {
        ButtonObjectPair newPair = new ButtonObjectPair
        {
            button = button,
            targetOn = targetOn,
            targetOff = targetOff
        };

        buttonObjectPairs.Add(newPair);

        // Tambahkan listener untuk tombol yang baru ditambahkan
        newPair.button.onClick.AddListener(() => ToggleObjects(newPair));
    }
}
