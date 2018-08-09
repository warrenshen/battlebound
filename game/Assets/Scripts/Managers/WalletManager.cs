using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.Web3.Accounts;

public class WalletManager : MonoBehaviour
{
    private string addressChallenge;

    private void Start()
    {
        if (PlayerPrefs.HasKey(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS))
        {
            UMPSingleton.Instance.ShowConfirmationDialog(
                "Wallet Already Exists",
                "You already have a wallet configured on this device, are you sure you want to replace it?",
                new UnityAction(GetAddressChallenge),
                new UnityAction(ReturnToMenu),
                "Confirm",
                "Return to menu"
            );
        }
    }

    private void GetAddressChallenge()
    {
        CryptoSingleton.Instance.GetAddressChallenge(
            new UnityAction<string>(OnGetChallenge)
        );
    }

    private void OnGetChallenge(string challenge)
    {
        if (challenge == null)
        {
            UMPSingleton.Instance.ShowConfirmationDialog(
                "Unexpected Error",
                "Server could not be reached - please try again later.",
                new UnityAction(ReturnToMenu),
                null,
                "Return to menu",
                ""
            );
        }

        this.addressChallenge = challenge;

        UMPSingleton.Instance.ShowConfirmationDialog(
            "Wallet Management",
            "If you have a 12-word mnemonic already, choose import wallet.",
            new UnityAction(ShowNewWalletEnterPassword),
            new UnityAction(ShowImportWalletEnterPassword),
            "New wallet",
            "Import wallet"
        );
    }

    private void ShowNewWalletEnterPassword()
    {
        UMPSingleton.Instance.ShowTwoInputFieldDialog(
            "Wallet Create Password",
            "You'll use this password to trade your cards",
            "Password",
            "Password confirmation",
            new UnityAction<UMP_TwoInputDialogUI, string, string>(VerifyNewWalletEnterPassword),
            new UnityAction(CancelNewWalletEnterPassword),
            "Proceed",
            "Cancel",
            InputField.ContentType.Password,
            InputField.ContentType.Password
        );
    }

    private void VerifyNewWalletEnterPassword(
        UMP_TwoInputDialogUI dialogUI,
        string password,
        string passwordConfirmation
    )
    {
        if (password != passwordConfirmation)
        {
            dialogUI.SetMessage("Password and confirmation do not match");
            dialogUI.SetMessageColor(Color.red);
        }
        else
        {
            string mnemonicString = CryptoSingleton.Instance.InitializePrivateKey(password);
            string mnemonicStringWithDescription = string.Format("Write these 12 words down in a <b>SAFE</b> place:\n\n{0}", mnemonicString);
            dialogUI.Close();

            UMPSingleton.Instance.ShowConfirmationDialog(
                "Wallet New Mnemonic",
                mnemonicStringWithDescription,
                new UnityAction(ShowNewWalletRepeatPassword),
                null,
                "Proceed",
                ""
            );
        }
    }

    private void CancelNewWalletEnterPassword()
    {
        ReturnToMenu();
    }

    private void ShowNewWalletRepeatPassword()
    {
        UMPSingleton.Instance.ShowInputFieldAndAreaDialog(
            "Wallet Double Check",
            "Please enter your mnemonic and password again",
            "Password repeat",
            "Mnemonic repeat",
            new UnityAction<UMP_TwoInputDialogUI, string, string>(VerifyNewWalletRepeatPassword),
            null,
            "Proceed",
            "Cancel",
            InputField.ContentType.Password,
            InputField.ContentType.Standard
        );
    }

