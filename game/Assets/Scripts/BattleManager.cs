using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleManager : MonoBehaviour {
    public Player you;
    public Player opponent;
    private Board board;

    private int turn;
    public GameObject spawnFX;
    //private List<HistoryItem> history;

    // Use this for initialization
    private void Awake()
    {
        you = new Player("Player");
        opponent = new Player("Enemy");
        board = new Board();
    }

    public void PlayCardToBoard(Player player, CardObject cardObject, RaycastHit hit) {
        //only called for creature or structure
        Transform targetPosition = hit.collider.transform;
        string lastChar = hit.collider.name.Substring(hit.collider.name.Length-1);
        int index = Int32.Parse(lastChar);

        board.PlaceCard(player, cardObject.card, index);
        GameObject fx = Instantiate(spawnFX, hit.collider.transform.position, Quaternion.identity);
        PlayCardGeneric(player, cardObject);
    }

    public void PlayCardGeneric(Player player, CardObject cardObject) {
        player.PlayCard(cardObject);
        Destroy(cardObject.gameObject);
    }
}
