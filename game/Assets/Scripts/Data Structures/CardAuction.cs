using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardAuction
{
	[SerializeField]
	private string id;
	public string Id => id;

	[SerializeField]
	private string name;
	public string Name => name;

	[SerializeField]
	private string image;
	public string Image => image;

	[SerializeField]
    private string seller;
    public string Seller => seller;

	[SerializeField]
	private Auction auction;
	public Auction Auction => auction;

	public CardAuctionObject wrapper;
}

[System.Serializable]
public class Auction
{
	[SerializeField]
	private int startingPrice;
	public int StartingPrice => startingPrice;

	[SerializeField]
	private int endingPrice;
	public int EndingPrice => endingPrice;

	[SerializeField]
	private int startedAt;
	public int StartedAt => startedAt;

	[SerializeField]
	private int duration;
	public int Duration => duration;
}
