﻿using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.Web3.Accounts;

public class CreateAuctionModalPanel : MonoBehaviour
{
    private Card card;

    [SerializeField]
    private InputField startingPriceInputField;
    [SerializeField]
    private InputField endingPriceInputField;
    [SerializeField]
    private InputField durationInputField;
    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private Button submitButton;

    public static CreateAuctionModalPanel Instance { get; private set; }

    public void Awake()
    {
        Debug.Log(PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS));
        Instance = this;

        this.cancelButton.onClick.AddListener(Close);
        //this.submitButton.onClick.AddListener(AuthorizeBidAuction);

        Close();
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowModalForCard(Card card)
    {
        this.card = card;
        this.gameObject.SetActive(true);
    }


    private void SubmitCreateAuctionTransaction(Account account)
    {
        int tokenId = Convert.ToInt32(card.Id.Substring(1));
        long startingPrice = Convert.ToInt64(startingPriceInputField.text);
        long endingPrice = Convert.ToInt64(endingPriceInputField.text);
        int duration = Convert.ToInt32(durationInputField.text);

        CryptoSingleton.Instance.CreateAuction(
            account,
            tokenId,
            startingPrice,
            endingPrice,
            duration
        );

        //Close();

        //return txHash;
    }
}
