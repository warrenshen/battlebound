using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.HdWallet;
using Nethereum.Web3.Accounts;

public class WalletManager : MonoBehaviour
{
    [SerializeField]
    private GameObject welcomePanel;
    [SerializeField]
    private Button finishWelcomeButton;
    [SerializeField]
    private Button backWelcomeButton;

    [SerializeField]
    private GameObject generatePasswordPanel;
    [SerializeField]
    private InputField passwordInputField;
    [SerializeField]
    private InputField passwordConfirmationInputField;
    [SerializeField]
    private Text generatePasswordErrorText;
    [SerializeField]
    private Button finishGeneratePasswordButton;
    [SerializeField]
    private Button backGeneratePasswordButton;

    [SerializeField]
    private GameObject viewMnemonicPanel;
    [SerializeField]
    private Text mnemonicText;
    [SerializeField]
    private InputField passwordRepeatInputField;
    [SerializeField]
    private Text viewMnemonicErrorText;
    [SerializeField]
    private Button finishViewMnemonicButton;

    [SerializeField]
    private GameObject viewSuccessPanel;
    [SerializeField]
    private Button finishViewSuccessButton;

    private string addressChallenge;

    public void Awake()
    {
        this.passwordInputField.onValueChanged.AddListener(
            new UnityAction<string>(OnPasswordValueChanged)
        );
        this.passwordConfirmationInputField.onValueChanged.AddListener(
            new UnityAction<string>(OnPasswordConfirmationValueChanged)
        );
        this.passwordRepeatInputField.onValueChanged.AddListener(
            new UnityAction<string>(OnPasswordRepeatValueChanged)
        );

        this.finishWelcomeButton.onClick.AddListener(OnFinishWelcomeClick);
        this.finishGeneratePasswordButton.onClick.AddListener(OnFinishGeneratePasswordClick);
        this.finishViewMnemonicButton.onClick.AddListener(OnFinishViewMnemonicClick);
        this.finishViewSuccessButton.onClick.AddListener(OnFinishViewSuccessClick);

        DisplayWelcomePanel();
    }

    private void DeactivateAllPanels()
    {
        this.welcomePanel.SetActive(false);
        this.generatePasswordPanel.SetActive(false);
        this.viewMnemonicPanel.SetActive(false);
        this.viewSuccessPanel.SetActive(false);
    }

    private void OnPasswordValueChanged(string password)
    {
        string passwordConfirmation = this.passwordConfirmationInputField.text;

        if (password.Length < 8)
        {
            SetGeneratePasswordError("Password must be more than 8 characters.");
        }
        else if (passwordConfirmation.Length > 0 && password != passwordConfirmation)
        {
            SetGeneratePasswordError("Password and password confirmation do not match.");
        }
        else
        {
            SetGeneratePasswordError("");
        }
    }

    private void OnPasswordConfirmationValueChanged(string passwordConfirmation)
    {
        string password = this.passwordInputField.text;

        if (password != passwordConfirmation)
        {
            SetGeneratePasswordError("Password and password confirmation do not match.");
        }
        else
        {
            SetGeneratePasswordError("");
        }
    }

    private void OnPasswordRepeatValueChanged(string passwordRepeat)
    {
        string passwordConfirmation = this.passwordConfirmationInputField.text;

        if (passwordRepeat != passwordConfirmation)
        {
            SetViewMnemonicError("Incorrect password!");
        }
        else
        {
            SetViewMnemonicError("");
        }
    }

    private void SetGeneratePasswordError(string message)
    {
        this.generatePasswordErrorText.text = message;

        if (message.Length > 0)
        {
            this.finishGeneratePasswordButton.interactable = false;
        }
        else
        {
            this.finishGeneratePasswordButton.interactable = true;
        }
    }

    private void DisplayWelcomePanel()
    {
        DeactivateAllPanels();
        this.welcomePanel.SetActive(true);
    }

    private void OnFinishWelcomeClick()
    {
        CryptoSingleton.Instance.GetAddressChallenge(
            new UnityAction<string>(OnGetChallenge)
        );
    }

    private void OnGetChallenge(string challenge)
    {
        if (challenge == null)
        {
            Debug.LogError("Could not get address challenge.");
            return;
        }

        this.addressChallenge = challenge;
        DisplayGeneratePasswordPanel();
    }

    private void DisplayGeneratePasswordPanel()
    {
        DeactivateAllPanels();
        SetGeneratePasswordError("");
        this.generatePasswordPanel.SetActive(true);
        this.finishGeneratePasswordButton.interactable = false;
    }

    private void OnFinishGeneratePasswordClick()
    {
        string password = this.passwordInputField.text;
        string passwordConfirmation = this.passwordConfirmationInputField.text;

        if (password != passwordConfirmation)
        {
            SetGeneratePasswordError("Password and password confirmation do not match.");
            return;
        }

        string mnemonicString = CryptoSingleton.Instance.InitializePrivateKey(password);
        this.mnemonicText.text = mnemonicString;

        DisplayViewMnemonicPanel();
    }

    private void DisplayViewMnemonicPanel()
    {
        DeactivateAllPanels();
        this.viewMnemonicPanel.SetActive(true);
        this.finishViewMnemonicButton.interactable = false;
    }

    private void OnFinishViewMnemonicClick()
    {
        string passwordRepeat = this.passwordRepeatInputField.text;

        // Mnemonic string in player prefs is comma-separated.
        string mnemonicString = PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_KEY_MNEMONIC);
        mnemonicString = mnemonicString.Replace(",", "");

        Wallet wallet = new Wallet(mnemonicString, passwordRepeat);
        Account account = wallet.GetAccount(0);

        if (account.Address != PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS))
        {
            Debug.LogError("Password repeat does not match saved player prefs.");
        }
        else
        {
            SetGeneratePasswordError("");
        }

        // TODO: show loading screen.
        UpdatePlayerAddress(passwordRepeat);
    }

    private void SetViewMnemonicError(string message)
    {
        this.viewMnemonicErrorText.text = message;

        if (message.Length > 0)
        {
            this.finishViewMnemonicButton.interactable = false;
        }
        else
        {
            this.finishViewMnemonicButton.interactable = true;
        }
    }

    private void UpdatePlayerAddress(string password)
    {
        if (this.addressChallenge == null)
        {
            Debug.LogError("Address challenge does not exist.");
            return;
        }

        string mnemonicString = PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_KEY_MNEMONIC);
        mnemonicString = mnemonicString.Replace(",", "");

        Wallet wallet = new Wallet(mnemonicString, password);
        Account account = wallet.GetAccount(0);

        CryptoSingleton.Instance.UpdatePlayerAddress(
            account,
            this.addressChallenge,
            new UnityAction<string>(DisplayViewSuccessPanel)
        );
    }

    private void DisplayViewSuccessPanel(string publicAddress)
    {
        DeactivateAllPanels();
        this.viewSuccessPanel.SetActive(true);
    }

    private void LeaveScene()
    {
    }

    private void OnFinishViewSuccessClick()
    {

    }
}
