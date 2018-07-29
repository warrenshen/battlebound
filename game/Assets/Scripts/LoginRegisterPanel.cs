using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Responses;

public class LoginRegisterPanel : MonoBehaviour
{
    private const int MODE_LOGIN = 0;
    private const int MODE_REGISTER = 1;

    private int mode = MODE_LOGIN;

    [SerializeField]
    private Transform loginPanel;

    [SerializeField]
    private InputField loginUsernameInputField;

    [SerializeField]
    private InputField loginPasswordInputField;

    [SerializeField]
    private Button loginButton;

    [SerializeField]
    private Transform registerPanel;

    [SerializeField]
    private InputField registerEmailInputField;

    [SerializeField]
    private InputField registerUsernameInputField;

    [SerializeField]
    private InputField registerPasswordInputField;

    [SerializeField]
    private Button switchButton;

    public static LoginRegisterPanel Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        this.loginButton.onClick.AddListener(OnLoginButtonClick);
        this.switchButton.onClick.AddListener(OnSwitchButtonClick);

        ShowRegisterPanel();
    }

    private void ShowLoginPanel()
    {
        this.mode = MODE_LOGIN;

        this.loginPanel.gameObject.SetActive(true);
        this.registerPanel.gameObject.SetActive(false);
        this.switchButton.GetComponentInChildren<Text>().text = "Create account";
    }

    private void ShowRegisterPanel()
    {
        this.mode = MODE_REGISTER;

        this.registerPanel.gameObject.SetActive(true);
        this.loginPanel.gameObject.SetActive(false);
        this.switchButton.GetComponentInChildren<Text>().text = "Log in";
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
            loginUsernameInputField.text,
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

    private void OnSwitchButtonClick()
    {
        if (this.mode == MODE_LOGIN)
        {
            ShowRegisterPanel();
        }
        else
        {
            ShowLoginPanel();
        }
    }

    private void OnRegisterButtonClick()
    {
        SparkSingleton.Instance.Register(
            registerEmailInputField.text,
            registerUsernameInputField.text,
            registerPasswordInputField.text
        );
    }

    public void OnRegisterSuccess(RegistrationResponse response)
    {
        Application.LoadLevel("Menu");

    }

    public void OnRegisterError(RegistrationResponse response)
    {
        Debug.Log("OnLoginError");
        Debug.Log(response.Errors.ToString());
    }
}
