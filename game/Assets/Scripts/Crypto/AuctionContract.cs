using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;

public class AuctionContract
{
	private static string web3Url = "https://rinkeby.infura.io/kBLFY7NMU7NrFMdpvDR8";
    
	private static string contractAddress = "0xEFa6737A7439BFAee477C471241822dAd3558CB2";
	private static string contractAbi = "[{'constant':false,'inputs':[{'name':'_tokenId','type':'uint256'},{'name':'_startingPrice','type':'uint256'},{'name':'_endingPrice','type':'uint256'},{'name':'_duration','type':'uint256'},{'name':'_seller','type':'address'}],'name':'createAuction','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[],'name':'unpause','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'_tokenId','type':'uint256'}],'name':'bid','outputs':[],'payable':true,'stateMutability':'payable','type':'function'},{'constant':true,'inputs':[],'name':'paused','outputs':[{'name':'','type':'bool'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[],'name':'withdrawBalance','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[],'name':'renounceOwnership','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_tokenId','type':'uint256'}],'name':'getAuction','outputs':[{'name':'seller','type':'address'},{'name':'startingPrice','type':'uint256'},{'name':'endingPrice','type':'uint256'},{'name':'duration','type':'uint256'},{'name':'startedAt','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'ownerCut','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[],'name':'pause','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'_tokenId','type':'uint256'}],'name':'cancelAuctionWhenPaused','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'owner','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_tokenId','type':'uint256'}],'name':'cancelAuction','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_tokenId','type':'uint256'}],'name':'getCurrentPrice','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'isSaleAuction','outputs':[{'name':'','type':'bool'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'nonFungibleContract','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'newOwner','type':'address'}],'name':'transferOwnership','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'inputs':[{'name':'_nftAddress','type':'address'},{'name':'_cut','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'constructor'},{'anonymous':false,'inputs':[{'indexed':false,'name':'tokenId','type':'uint256'},{'indexed':false,'name':'startingPrice','type':'uint256'},{'indexed':false,'name':'endingPrice','type':'uint256'},{'indexed':false,'name':'duration','type':'uint256'}],'name':'AuctionCreated','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'tokenId','type':'uint256'},{'indexed':false,'name':'totalPrice','type':'uint256'},{'indexed':false,'name':'winner','type':'address'}],'name':'AuctionSuccessful','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'tokenId','type':'uint256'}],'name':'AuctionCancelled','type':'event'},{'anonymous':false,'inputs':[],'name':'Pause','type':'event'},{'anonymous':false,'inputs':[],'name':'Unpause','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'previousOwner','type':'address'}],'name':'OwnershipRenounced','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'previousOwner','type':'address'},{'indexed':true,'name':'newOwner','type':'address'}],'name':'OwnershipTransferred','type':'event'}]";

	public static string signBidTransaction(
		Account account,
		int nonce,
		int tokenId,
        long bidPrice,
		long gasPrice = 5000000000L
	)
	{
		Web3 web3 = new Web3();
        Contract contract = web3.Eth.GetContract(contractAbi, contractAddress);
        
        Function bidFunction = contract.GetFunction("bid");
        string data = bidFunction.GetData(tokenId);
        string signedTx = Web3.OfflineTransactionSigner.SignTransaction(
            account.PrivateKey,
            contractAddress,
			bidPrice,
            nonce,
            gasPrice,
            500000,
            data
        );

		return signedTx;
	}
}
