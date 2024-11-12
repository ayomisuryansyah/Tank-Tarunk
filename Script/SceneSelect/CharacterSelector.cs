using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelector : MonoBehaviour
{
    public GameObject[] characters; // Array untuk karakter yang bisa dipilih
    private int selectedCharacterIndex = 0; // Index karakter yang dipilih

    private void Start()
    {
        UpdateCharacterSelection();
    }

    public void SelectNextCharacter()
    {
        selectedCharacterIndex = (selectedCharacterIndex + 1) % characters.Length;
        UpdateCharacterSelection();
    }

    public void SelectPreviousCharacter()
    {
        selectedCharacterIndex = (selectedCharacterIndex - 1 + characters.Length) % characters.Length;
        UpdateCharacterSelection();
    }

    private void UpdateCharacterSelection()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == selectedCharacterIndex);
        }
    }

    public void ConfirmSelection()
    {
        PlayerPrefs.SetInt("SelectedCharacter", selectedCharacterIndex);
        SceneManager.LoadScene("GameScene"); // Ganti "GameScene" dengan nama scene berikutnya
    }
}
