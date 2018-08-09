using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.KeyStore;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3.Accounts;


public class CryptoSingleton : Singleton<CryptoSingleton>
{
    public const string PLAYER_PREFS_PUBLIC_ADDRESS = "PLAYER_PREFS_PUBLIC_ADDRESS";
    public const string PLAYER_PREFS_ENCRYPTED_KEY_STORE = "PLAYER_PREFS_ENCRYPTED_KEY_STORE";

    private int nonce;
    private string txHash;

    private UnityAction<string> updateAddressCallback;
    private UnityAction<string> getChallengeCallback;

    private string GetPublicAddress()
    {
        return PlayerPrefs.GetString(PLAYER_PREFS_PUBLIC_ADDRESS);
    }

    public async Task<string> CreateAuction(
        Account account,
        int tokenId,
        long startingPrice,
        long endingPrice,
        int duration
    )
    {
        // TODO: validate all input prams.
        if (duration < 60)
        {
            Debug.LogError("Invalid duration parameter");
            return "Invalid duration parameter";
        }

        int nonce = (int)await FetchTransactionNonce();
        string signedTx = TreasuryContract.SignCreateAuctionTransaction(
            account,
            nonce,
            tokenId,
            startingPrice,
            endingPrice,
            duration
        );
        Debug.Log("Nonce: " + nonce.ToString());
        Debug.Log("Signed tx: " + signedTx);

        string publicAddress = GetPublicAddress();
        return (string)await SubmitCreateAuctionTransaction(
            publicAddress,
            signedTx,
            tokenId,
            startingPrice,
            endingPrice,
            duration
        );
    }

    private IEnumerator SubmitCreateAuctionTransaction(
        string publicAddress,
        string signedTx,
        int tokenId,
        long startingPrice,
        long endingPrice,
        int duration
    )
    {
        this.txHash = null;

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("SubmitCreateAuctionTransaction");
        request.SetEventAttribute("publicAddress", publicAddress);
        request.SetEventAttribute("signedTx", signedTx);
        request.SetEventAttribute("tokenId", tokenId);
        request.SetEventAttribute("startingPrice", startingPrice);
        request.SetEventAttribute("endingPrice", endingPrice);
        request.SetEventAttribute("duration", duration);
        request.Send(
            OnSubmitCreateAuctionTransactionSuccess,
            OnSubmitCreateAuctionTransactionError
        );

        while (this.txHash == null)
        {
            yield return null;
        }

        yield return this.txHash;
    }

    private void OnSubmitCreateAuctionTransactionSuccess(LogEventResponse response)
    {
        this.txHash = response.ScriptData.GetString("txHash");
        Debug.Log("Got txHash: " + this.txHash);
    }

    private void OnSubmitCreateAuctionTransactionError(LogEventResponse response)
    {
        Debug.Log("SubmitCreateAuctionTransaction request failure.");
        this.txHash = "0x";
    }

    public async Task<string> BidAuction(
        Account account,
        int tokenId,
        long bidPrice
    )
    {
        if (bidPrice <= 0)
        {
            return "Invalid price parameter";
        }

        int nonce = (int)await FetchTransactionNonce();
        string signedTx = AuctionContract.SignBidAuctionTransaction(
            account,
            nonce,
            tokenId,
            bidPrice
        );
        Debug.Log("Nonce: " + nonce.ToString());
        Debug.Log("Signed tx: " + signedTx);

        string publicAddress = GetPublicAddress();
        return (string)await SubmitBidAuctionTransaction(
            publicAddress,
            signedTx,
            tokenId,
            bidPrice
        );
    }

    private IEnumerator SubmitBidAuctionTransaction(
        string publicAddress,
        string signedTx,
        int tokenId,
        long bidPrice
    )
    {
        this.txHash = null;

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("SubmitBidAuctionTransaction");
        request.SetEventAttribute("publicAddress", publicAddress);
        request.SetEventAttribute("signedTx", signedTx);
        request.SetEventAttribute("tokenId", tokenId);
        request.SetEventAttribute("bidPrice", bidPrice);
        request.Send(
            OnSubmitBidAuctionTransactionSuccess,
            OnSubmitBidAuctionTransactionError
        );

        while (this.txHash == null)
        {
            yield return null;
        }

        yield return this.txHash;
    }

    private void OnSubmitBidAuctionTransactionSuccess(LogEventResponse response)
    {
        this.txHash = response.ScriptData.GetString("txHash");
        Debug.Log("Got txHash: " + txHash);
    }

