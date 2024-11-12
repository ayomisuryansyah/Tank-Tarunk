using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Fungsi untuk berpindah ke scene Tank_Test
    public void PlayGame()
    {
        SceneManager.LoadScene("Tank_Test");
    }

    // Fungsi untuk keluar dari aplikasi
    public void ExitGame()
    {
        // Fungsi ini hanya berfungsi di aplikasi build, tidak di editor
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
