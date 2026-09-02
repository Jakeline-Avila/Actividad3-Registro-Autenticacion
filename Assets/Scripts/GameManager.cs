using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private string url = "https://sid-restapi.onrender.com";

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private GameObject gameOverPanel;

    private int score = 0;
    private bool gameOver = false;
    private bool scoreSaved = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateScoreText();
    }

    public void AddScore()
    {
        if (gameOver)
            return;

        score++;

        Debug.Log("Puntaje: " + score);

        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;

        Debug.Log("GAME OVER");
        Debug.Log("Puntaje final: " + score);

        if (finalScoreText != null)
        {
            finalScoreText.text = "Puntaje final: " + score;
        }

        // Guardar el puntaje en el servidor
        if (!scoreSaved)
        {
            scoreSaved = true;
            StartCoroutine(UpdateScore());
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private IEnumerator UpdateScore()
    {
        string token = PlayerPrefs.GetString("token", "");
        string username = PlayerPrefs.GetString("username", "");

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(username))
        {
            Debug.LogError("No hay token o usuario para actualizar el score.");
            yield break;
        }

        // Primero obtenemos el perfil actual
        UnityWebRequest profileRequest =
            UnityWebRequest.Get(url + "/api/usuarios/" + username);

        profileRequest.SetRequestHeader("x-token", token);

        yield return profileRequest.SendWebRequest();

        if (profileRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error obteniendo el perfil: " + profileRequest.error);
            Debug.LogError(profileRequest.downloadHandler.text);
            yield break;
        }

        UserResponse profileResponse =
            JsonUtility.FromJson<UserResponse>(
                profileRequest.downloadHandler.text
            );

        int previousScore = 0;

        if (profileResponse.usuario.data != null)
        {
            previousScore = profileResponse.usuario.data.score;
        }

        Debug.Log("Mejor puntaje anterior: " + previousScore);
        Debug.Log("Puntaje de esta partida: " + score);

        // Si el nuevo puntaje NO supera el anterior, no hacemos PATCH
        if (score <= previousScore)
        {
            Debug.Log("El puntaje no supera el récord. Se conserva: " + previousScore);
            yield break;
        }

        // El nuevo puntaje es mayor, entonces lo actualizamos
        ScoreData scoreData = new ScoreData();

        scoreData.username = username;

        scoreData.data = new ScoreValues();
        scoreData.data.score = score;

        string jsonData = JsonUtility.ToJson(scoreData);

        Debug.Log("Nuevo récord. Enviando PATCH:");
        Debug.Log(jsonData);

        UnityWebRequest request =
            new UnityWebRequest(url + "/api/usuarios", "PATCH");

        byte[] bodyRaw =
            System.Text.Encoding.UTF8.GetBytes(jsonData);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-token", token);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error actualizando score: " + request.error);
            Debug.LogError(
                "Respuesta del servidor: " +
                request.downloadHandler.text
            );
        }
        else
        {
            Debug.Log("¡Nuevo récord guardado!");
            Debug.Log(
                "Respuesta PATCH: " +
                request.downloadHandler.text
            );
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}