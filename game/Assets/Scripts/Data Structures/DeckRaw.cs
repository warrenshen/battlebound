using System.Collections;
using System.Collections.Generic;

public class DeckRaw
{
    public string name;
    public List<string> cardIds;

    public enum DeckClass : byte { Warrior, Hunter }; //TODO: add more
    public DeckClass deckClass;

    public DeckRaw(string name, List<string> cardIds, DeckRaw.DeckClass deckClass)
    {
        this.name = name;
        this.cardIds = cardIds;
        this.deckClass = deckClass;
    }

    public int Size()
    {
        return cardIds.Count;
    }

    public override string ToString()
    {
        string output = "";
        foreach (string cardId in cardIds)
        {
            output += cardId + ", ";
        }
        return output.Substring(0, output.Length - 1);
    }
}
