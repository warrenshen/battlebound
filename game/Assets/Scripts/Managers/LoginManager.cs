using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    private Text usernameText;

    [SerializeField]
    private Button loginButton;

    [SerializeField]
    private Button logoutButton;

    private void Awake()
    {
        this.usernameText.text = "Not logged in";
        this.loginButton.onClick.AddListener(OnLoginButtonClick);
        this.logoutButton.onClick.AddListener(OnLogoutButtonClick);
    }

    private void OnLoginButtonClick()
    {
        LoginRegisterPanel.Instance.Open();
    }

    private void OnLogoutButtonClick()
    {
        SparkSingleton.Instance.Logout();
    }
}
