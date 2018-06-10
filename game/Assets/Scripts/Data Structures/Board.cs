using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board {
    private Dictionary<Player, PlayingField> fields;
    private BattleManager manager;

    public Board() {
        
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

    public class PlayingField {
        protected List<Card> creatures;
        //List<Artifact> artifacts;

        void Initialize() {
            creatures = new List<Card>(6);
        }
    }
}