    private void OnSubmitBidAuctionTransactionError(LogEventResponse response)
    {
        Debug.Log("OnSubmitBidAuctionTransactionError request failure.");
        this.txHash = "0x";
    }

    public async Task<string> CancelAuction(
        Account account,
        int tokenId
    )
    {
        // TODO: validate all input prams.
        if (tokenId < 0)
        {
            return "Invalid tokenId parameter";
        }

        int nonce = (int)await FetchTransactionNonce();
        string signedTx = AuctionContract.SignCancelAuctionTransaction(
            account,
            nonce,
            tokenId
        );
        Debug.Log("Nonce: " + nonce.ToString());
        Debug.Log("Signed tx: " + signedTx);

        string publicAddress = GetPublicAddress();
        return txHash = (string)await SubmitCancelAuctionTransaction(
            publicAddress,
            signedTx,
            tokenId
        );
    }

    private IEnumerator SubmitCancelAuctionTransaction(
        string publicAddress,
        string signedTx,
        int tokenId
    )
    {
        this.txHash = null;

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("SubmitCancelAuctionTransaction");
        request.SetEventAttribute("publicAddress", publicAddress);
        request.SetEventAttribute("signedTx", signedTx);
        request.SetEventAttribute("tokenId", tokenId);
        request.Send(
            OnSubmitCancelAuctionTransactionSuccess,
            OnSubmitCancelAuctionTransactionError
        );

        while (this.txHash == null)
        {
            yield return null;
        }

        yield return this.txHash;
    }

    private void OnSubmitCancelAuctionTransactionSuccess(LogEventResponse response)
    {
        this.txHash = response.ScriptData.GetString("txHash");
        Debug.Log("Got txHash: " + this.txHash);
    }

    private void OnSubmitCancelAuctionTransactionError(LogEventResponse response)
    {
        Debug.Log("SubmitCancelAuctionTransaction request failure.");
        this.txHash = "0x";
    }

    public void GetAddressChallenge(UnityAction<string> callback)
    {
        this.getChallengeCallback = callback;

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetAddressChallenge");
        request.Send(OnGetAddressChallengeSuccess, OnGetAddressChallengeFailure);
    }

    private void OnGetAddressChallengeSuccess(LogEventResponse response)
    {
        GSData scriptData = response.ScriptData;
        string challenge = scriptData.GetString("addressChallenge");
        if (this.getChallengeCallback != null)
        {
            this.getChallengeCallback(challenge);
            this.getChallengeCallback = null;
        }
        else
        {
            Debug.LogError("Challenge callback does not exist.");
        }
    }

    private void OnGetAddressChallengeFailure(LogEventResponse response)
    {
        if (this.getChallengeCallback != null)
        {
            this.getChallengeCallback(null);
            this.getChallengeCallback = null;
        }
        else
        {
            Debug.LogError("Callback does not exist.");
        }

        Debug.LogError("GetAddressChallenge request failure.");
    }

    public void UpdatePlayerAddress(
        Account account,
        string challenge,
        UnityAction<string> callback
    )
    {
        this.updateAddressCallback = callback;

        EthECKey ethECKey = new EthECKey(account.PrivateKey);
        string publicAddress = ethECKey.GetPublicAddress();
        //Debug.Log("PA: " + publicAddress);

        challenge = "0x" + challenge;
        //Debug.Log("C: " + challenge);

        List<byte> byteList = new List<byte>();
        var bytePrefix = "0x19".HexToByteArray();
        var textBytePrefix = Encoding.UTF8.GetBytes("Ethereum Signed Message:\n" + challenge.HexToByteArray().Length);
        var bytesMessage = challenge.HexToByteArray();

        byteList.AddRange(bytePrefix);
        byteList.AddRange(textBytePrefix);
        byteList.AddRange(bytesMessage);

        Sha3Keccack hasher = new Sha3Keccack();
        var hash = hasher.CalculateHash(byteList.ToArray()).ToHex();
        //Debug.Log("HH: " + hash);

        var signer = new MessageSigner();

        string signature = signer.Sign(hash.HexToByteArray(), ethECKey.GetPrivateKey());
        //Debug.Log("Signature: " + signature);

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("UpdatePlayerAddress");
        request.SetEventAttribute("address", publicAddress);
        request.SetEventAttribute("signature", signature);
        request.Send(OnUpdatePlayerAddressSuccess, OnUpdatePlayerAddressFailure);
    }

