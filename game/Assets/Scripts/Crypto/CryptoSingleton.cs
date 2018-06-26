using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
	private string txHash;

    public async Task<string> CreateAuction(
		int tokenId,
	    long startingPrice,
		long endingPrice,
        int duration
	)
	{
		// TODO: validate all input prams.
		if (duration < 60)
		{
			return "Invalid duration parameter";
		}

		return await CreateAuctionHelper(
			tokenId,
			startingPrice,
			endingPrice,
			duration
		);
	}

	private async Task<string> CreateAuctionHelper(
		int tokenId,
        long startingPrice,
        long endingPrice,
        int duration
	)
	{
		int nonce = (int) await FetchTransactionNonce();

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

		return (string) await SubmitCreateAuctionTransaction(
			signedTx,
            tokenId,
			startingPrice,
			endingPrice,
            duration
		);
	}

	private IEnumerator SubmitCreateAuctionTransaction(
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
    
	public async Task<string> BidAuction(int tokenId, long bidPrice)
	{
		if (bidPrice <= 0)
		{
			return "Invalid price parameter";
		}

		return await BidAuctionHelper(
            tokenId,
            bidPrice
        );
	}

	private async Task<string> BidAuctionHelper(
        int tokenId,
		long bidPrice
    )
    {
        int nonce = (int) await FetchTransactionNonce();

        Account account = AuthorizeAndGetAccount();
		string signedTx = AuctionContract.signBidAuctionTransaction(
            account,
            nonce,
            tokenId,
            bidPrice
        );
        Debug.Log("Nonce: " + nonce.ToString());
		Debug.Log("Signed tx: " + signedTx);

        return (string) await SubmitBidAuctionTransaction(
            signedTx,
            tokenId,
            bidPrice
        );
    }

	private IEnumerator SubmitBidAuctionTransaction(
        string signedTx,
        int tokenId,
        long bidPrice
    )
    {
        this.txHash = null;

		LogEventRequest request = new LogEventRequest();
        request.SetEventKey("SubmitBidAuctionTransaction");
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
		Debug.Log("Got txHash: " + txHash);
		this.txHash = response.ScriptData.GetString("txHash");
	}

	private void OnSubmitBidAuctionTransactionError(LogEventResponse response)
	{
		Debug.Log("OnSubmitBidAuctionTransactionError request failure.");
		this.txHash = "0x";
	}

	public async Task<string> CancelAuction(int tokenId)
    {
        // TODO: validate all input prams.
        if (tokenId < 0)
        {
            return "Invalid tokenId parameter";
        }
        
        return await CancelAuctionHelper(tokenId);
    }

    private async Task<string> CancelAuctionHelper(int tokenId)
    {
        int nonce = (int) await FetchTransactionNonce();
        
        Account account = AuthorizeAndGetAccount();
		string signedTx = AuctionContract.signCancelAuctionTransaction(
            account,
            nonce,
            tokenId
        );
        Debug.Log("Nonce: " + nonce.ToString());
        Debug.Log("Signed tx: " + signedTx);

        string txHash = (string) await SubmitCancelAuctionTransaction(
            signedTx,
            tokenId
        );

        // Handle error case.
        return txHash;
    }

	private IEnumerator SubmitCancelAuctionTransaction(
        string signedTx,
        int tokenId
    )
    {
		this.txHash = null;

        LogEventRequest request = new LogEventRequest();
		request.SetEventKey("SubmitCancelAuctionTransaction");
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
