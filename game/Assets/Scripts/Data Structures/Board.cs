using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Board
{
    [SerializeField]
    private Dictionary<Player, PlayingField> playerToFields;
    private static Board instance;

    public static Board Instance()
    {
        if (instance == null)
        {
            instance = new Board();
        }
        return instance;
    }


    public Board() {
        //create field for each player
        playerToFields = new Dictionary<Player, PlayingField>();
    }

    public void PlaceCreature(BoardCreature creature, Player player, int position) {
        if (!playerToFields.ContainsKey(player)) {
            AddPlayer(player);
        }
        PlayingField selected = playerToFields[player];
        selected.Place(creature, position);

    }

	// Update is called once per frame
	void AddPlayer (Player player) {
        PlayingField created = new PlayingField();
        player.SetPlayingField(created);
        playerToFields[player] = created;
	}


    [System.Serializable]
    public class PlayingField {
        
        [SerializeField]
        protected BoardCreature[] creatures;
        //List<Artifact> artifacts;

        public PlayingField() {
            creatures = new BoardCreature[6];
        }

        public bool Place(BoardCreature creature, int index) {
            if(creatures[index] != null) {
                Debug.LogError("Attempting to place unit where one exists.");
                return false;
            }
            else {
                creatures[index] = creature;
                return true;
            }
        }
    }
}