    private void OnUpdatePlayerAddressSuccess(LogEventResponse response)
    {
        string address = response.ScriptData.GetString("address");

        if (this.updateAddressCallback != null)
        {
            this.updateAddressCallback(address);
            this.updateAddressCallback = null;
        }
        else
        {
            Debug.LogError("Callback does not exist.");
        }
    }

    private void OnUpdatePlayerAddressFailure(LogEventResponse response)
    {
        if (this.updateAddressCallback != null)
        {
            this.updateAddressCallback(null);
            this.updateAddressCallback = null;
        }
        else
        {
            Debug.LogError("Callback does not exist.");
        }

        Debug.LogError("UpdatePlayerAddress request failure.");
    }

    /*
     * Generates a private key with the given password and
     * returns the mnemonic associated with the private key.
     * 
     * @return string - mnemonic string with comma delimiter
     */
    public string InitializePrivateKey(string password)
    {
        Wallet wallet = new Wallet(Wordlist.English, WordCount.Twelve);
        string mnemonicString = string.Join(" ", wallet.Words);

        Account account = wallet.GetAccount(0);
        string publicAddress = account.Address;

        KeyStorePbkdf2Service keyStorePbkdf2Service = new KeyStorePbkdf2Service();
        string keyStoreJson = keyStorePbkdf2Service.EncryptAndGenerateKeyStoreAsJson(
            password,
            account.PrivateKey.HexToByteArray(),
            publicAddress
        );

        PlayerPrefs.SetString(PLAYER_PREFS_ENCRYPTED_KEY_STORE, keyStoreJson);
        PlayerPrefs.SetString(PLAYER_PREFS_PUBLIC_ADDRESS, publicAddress);

        Debug.Log("Set player prefs public address to: " + publicAddress);

        return mnemonicString;
    }

    public string RecoverPrivateKey(string mnemonicString, string password)
    {
        Account account = GetAccountWithMnemonic(mnemonicString);
        if (account == null)
        {
            return null;
        }

        string publicAddress = account.Address;

        KeyStorePbkdf2Service keyStorePbkdf2Service = new KeyStorePbkdf2Service();
        string keyStoreJson = keyStorePbkdf2Service.EncryptAndGenerateKeyStoreAsJson(
            password,
            account.PrivateKey.HexToByteArray(),
            publicAddress
        );

        PlayerPrefs.SetString(PLAYER_PREFS_ENCRYPTED_KEY_STORE, keyStoreJson);
        PlayerPrefs.SetString(PLAYER_PREFS_PUBLIC_ADDRESS, publicAddress);

        return publicAddress;
    }

    public Account GetAccountWithPassword(string password)
    {
        string keyStoreJson = PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_ENCRYPTED_KEY_STORE);
        if (string.IsNullOrEmpty(keyStoreJson))
        {
            Debug.LogError("Player prefs key store does not exist.");
            return null;
        }

        KeyStorePbkdf2Service keyStorePbkdf2Service = new KeyStorePbkdf2Service();
        var keyStore = keyStorePbkdf2Service.DeserializeKeyStoreFromJson(keyStoreJson);
        string privateKeyDecrypted = keyStorePbkdf2Service.DecryptKeyStore(password, keyStore).ToHex();

        return new Account(privateKeyDecrypted.HexToByteArray());
    }

    public Account GetAccountWithMnemonic(string mnemonicString)
    {
        if (mnemonicString.Contains(","))
        {
            mnemonicString = mnemonicString.Replace(",", "");
        }

        string[] parsedMnemonic = mnemonicString.Split(' ');
        if (parsedMnemonic.Length != 12)
        {
            return null;
        }

        Wallet wallet = new Wallet(mnemonicString, null);
        return wallet.GetAccount(0);
    }

    private IEnumerator FetchTransactionNonce()
    {
        this.nonce = -1;

        GetTransactionNonce();

        while (this.nonce < 0)
        {
            yield return null;
        }

        yield return this.nonce;
    }

    private void GetTransactionNonce()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetTransactionNonce");
        request.Send(OnGetTransactionNonceSuccess, OnGetTransactionNonceError);
    }

    private void OnGetTransactionNonceSuccess(LogEventResponse response)
    {
        this.nonce = (int)response.ScriptData.GetInt("nonce");
    }

    private void OnGetTransactionNonceError(LogEventResponse response)
    {
        StopCoroutine("CreateAuctionHelper");
        GSData errors = response.Errors;
        Debug.LogError(errors);
    }
}
