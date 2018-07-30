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
        SparkSingleton.Instance.AddAuthenticatedCallback(Callback);
    }

    private void Callback()
    {
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
