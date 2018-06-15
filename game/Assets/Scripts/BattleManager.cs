using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class BattleManager : MonoBehaviour {
    public Player you;
    public Player opponent;
    [SerializeField]
    private Board board;

    private int turn;
    public int battleLayer;
    public int boardLayer;
    public GameObject spawnFX;
    //private List<HistoryItem> history;

    private BoardCreature mouseDownCreature;
    private BoardCreature mouseUpCreature;

    private ActionManager actionManager;
    public LineRenderer attackArrow;

    // Use this for initialization
    private void Awake()
    {
        you = new Player("Player");
        opponent = new Player("Enemy");
        board = new Board();

        battleLayer = LayerMask.GetMask("Battle");
        boardLayer = LayerMask.GetMask("Board");
        actionManager = Camera.main.GetComponent<ActionManager>();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, battleLayer)) //use battle layer mask
            {
                mouseDownCreature = hit.collider.GetComponent<BoardCreature>();
                attackArrow.SetPosition(0, hit.collider.transform.position);
                attackArrow.enabled = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, battleLayer)) //use battle layer mask
            {
                CheckFight(hit);
            }
            CheckPlayCard(ray, hit);
            //reset state
            mouseUpCreature = null;
            attackArrow.enabled = false;
        }
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                attackArrow.SetPosition(1, hit.point);
            }
        }
    }

    private void CheckFight(RaycastHit hit) {
        mouseUpCreature = hit.collider.GetComponent<BoardCreature>();
        if (mouseUpCreature == mouseDownCreature)
        {
            //show creature details
            Debug.Log("Show details.");
        }
        else if (mouseUpCreature.Owner != mouseDownCreature.Owner)
        { //should wrap in a helper function isEnemy() for 2v2
          //valid attack
            Debug.Log("Attack made.");
            mouseDownCreature.Fight(mouseUpCreature);
        }
    }

    private void CheckPlayCard(Ray ray, RaycastHit hit) {
        //TODO: check target.card.type for spell or weapon first
        if (!actionManager.HasDragTarget())
            return;

        CardObject target = actionManager.GetDragTarget();
        //TODO: think about using displacement distance as activation indicator vs screen height
        float minThreshold = 0.20f;
        if (target.card.Cost > target.card.Owner.Mana)
        {
            //can't play card due to mana
            actionManager.ResetTarget();
        }
        else if (Input.mousePosition.y < Screen.height * minThreshold)
        {
            //didn't lift card high enough to activate
            actionManager.ResetTarget();
        }
        else if (target.card.Category == Card.CardType.Spell || target.card.Category == Card.CardType.Weapon)
        {
            this.PlayCardGeneric(target.card.Owner, target);
        }
        else if (Physics.Raycast(ray, out hit, 100f, boardLayer) && hit.collider.name.Contains(target.card.Owner.Name))
        {
            //place card
            Debug.Log("Mouse up with board playable card.");
            this.PlayCardToBoard(target.card.Owner, target, hit);
        }
        else
        {
            //no good activation events, return to hand or original pos/rot in collection
            actionManager.ResetTarget();
        }
    }

    public void PlayCardToBoard(Player player, CardObject cardObject, RaycastHit hit) {
        //only called for creature or structure
        Transform targetPosition = hit.collider.transform;
        string lastChar = hit.collider.name.Substring(hit.collider.name.Length-1);
        int index = Int32.Parse(lastChar);

        board.PlaceCard(player, cardObject.card, index);
        GameObject fx = Instantiate(spawnFX, hit.collider.transform.position + new Vector3(0f, 0f, -0.1f), Quaternion.identity);
        CreateBoardCreature(cardObject, player, hit.collider.transform.position);
        PlayCardGeneric(player, cardObject);
    }

    public void PlayCardGeneric(Player player, CardObject cardObject) {
        player.PlayCard(cardObject);    //removes card from hand, spend mana
        Destroy(cardObject.gameObject);
    }

    public void CreateBoardCreature(CardObject cardObject, Player owner, Vector3 pos) {
        GameObject created = new GameObject(cardObject.card.Name);
        created.transform.position = pos;
        BoardCreature creature = created.AddComponent<BoardCreature>();
        creature.Initialize(cardObject, owner);
    }


}
