using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    private Text usernameText;

    [SerializeField]
    private Button loginButton;

    private void Awake()
    {
        this.usernameText.text = "Not logged in";
        this.loginButton.onClick.AddListener(OnLoginButtonClick);

        SparkSingleton.Instance.AddAuthenticatedCallback(Callback);
    }

    private void Callback()
    {
        Debug.Log("called");
        if (SparkSingleton.Instance.IsAuthenticated)
        {
            Application.LoadLevel("Menu");
        }
    }

    private void OnLoginButtonClick()
    {
        //LoginRegisterPanel.Instance.Open();
    }
}
