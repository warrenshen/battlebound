using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    private void Start()
    {
        SparkSingleton.Instance.AddAuthenticatedCallback(Callback);

        bool hasLoggedIn = SparkSingleton.Instance.HasLoggedIn();
        if (hasLoggedIn)
        {
            ShowLogin();
        }
        else
        {
            ShowRegister();
        }
    }

    private void Callback()
    {
        if (SparkSingleton.Instance.IsAuthenticated)
        {
            UMPSingleton.Instance.CloseAllDialogs();
            Application.LoadLevel("Menu");
        }
    }

    private void ShowLogin()
    {
        UMPSingleton.Instance.ShowTwoInputFieldDialog(
            "Welcome Back",
            null,
            "email",
            "password",
            new UnityAction<UMP_TwoInputDialogUI, string, string>(VerifyLogin),
            new UnityAction(ShowRegister),
            "Log in",
            "Register",
            InputField.ContentType.Standard,
            InputField.ContentType.Password
        );
    }

    private void VerifyLogin(
        UMP_TwoInputDialogUI dialogUI,
        string email,
        string password
    )
    {
        if (email.Length < 3)
        {
            dialogUI.SetMessage("Email is not long enough.");
            dialogUI.SetMessageColor(Color.red);
        }
        else if (password.Length < 1)
        {
            dialogUI.SetMessage("Password is not long enough.");
            dialogUI.SetMessageColor(Color.red);
        }
        else
        {
            SparkSingleton.Instance.Login(
                email,
                password,
                () =>
                {
                    if (SparkSingleton.Instance.IsAuthenticated)
                    {
                        dialogUI.Close();
                        Application.LoadLevel("Menu");
                    }
                    else
                    {
                        dialogUI.SetMessage("Invalid email or password.");
                        dialogUI.SetMessageColor(Color.red);
                    }
                }
            );
        }
    }

    private void ShowRegister()
    {
        UMPSingleton.Instance.ShowThreeInputFieldDialog(
            "Create Account",
            null,
            "email",
            "password",
            "password confirmation",
            new UnityAction<UMP_ThreeInputDialogUI, string, string, string>(VerifyRegister),
            new UnityAction(ShowLogin),
            "Register",
            "Log in",
            InputField.ContentType.Standard,
            InputField.ContentType.Password,
            InputField.ContentType.Password
        );
    }

    private void VerifyRegister(
        UMP_ThreeInputDialogUI dialogUI,
        string email,
        string password,
        string passwordConfirmation
    )
    {
        if (email.Length < 3)
        {
            dialogUI.SetMessage("Email is not long enough.");
            dialogUI.SetMessageColor(Color.red);
        }
        else if (password.Length < 1)
        {
            dialogUI.SetMessage("Password is not long enough.");
            dialogUI.SetMessageColor(Color.red);
        }
        else if (password != passwordConfirmation)
        {
            dialogUI.SetMessage("Password and confirmation do not match.");
            dialogUI.SetMessageColor(Color.red);
        }
        else
        {
            SparkSingleton.Instance.Register(
                email,
                "",
                password,
                () =>
                {
                    if (SparkSingleton.Instance.IsAuthenticated)
                    {
                        dialogUI.Close();
                        Application.LoadLevel("Menu");
                    }
                    else
                    {
                        dialogUI.SetMessage("Something went wrong, please try again later.");
                        dialogUI.SetMessageColor(Color.red);
                    }
                }
            );
        }
    }
}
