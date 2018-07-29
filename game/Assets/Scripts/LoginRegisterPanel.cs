using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Responses;

public class LoginRegisterPanel : MonoBehaviour
{
    [SerializeField]
    private Transform loginPanel;

    [SerializeField]
    private InputField usernameInputField;

    [SerializeField]
    private InputField passwordInputField;

    [SerializeField]
    private Button loginButton;

    public static LoginRegisterPanel Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        this.loginButton.onClick.AddListener(OnLoginButtonClick);
    }

    //private void Close()
    //{
    //    this.gameObject.SetActive(false);
    //}

    //public void Open()
    //{
    //    this.gameObject.SetActive(true);
    //}

    private void OnLoginButtonClick()
    {
        SparkSingleton.Instance.Login(
            usernameInputField.text,
            "password"
        );
    }

    public void OnLoginSuccess(AuthenticationResponse response)
    {
        Application.LoadLevel("Menu");

    }

    public void OnLoginError(AuthenticationResponse response)
    {
        Debug.Log("OnLoginError");
        Debug.Log(response.Errors.ToString());
    }
}
