using UnityEngine;

[System.Serializable]
public class CardAuction
{
    [SerializeField]
    private string name;

    [SerializeField]
    private int category;
    public int Category => category;

    [SerializeField]
    private Card card;
    public Card Card => card;

    [SerializeField]
    private string seller;
    public string Seller => seller;

    [SerializeField]
    private AuctionDetails auction;
    public AuctionDetails Auction => auction;

    [System.Serializable]
    public class AuctionDetails
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

    public static CardAuction GetFromJson(string json)
    {
        CardAuction cardAuction = JsonUtility.FromJson<CardAuction>(json);

        Card card;
        switch (cardAuction.Category)
        {
            case (int)Card.CardType.Creature: //creature
                card = CreatureCard.GetFromJson(json);
                break;
            case (int)Card.CardType.Spell:  //spell
                card = SpellCard.GetFromJson(json);
                break;
            case (int)Card.CardType.Weapon:  //weapon
                card = WeaponCard.GetFromJson(json);
                break;
            case (int)Card.CardType.Structure:  //structure
                card = StructureCard.GetFromJson(json);
                break;
            default:
                Debug.LogError("Card has no category/type field!");
                return null;
        }

        cardAuction.SetCard(card);

        return cardAuction;
    }

    public string GetId()
    {
        return this.card.Id;
    }

    public void SetCard(Card card)
    {
        this.card = card;
    }
}

