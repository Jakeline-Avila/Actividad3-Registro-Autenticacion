using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{
    private string url = "https://sid-restapi.onrender.com";

    [SerializeField] private TMP_Text leaderboardText;

    private void Start()
    {
        StartCoroutine(GetLeaderboard());
    }

    private IEnumerator GetLeaderboard()
    {
        string token = PlayerPrefs.GetString("token", "");

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("No hay token de autenticación.");
            leaderboardText.text = "Error de autenticación.";
            yield break;
        }

        string requestUrl =
            url + "/api/usuarios?limit=100&skip=0";

        UnityWebRequest request =
            UnityWebRequest.Get(requestUrl);

        request.SetRequestHeader("x-token", token);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error obteniendo usuarios: " + request.error);
            Debug.LogError("Respuesta: " + request.downloadHandler.text);

            leaderboardText.text = "No se pudo cargar el ranking.";
            yield break;
        }

        Debug.Log("Respuesta usuarios:");
        Debug.Log(request.downloadHandler.text);

        UsersResponse response =
            JsonUtility.FromJson<UsersResponse>(
                request.downloadHandler.text
            );

        List<UserData> users =
            new List<UserData>(response.usuarios);

        // Ordenar de mayor a menor puntaje
        users.Sort((a, b) =>
            b.data.score.CompareTo(a.data.score)
        );

        string ranking = "";

        int position = 1;

        foreach (UserData user in users)
        {
            ranking += position + ". "
                + user.username
                + " - "
                + user.data.score
                + "\n";

            position++;
        }

        if (string.IsNullOrEmpty(ranking))
        {
            ranking = "No hay jugadores registrados.";
        }

        leaderboardText.text = ranking;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

[System.Serializable]
public class UsersResponse
{
    public UserData[] usuarios;
}