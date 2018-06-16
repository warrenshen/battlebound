using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Board {
    [SerializeField]
    private Dictionary<Player, PlayingField> fields;

    public Board() {
        //create field for each player
    }

    public void PlaceCard(Player player, Card card, int position) {
        Debug.Log("Played card.");
    }

	// Update is called once per frame
	void AddPlayer (Player player) {
        PlayingField created = new PlayingField();
        player.SetPlayingField(created);
        fields[player] = created;
	}

    [System.Serializable]
    public class PlayingField {
        [SerializeField]
        protected List<Card> creatures;
        //List<Artifact> artifacts;

        void Initialize() {
            creatures = new List<Card>(6);
        }
    }
}
