using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text welcomeText;

    private void Start()
    {
        string username = PlayerPrefs.GetString("username", "");

        if (string.IsNullOrEmpty(username))
        {
            SceneManager.LoadScene("Login");
            return;
        }

        welcomeText.text = "Bienvenido, " + username;
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.Save();

        SceneManager.LoadScene("Login");
    }

    public void OpenLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }
}