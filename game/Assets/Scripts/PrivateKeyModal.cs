using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.Web3.Accounts;

public class PrivateKeyModal : MonoBehaviour
{
    [SerializeField]
    private InputField passwordInputField;

    [SerializeField]
    private InputField mnemonicInputField;

    [SerializeField]
    private Button backButton;

    [SerializeField]
    private Button submitButton;

    private UnityAction<Account> awaitingAction;

    public static PrivateKeyModal Instance { get; private set; }

    public void Awake()
    {
        Instance = this;

        this.backButton.onClick.AddListener(Close);
        this.submitButton.onClick.AddListener(VerifyPasswordAndContinue);

        Close();
    }

    public void ShowModalWithCallback(UnityAction<Account> callback)
    {
        this.awaitingAction = callback;
        this.gameObject.SetActive(true);
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }

    private void VerifyPasswordAndContinue()
    {
        string password = this.passwordInputField.text;

        if (!PlayerPrefs.HasKey(CryptoSingleton.PLAYER_PREFS_ENCRYPTED_KEY_STORE))
        {
            Debug.LogError("Player prefs encrypted key store does not exist.");
            return;
        }
        else if (!PlayerPrefs.HasKey(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS))
        {
            Debug.LogError("Player prefs public address does not exist.");
            return;
        }

        Account account = CryptoSingleton.Instance.GetAccountWithPassword(password);

        string publicAddress = PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS);
        if (publicAddress != account.Address)
        {
            Debug.Log(publicAddress);
            Debug.Log(account.Address);
            Debug.LogError("Password is incorrect?");
            return;
        }
        else
        {
            if (this.awaitingAction != null)
            {
                this.awaitingAction(account);
            }
            else
            {
                Debug.LogError("Verify password and continue called with no awaiting action.");
            }
        }

        Close();
    }
}
