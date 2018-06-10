using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class Marketplace : MonoBehaviour {

	private List<CardAuction> cardAuctions;

	private GameObject cardAuctionsGO;
    
	private void Awake()
	{
		cardAuctions = new List<CardAuction>();
		cardAuctionsGO = new GameObject("Card Auctions");

		GetCardAuctions();
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

		CreateAuctionsUI();
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
}
