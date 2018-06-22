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

    public void PlaceCreature(BoardCreature creature, int position) {
        if (!playerToFields.ContainsKey(creature.Owner)) {
            AddPlayer(creature.Owner);
        }
        PlayingField selected = playerToFields[creature.Owner];
        selected.Place(creature, position);
    }

    public void RemoveCreature(BoardCreature creature) {
        PlayingField selected = playerToFields[creature.Owner];
        selected.Remove(creature);
    }
    
	public void AddPlayer (Player player) {
        PlayingField created = new PlayingField();
        player.SetPlayingField(created);
        playerToFields[player] = created;
	}

    public PlayingField GetField(Player player) {
        return playerToFields[player];
    }


    [System.Serializable]
    public class PlayingField {
        
        [SerializeField]
        protected BoardCreature[] creatures;
        //List<Artifact> artifacts;

        public PlayingField() {
            creatures = new BoardCreature[6];
        }
        
		public BoardCreature GetCreatureByIndex(int index)
		{
			return this.creatures[index];
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

        public void Remove(BoardCreature creature) {
            for (int i = 0; i < creatures.Length; i++) {
                if (creatures[i] == creature)
                    creatures[i] = null;
            }
        }

        public BoardCreature[] GetCreatures() {
            return creatures;
        }

        public void RecoverCreatures() {
            for (int i = 0; i < creatures.Length; i++)
            {
                if (creatures[i] != null)
                    creatures[i].RecoverAttack();
            }
        }
    }
}
