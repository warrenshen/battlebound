using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    private void Start()
    {
        ShowOptions();
    }

    private void ShowOptions()
    {
        UMPSingleton.Instance.ShowConfirmationDialog(
            "Welcome to Battlebound",
            "Please select an option below.",
            new UnityAction(ShowLogin),
            new UnityAction(ShowRegister),
            "Log in",
            "Create account"
        );
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
            "Cancel",
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
            dialogUI.SetMessage("");

            SparkSingleton.Instance.Login(
                email,
                password,
                () =>
                {
                    if (SparkSingleton.Instance.IsAuthenticated)
                    {
                        dialogUI.Close();
                        LO_LoadingScreen.LoadScene("Menu");
                    }
                    else
                    {
                        string sparkErrorMessage = SparkSingleton.Instance.LoginRegisterErrorMessage;
                        string errorMessage;
                        if (sparkErrorMessage != null)
                        {
                            errorMessage = sparkErrorMessage;
                        }
                        else
                        {
                            errorMessage = "Something went wrong, please try again later";
                        }
                        dialogUI.SetMessage(errorMessage);
                        dialogUI.SetMessageColor(Color.red);
                    }
                }
            );
        }
    }

    private void ShowRegister()
    {
        SparkSingleton.Instance.ClearAuthenticatedCallbacks();

        UMPSingleton.Instance.ShowThreeInputFieldDialog(
            "Create Account",
            null,
            "email",
            "password",
            "password confirmation",
            new UnityAction<UMP_ThreeInputDialogUI, string, string, string>(VerifyRegister),
            new UnityAction(ShowLogin),
            "Create account",
            "Cancel",
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
        if (
            email.Length < 3 ||
            !email.Contains("@") ||
            !email.Contains(".")
        )
        {
            dialogUI.SetMessage("Email address is invalid.");
            dialogUI.SetMessageColor(Color.red);
        }
        else if (email.Length > 30)
        {
            dialogUI.SetMessage("Email address cannot be more than 30 characters.");
            dialogUI.SetMessageColor(Color.red);
        }
        else if (password.Length < 8)
        {
            dialogUI.SetMessage("Password must be at least 8 characters.");
            dialogUI.SetMessageColor(Color.red);
        }
        else if (password.Length > 36)
        {
            dialogUI.SetMessage("Password cannot be more than 36 characters.");
            dialogUI.SetMessageColor(Color.red);
        }
        else if (password != passwordConfirmation)
        {
            dialogUI.SetMessage("Password and confirmation do not match.");
            dialogUI.SetMessageColor(Color.red);
        }
        else
        {
            dialogUI.SetMessage("");

            SparkSingleton.Instance.Register(
                email,
                "",
                password,
                () =>
                {
                    if (SparkSingleton.Instance.IsAuthenticated)
                    {
                        dialogUI.Close();
                        ShowUpdateDisplayName();
                    }
                    else
                    {
                        string sparkErrorMessage = SparkSingleton.Instance.LoginRegisterErrorMessage;
                        string errorMessage;
                        if (sparkErrorMessage != null)
                        {
                            errorMessage = sparkErrorMessage;
                        }
                        else
                        {
                            errorMessage = "Something went wrong, please try again later";
                        }
                        dialogUI.SetMessage(errorMessage);
                        dialogUI.SetMessageColor(Color.red);
                    }
                }
            );
        }
    }

    private void ShowUpdateDisplayName()
    {
        UMPSingleton.Instance.ShowInputFieldDialog(
            "Choose Username",
            "Please enter in a username for your account.",
            new UnityAction<UMP_InputDialogUI, string>(UpdateDisplayName),
            null,
            "Proceed",
            null,
            "Enter username...",
            InputField.ContentType.Standard
        );
    }

    private void UpdateDisplayName(UMP_InputDialogUI dialogUI, string displayName)
    {
        dialogUI.SetMessage("");

        SparkSingleton.Instance.SendUpdateDisplayNameRequest(
            displayName,
            new UnityAction(() =>
            {
                if (SparkSingleton.Instance.IsDisplayNameValid())
                {
                    dialogUI.Close();
                    LO_LoadingScreen.LoadScene("Menu");
                }
                else
                {
                    dialogUI.SetMessage(SparkSingleton.Instance.LoginRegisterErrorMessage);
                    dialogUI.SetMessageColor(Color.red);
                }
            })
        );
    }
}
