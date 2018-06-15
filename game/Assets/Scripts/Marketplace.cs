using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class Marketplace : MonoBehaviour {

	private List<CardAuction> cardAuctions;
	//private List<CardAuction> activePlayerCardAuctions;

	private GameObject cardAuctionsGO;
    
	private void Awake()
	{
		CreateAuctionModalPanel.Instance().HideModal();

		cardAuctions = new List<CardAuction>();
		cardAuctionsGO = new GameObject("Card Auctions");

		GetCardAuctions();
		GetPlayerCardAuctions();
	}

	private void GetCardAuctions()
	{
		LogEventRequest request = new LogEventRequest();
		request.SetEventKey("GetCardAuctions");
		request.Send(OnGetCardAuctionsSuccess, OnGetCardAuctionsError);
	}

	private void OnGetCardAuctionsSuccess(LogEventResponse response)
	{
		// JSON list of card auctions.
		List<GSData> dataList = response.ScriptData.GetGSDataList("auctions");
		Debug.Log(dataList.Count + " auctions found.");

		foreach (GSData data in dataList)
		{
			CardAuction cardAuction = JsonUtility.FromJson<CardAuction>(data.JSON);
			cardAuctions.Add(cardAuction);
		}

		//CreateAuctionsUI();
	}

	private void OnGetCardAuctionsError(LogEventResponse response)
	{
		GSData errors = response.Errors;
		Debug.Log(errors);
	}

    private void CreateAuctionsUI()
	{
		int index = 0;
        int rowSize = 4;

        Vector3 topLeft = new Vector3(-5.05f, 3.11f, 18.18f);
        Vector3 horizontalOffset = new Vector3(4.64f, 0f, 0f);
        Vector3 verticalOffset = new Vector3(0f, -3.75f, 0f);

        Transform grayed = new GameObject("Grayed").transform as Transform;
        foreach (CardAuction cardAuction in cardAuctions)
        {
            GameObject cardAuctionGO = new GameObject(cardAuction.Name);
			cardAuctionGO.transform.parent = cardAuctionsGO.transform;
			CardAuctionObject wrapper = cardAuctionGO.AddComponent<CardAuctionObject>();
            wrapper.InitializeCardAuction(cardAuction);

			cardAuctionGO.transform.position = topLeft + index % rowSize * horizontalOffset + index / rowSize * verticalOffset;
            index++;
        }
	}

    private void GetPlayerCardAuctions()
	{
		LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetPlayerCardAuctions");
		request.Send(OnGetPlayerCardAuctionsSuccess, OnGetPlayerCardAuctionsError);
	}

	private void OnGetPlayerCardAuctionsSuccess(LogEventResponse response)
	{
		// JSON list of card auctions.
		List<GSData> auctionableDataList = response.ScriptData.GetGSDataList("auctionableCards");
		Debug.Log(auctionableDataList.Count + " auctionable cards found.");

		List<GSData> auctionedDataList = response.ScriptData.GetGSDataList("auctionedCards");
		Debug.Log(auctionedDataList.Count + " auctioned cards found.");

		List<Card> auctionableCards = new List<Card>();
		foreach (GSData data in auctionableDataList)
        {
            Card card = JsonUtility.FromJson<Card>(data.JSON);
			auctionableCards.Add(card);
        }

		CreatePlayerAuctionsUI(auctionableCards);

		List<Card> auctionedCards = new List<Card>();
		foreach (GSData data in auctionedDataList)
        {
            Card card = JsonUtility.FromJson<Card>(data.JSON);
			auctionedCards.Add(card);
        }

        // Render auctioned cards.
	}

	private void OnGetPlayerCardAuctionsError(LogEventResponse response)
	{
		GSData errors = response.Errors;
        Debug.Log(errors);
	}

	private void CreatePlayerAuctionsUI(List<Card> auctionableCards)
	{
		int index = 0;
        int rowSize = 4;

        Vector3 topLeft = new Vector3(-5.05f, 3.11f, 18.18f);
        Vector3 horizontalOffset = new Vector3(4.64f, 0f, 0f);
        Vector3 verticalOffset = new Vector3(0f, 3.75f, 0f);
        
        Transform grayed = new GameObject("Grayed").transform as Transform;
		foreach (Card auctionableCard in auctionableCards)
        {
			GameObject cardGO = new GameObject(auctionableCard.Name);
			cardGO.transform.parent = cardAuctionsGO.transform;
			AuctionableCardObject wrapper = cardGO.AddComponent<AuctionableCardObject>();
			wrapper.InitializeCard(auctionableCard);

			cardGO.transform.position = topLeft + index % rowSize * horizontalOffset + index / rowSize * verticalOffset;
            index++;
        }
	}
}
