﻿using System;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;

public static class TreasuryContract
{
    private static string contractAbi = "[{'constant':true,'inputs':[],'name':'name','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'pure','type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_tokenId','type':'uint256'}],'name':'approve','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'instancesSupply','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_from','type':'address'},{'name':'_to','type':'address'},{'name':'_tokenId','type':'uint256'}],'name':'transferFrom','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_templateId','type':'uint256'}],'name':'getTemplate','outputs':[{'name':'generation','type':'uint128'},{'name':'power','type':'uint128'},{'name':'name','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'templatesSupply','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_tokenId','type':'uint256'},{'name':'_startingPrice','type':'uint256'},{'name':'_endingPrice','type':'uint256'},{'name':'_duration','type':'uint256'}],'name':'createSaleAuction','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_templateId','type':'uint256'}],'name':'instanceLimit','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_tokenId','type':'uint256'}],'name':'ownerOf','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[],'name':'renounceOwnership','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'tokensOfOwner','outputs':[{'name':'','type':'uint256[]'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'owner','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_cardId','type':'uint256'}],'name':'getCard','outputs':[{'name':'generation','type':'uint128'},{'name':'power','type':'uint128'},{'name':'name','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'symbol','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'pure','type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_tokenId','type':'uint256'}],'name':'transfer','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'_address','type':'address'}],'name':'setSaleAuction','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_tokenId','type':'uint256'}],'name':'templateIdOf','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'saleAuction','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_templateId','type':'uint256'},{'name':'_owner','type':'address'}],'name':'mintCard','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'_mintLimit','type':'uint256'},{'name':'_generation','type':'uint128'},{'name':'_power','type':'uint128'},{'name':'_name','type':'string'}],'name':'createTemplate','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'newOwner','type':'address'}],'name':'transferOwnership','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'anonymous':false,'inputs':[{'indexed':false,'name':'templateId','type':'uint256'}],'name':'TemplateCreated','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'owner','type':'address'},{'indexed':false,'name':'cardId','type':'uint256'},{'indexed':false,'name':'templateId','type':'uint256'}],'name':'InstanceMinted','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'from','type':'address'},{'indexed':true,'name':'to','type':'address'},{'indexed':false,'name':'tokenId','type':'uint256'}],'name':'Transfer','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'owner','type':'address'},{'indexed':false,'name':'approved','type':'address'},{'indexed':false,'name':'tokenId','type':'uint256'}],'name':'Approval','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'previousOwner','type':'address'}],'name':'OwnershipRenounced','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'previousOwner','type':'address'},{'indexed':true,'name':'newOwner','type':'address'}],'name':'OwnershipTransferred','type':'event'}]";

    public static string SignCreateAuctionTransaction(
        Account account,
        int nonce,
        int tokenId,
        long startingPrice,
        long endingPrice,
        int duration
    )
    {
        string contractAddress = MarketplaceManager.Instance.ContractAddressTreasury;
        string gasPriceSuggested = MarketplaceManager.Instance.GasPriceSuggested;
        long gasPrice = Int64.Parse(gasPriceSuggested);

        Web3 web3 = new Web3();
        Contract contract = web3.Eth.GetContract(contractAbi, contractAddress);

        Function createAuctionFunction = contract.GetFunction("createSaleAuction");
        string data = createAuctionFunction.GetData(
            tokenId,
            startingPrice,
            endingPrice,
            duration
        );
        string signedTx = Web3.OfflineTransactionSigner.SignTransaction(
            account.PrivateKey,
            contractAddress,
            0,
            nonce,
            gasPrice,
            200000, // Gas limit.
            data
        );

        return signedTx;
    }
}