    private void VerifyNewWalletRepeatPassword(
        UMP_TwoInputDialogUI dialogUI,
        string passwordRepeat,
        string mnemonicRepeat
    )
    {
        Account accountByPassword = CryptoSingleton.Instance.GetAccountWithPassword(passwordRepeat);
        Account accountByMnemonic = CryptoSingleton.Instance.GetAccountWithMnemonic(mnemonicRepeat);

        if (
            accountByPassword == null ||
            accountByMnemonic == null ||
            accountByPassword.Address != PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS) ||
            accountByMnemonic.Address != PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS)
        )
        {
            dialogUI.SetMessage("Mnemonic or password is not correct.");
            dialogUI.SetMessageColor(Color.red);
        }
        else
        {
            dialogUI.Close();
            UpdatePlayerAddressNewWallet(passwordRepeat);
        }
    }

    private void UpdatePlayerAddressNewWallet(string password)
    {
        if (this.addressChallenge == null)
        {
            Debug.LogError("Address challenge does not exist.");
            return;
        }

        Account account = CryptoSingleton.Instance.GetAccountWithPassword(password);

        CryptoSingleton.Instance.UpdatePlayerAddress(
            account,
            this.addressChallenge,
            new UnityAction<string>(ShowNewWalletFinish)
        );
    }

    private void ShowNewWalletFinish(string publicAddress)
    {
        UMPSingleton.Instance.ShowConfirmationDialog(
            "Wallet Create Success",
            string.Format("Your wallet's public identifier is: {0}", publicAddress),
            new UnityAction(ReturnToMenu),
            null,
            "Return to menu",
            ""
        );
    }

    private void ShowImportWalletEnterPassword()
    {
        UMPSingleton.Instance.ShowInputFieldAndAreaDialog(
            "Wallet Import",
            "Enter your 12-word mnemonic and a password to protect your wallet",
            "New password",
            "Mnemonic",
            new UnityAction<UMP_TwoInputDialogUI, string, string>(VerifyImportWalletEnterPassword),
            new UnityAction(CancelNewWalletEnterPassword),
            "Proceed",
            "Cancel",
            InputField.ContentType.Password,
            InputField.ContentType.Standard
        );
    }

    private void VerifyImportWalletEnterPassword(
        UMP_TwoInputDialogUI dialogUI,
        string password,
        string mnemonic
    )
    {
        Account accountByMnemonic = CryptoSingleton.Instance.GetAccountWithMnemonic(mnemonic);

        if (accountByMnemonic == null)
        {
            dialogUI.SetMessage("Mnemonic or password format is not correct.");
            dialogUI.SetMessageColor(Color.red);
        }
        else
        {
            string publicAddress = CryptoSingleton.Instance.RecoverPrivateKey(mnemonic, password);
            Debug.Log(publicAddress);
            dialogUI.Close();
            ShowImportWalletRepeatPassword();
        }
    }

    private void ShowImportWalletRepeatPassword()
    {
        UMPSingleton.Instance.ShowInputFieldAndAreaDialog(
            "Wallet Double Check",
            "Please enter your mnemonic and password again",
            "Password repeat",
            "Mnemonic repeat",
            new UnityAction<UMP_TwoInputDialogUI, string, string>(VerifyImportWalletRepeatPassword),
            null,
            "Proceed",
            "Cancel",
            InputField.ContentType.Password,
            InputField.ContentType.Standard
        );
    }

    private void VerifyImportWalletRepeatPassword(
        UMP_TwoInputDialogUI dialogUI,
        string passwordRepeat,
        string mnemonicRepeat
    )
    {
        Account accountByPassword = CryptoSingleton.Instance.GetAccountWithPassword(passwordRepeat);
        Account accountByMnemonic = CryptoSingleton.Instance.GetAccountWithMnemonic(mnemonicRepeat);

        if (
            accountByPassword == null ||
            accountByMnemonic == null ||
            accountByPassword.Address != PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS) ||
            accountByMnemonic.Address != PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS)
        )
        {
            dialogUI.SetMessage("Mnemonic or password is not correct.");
            dialogUI.SetMessageColor(Color.red);
        }
        else
        {
            dialogUI.Close();
            UpdatePlayerAddressImportWallet(passwordRepeat);
        }
    }

    private void UpdatePlayerAddressImportWallet(string password)
    {
        if (this.addressChallenge == null)
        {
            Debug.LogError("Address challenge does not exist.");
            return;
        }

        Account account = CryptoSingleton.Instance.GetAccountWithPassword(password);

        CryptoSingleton.Instance.UpdatePlayerAddress(
            account,
            this.addressChallenge,
            new UnityAction<string>(ShowImportWalletFinish)
        );
    }

    private void ShowImportWalletFinish(string publicAddress)
    {
        UMPSingleton.Instance.ShowConfirmationDialog(
            "Wallet Import Success",
            string.Format("Your wallet's public identifier is: {0}", publicAddress),
            new UnityAction(ReturnToMenu),
            null,
            "Return to menu",
            ""
        );
    }

    private void ReturnToMenu()
    {
        Application.LoadLevel("Menu");
    }
}
