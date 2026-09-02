using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Authmanager : MonoBehaviour
{
    private string url = "https://sid-restapi.onrender.com";
    private string token = "";
    private string username = "";

    [Header("Login")]
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_Text statusText;

    private void Start()
    {
        token = PlayerPrefs.GetString("token", "");
        username = PlayerPrefs.GetString("username", "");

        if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(username))
        {
            Debug.Log("Sesión encontrada: " + username);
            StartCoroutine(GetProfile());
        }
    }

    // =========================
    // VALIDAR TOKEN
    // =========================

    private IEnumerator GetProfile()
    {
        Debug.Log("Validando token...");

        UnityWebRequest request =
            UnityWebRequest.Get(url + "/api/usuarios/" + username);

        request.SetRequestHeader("x-token", token);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error de autenticación: " + request.error);
            Debug.LogError(request.downloadHandler.text);

            PlayerPrefs.DeleteKey("token");
            PlayerPrefs.DeleteKey("username");
            PlayerPrefs.Save();

            token = "";
            username = "";

            SetStatus("Sesión expirada. Inicia sesión nuevamente.");
            yield break;
        }

        Debug.Log("Token válido.");
        Debug.Log("Perfil: " + request.downloadHandler.text);

        SetStatus("Sesión válida: " + username);

        // Ir al menú después de validar el token
        SceneManager.LoadScene("Menu");
    }

    // =========================
    // LOGIN
    // =========================

    public void LoginButtonClick()
    {
        StartCoroutine(Login());
    }

    private IEnumerator Login()
    {
        AuthData authData = new AuthData();

        authData.username = usernameField.text;
        authData.password = passwordField.text;

        string jsonData = JsonUtility.ToJson(authData);

        Debug.Log("Enviando Login: " + jsonData);

        UnityWebRequest request = UnityWebRequest.Post(
            url + "/api/auth/login",
            jsonData,
            "application/json"
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error Login: " + request.error);
            Debug.LogError(request.downloadHandler.text);

            SetStatus("Usuario o contraseña incorrectos.");

            yield break;
        }

        Debug.Log("Respuesta Login: " + request.downloadHandler.text);

        UserResponse response =
            JsonUtility.FromJson<UserResponse>(
                request.downloadHandler.text
            );

        token = response.token;
        username = response.usuario.username;

        PlayerPrefs.SetString("token", token);
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.Save();

        Debug.Log("Login correcto.");
        Debug.Log("Usuario: " + username);
        Debug.Log("Token: " + token);

        SetStatus("Login correcto.");

        // Ir al menú después del login
        SceneManager.LoadScene("Menu");
    }

    // =========================
    // REGISTRO
    // =========================

    public void RegisterButtonClick()
    {
        StartCoroutine(RegisterUser());
    }

    private IEnumerator RegisterUser()
    {
        AuthData authData = new AuthData();

        authData.username = usernameField.text;
        authData.password = passwordField.text;

        string jsonData = JsonUtility.ToJson(authData);

        Debug.Log("Enviando Registro: " + jsonData);

        UnityWebRequest request = UnityWebRequest.Post(
            url + "/api/usuarios",
            jsonData,
            "application/json"
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error Registro: " + request.error);
            Debug.LogError(request.downloadHandler.text);

            SetStatus("No se pudo registrar el usuario.");

            yield break;
        }

        Debug.Log("Respuesta Registro: " + request.downloadHandler.text);

        UserResponse response =
            JsonUtility.FromJson<UserResponse>(
                request.downloadHandler.text
            );

        Debug.Log(
            "Usuario registrado: " +
            response.usuario.username
        );

        SetStatus("Registro exitoso.");

        // Iniciar sesión automáticamente
        StartCoroutine(Login());
    }

    // =========================
    // CERRAR SESIÓN
    // =========================

    public void LogoutButtonClick()
    {
        token = "";
        username = "";

        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.Save();

        Debug.Log("Sesión cerrada.");

        SceneManager.LoadScene("Login");
    }

    // =========================
    // MENSAJES EN PANTALLA
    // =========================

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}

// =========================
// DATOS DE AUTENTICACIÓN
// =========================

[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
}

// =========================
// RESPUESTA DEL SERVIDOR
// =========================

[System.Serializable]
public class UserResponse
{
    public UserData usuario;
    public string token;
}

// =========================
// DATOS DEL USUARIO
// =========================

[System.Serializable]
public class UserData
{
    public string _id;
    public string username;
    public string password;
    public bool estado;
    public ScoreValues data;
}