using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3.Accounts;


public class CryptoSingleton : Singleton<CryptoSingleton>
{
	const string PLAYER_PREFS_KEY_MNEMONIC = "PLAYER_PREFS_KEY_MNEMONIC";

	private int nonce;

	private void Awake()
    {
        base.Awake();
    }

    public string CreateAuction(
		int tokenId,
	    int startingPrice,
        int endingPrice,
        int duration
	)
	{
		// TODO: validate all input prams.
		if (duration < 60)
		{
			return "Invalid duration parameter";	
		}

		object[] args = new object[4] {
			tokenId,
			startingPrice,
			endingPrice,
			duration,
		};

		StartCoroutine("CreateAuctionHelper", args);

		return "";
	}

	private IEnumerator CreateAuctionHelper(object[] args)
	{
		int tokenId = (int) args[0];
		int startingPrice = (int) args[1];
		int endingPrice = (int) args[2];
		int duration = (int) args[3];

		this.nonce = -1;

		GetTransactionNonce();
        
        while (this.nonce < 0)
		{
			yield return null;
		}

		Account account = AuthorizeAndGetAccount();
        string signedTx = TreasuryContract.signCreateAuctionTransaction(
            account,
            nonce,
            tokenId,
            startingPrice,
            endingPrice,
            duration
        );
		Debug.Log("Nonce: " + nonce.ToString());
		Debug.Log("Signed tx: " + signedTx);

        LogEventRequest request = new LogEventRequest();
		request.SetEventKey("SubmitCreateAuctionTransaction");
		request.SetEventAttribute("signedTx", signedTx);
		request.SetEventAttribute("tokenId", tokenId);
		request.SetEventAttribute("startingPrice", startingPrice);
		request.SetEventAttribute("endingPrice", endingPrice);
		request.SetEventAttribute("duration", duration);
		//request.Send(OnGetTransactionNonceError, OnGetTransactionNonceError);
	}
    
 //   public string BidAuction(int tokenId, long price)
	//{
	//	// TODO: validate input prams.
	//	GetTransactionNonce();
	//	Account account = AuthorizeAndGetAccount();
	//}

	public void UpdatePlayerAddress()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetAddressChallenge");
        request.Send(OnGetAddressChallengeSuccess, OnGetAddressChallengeFailure);
    }

    private void OnGetAddressChallengeSuccess(LogEventResponse response)
    {
        GSData scriptData = response.ScriptData;
        string challenge = scriptData.GetString("addressChallenge");

		Account account = AuthorizeAndGetAccount();
        EthECKey ethECKey = new EthECKey(account.PrivateKey);
        string publicAddress = ethECKey.GetPublicAddress();
        Debug.Log("PA: " + publicAddress);

        challenge = "0x" + challenge;
        Debug.Log("C: " + challenge);

        List<byte> byteList = new List<byte>();
        var bytePrefix = "0x19".HexToByteArray();
        var textBytePrefix = Encoding.UTF8.GetBytes("Ethereum Signed Message:\n" + challenge.HexToByteArray().Length);
        var bytesMessage = challenge.HexToByteArray();

        byteList.AddRange(bytePrefix);
        byteList.AddRange(textBytePrefix);
        byteList.AddRange(bytesMessage);

        Sha3Keccack hasher = new Sha3Keccack();
        var hash = hasher.CalculateHash(byteList.ToArray()).ToHex();
        Debug.Log("HH: " + hash);

        var signer = new MessageSigner();

        string signature = signer.Sign(hash.HexToByteArray(), ethECKey.GetPrivateKey());
        Debug.Log("Signature: " + signature);

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("UpdatePlayerAddress");
        request.SetEventAttribute("address", publicAddress);
        request.SetEventAttribute("signature", signature);
        request.Send(OnUpdatePlayerAddressSuccess, OnUpdatePlayerAddressFailure);
    }

    private void OnGetAddressChallengeFailure(LogEventResponse response)
    {
        Debug.Log("GetAddressChallenge request failure.");
    }

    private void OnUpdatePlayerAddressSuccess(LogEventResponse response)
    {
        Debug.Log("GetAddressChallenge request success.");
        Debug.Log("Set player address to: " + response.ScriptData.GetString("address"));
    }

    private void OnUpdatePlayerAddressFailure(LogEventResponse response)
    {
        Debug.Log("UpdatePlayerAddress request failure.");
    }

    private Account AuthorizeAndGetAccount()
    {
		if (!PlayerPrefs.HasKey(PLAYER_PREFS_KEY_MNEMONIC))
		{
			GenerateMnemonic();
		}

        // Mnemonic string in player prefs is comma-separated.
        string mnemonicString = PlayerPrefs.GetString(PLAYER_PREFS_KEY_MNEMONIC);
        mnemonicString = mnemonicString.Replace(",", "");

        Wallet wallet = new Wallet(mnemonicString, "");
        Account account = wallet.GetAccount(0);
		return account;
    }

	private void GenerateMnemonic()
    {
        //Wallet wallet = new Wallet(Wordlist.English, WordCount.Twelve, password);
        Wallet wallet = new Wallet(Wordlist.English, WordCount.Twelve);
        string mnemonicString = string.Join(", ", wallet.Words);

        Debug.Log(mnemonicString);
        Debug.Log(wallet.Seed);
        Debug.Log(string.Join("", wallet.GetPrivateKey(0)));

        PlayerPrefs.SetString(PLAYER_PREFS_KEY_MNEMONIC, mnemonicString);
        Debug.Log("Set player pref mnemonic!");

		UpdatePlayerAddress();
    }

    private void GetTransactionNonce()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetTransactionNonce");
        request.Send(OnGetTransactionNonceSuccess, OnGetTransactionNonceError);
    }

    private void OnGetTransactionNonceSuccess(LogEventResponse response)
    {        
		this.nonce = (int) response.ScriptData.GetInt("nonce");
		Debug.Log("Got nonce: " + this.nonce);
    }

    private void OnGetTransactionNonceError(LogEventResponse response)
    {
		StopCoroutine("CreateAuctionHelper");
		GSData errors = response.Errors;
		Debug.Log(errors);
    }
}
