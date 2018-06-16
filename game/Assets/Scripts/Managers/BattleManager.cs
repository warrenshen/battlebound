using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class BattleManager : MonoBehaviour {
    private Player you;
    private Player opponent;
    [SerializeField]
    private Board board;

    [SerializeField]
    private Player activePlayer;
    private int turnIndex;
    private int turnCount;
    private List<Player> players;

    public int battleLayer;
    public int boardLayer;
    public GameObject spawnFX;
    //private List<HistoryItem> history;

    private BoardCreature mouseDownCreature;
    private BoardCreature mouseUpCreature;

    public LineRenderer attackArrow;

    public static BattleManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.you = new Player("Player");
        this.opponent = new Player("Enemy");
        this.board = new Board();
    }

    private void Start()
    {
        // Use this for initialization
        battleLayer = 9;
        boardLayer = LayerMask.GetMask("Board");
        GameStart();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f) && hit.collider.gameObject.layer == battleLayer) //use battle layer mask
            {
                mouseDownCreature = hit.collider.GetComponent<BoardCreature>();
                if (mouseDownCreature.Owner.active)
                {
                    //to-do: don't show attack arrow unless mouse no longer in bounds of board creature
                    attackArrow.SetPosition(0, hit.collider.transform.position);
                    attackArrow.enabled = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            bool cast = Physics.Raycast(ray, out hit, 100f);
            if (cast && hit.collider.gameObject.layer == battleLayer && mouseDownCreature != null) //use battle layer mask
            {
                CheckFight(hit);
            }
            else if (CheckPlayCard(ray, hit))
            {
                //do something?
            }
            else if (cast && hit.collider.name == "End Turn")
            {
                NextTurn();
            }
            //reset state
            mouseDownCreature = null;
            mouseUpCreature = null;
            attackArrow.enabled = false;
        }

        //render attack arrow correctly
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                attackArrow.SetPosition(1, hit.point);
            }
        }
    }


    private void GameStart() {
        players = new List<Player>();
        players.Add(you);
        players.Add(opponent);

        turnIndex = UnityEngine.Random.Range(0, players.Count);
        turnCount = 0;
        NextTurn();
    }


    private void NextTurn() {
        activePlayer.active = false;
        turnCount++;
        turnIndex++;
        activePlayer = players[turnIndex % players.Count];

        //do some turn transition render
        activePlayer.NewTurn();

    }


    private void CheckFight(RaycastHit hit) {
        
        if (!attackArrow.enabled)
            return;
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


    private bool CheckPlayCard(Ray ray, RaycastHit hit) {
        
        //TODO: check target.card.type for spell or weapon first
        if (!ActionManager.Instance.HasDragTarget())
            return false;
        CardObject target = ActionManager.Instance.GetDragTarget();
        //TODO: think about using displacement distance as activation indicator vs screen height
        float minThreshold = 0.20f;
        if (target.card.Cost > target.card.Owner.Mana)
        {
            //can't play card due to mana
            ActionManager.Instance.ResetTarget();
            return false;
        }
        else if (Input.mousePosition.y < Screen.height * minThreshold)
        {
            //didn't lift card high enough to activate
            ActionManager.Instance.ResetTarget();
            return false;
        }
        else if (target.card.GetType() == typeof(SpellCard) || target.card.GetType() == typeof(WeaponCard))
        {
            this.PlayCardGeneric(target.card.Owner, target);
            return true;
        }
        else if (Physics.Raycast(ray, out hit, 100f, boardLayer) && hit.collider.name.Contains(target.card.Owner.Name))
        {
            //place card
            Debug.Log("Mouse up with board playable card.");
            this.PlayCardToBoard(target.card.Owner, target, hit);
            return true;
        }
        else
        {
            //no good activation events, return to hand or original pos/rot in collection
            ActionManager.Instance.ResetTarget();
            return false;
        }
    }

    public void PlayCardToBoard(Player player, CardObject cardObject, RaycastHit hit) {
        
        //only called for creature or structure
        Transform targetPosition = hit.collider.transform;
        string lastChar = hit.collider.name.Substring(hit.collider.name.Length-1);
        int index = Int32.Parse(lastChar);

        board.PlaceCard(player, cardObject.card, index);
        GameObject fx = GameObject.Instantiate(spawnFX, hit.collider.transform.position + new Vector3(0f, 0f, -0.1f), Quaternion.identity);
        CreateBoardCreature(cardObject, player, hit.collider.transform.position);
        PlayCardGeneric(player, cardObject);
    }


    public void PlayCardGeneric(Player player, CardObject cardObject) {
        
        player.PlayCard(cardObject);    //removes card from hand, spend mana
        GameObject.Destroy(cardObject.gameObject);
    }


    public void CreateBoardCreature(CardObject cardObject, Player owner, Vector3 pos) {
        
        GameObject created = new GameObject(cardObject.card.Name);
        created.transform.position = pos;
        BoardCreature creature = created.AddComponent<BoardCreature>();
        creature.Initialize(cardObject, owner);
    }
}
