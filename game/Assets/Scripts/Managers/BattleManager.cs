using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class BattleManager : MonoBehaviour
{
    private Player you;
    private Player opponent;

    [SerializeField]
    private Player activePlayer;
    private int turnIndex;
    private int turnCount;
    private List<Player> players;

    public int battleLayer;
    public int boardLayer;
    //private List<HistoryItem> history;

    private BoardCreature mouseDownCreature;
    private BoardCreature mouseUpCreature;
    private List<BoardCreature> validTargets; //used to store/cache valid targets

    public LineRenderer attackArrow;
    [SerializeField]
    List<BoardCreature> allTargets;

    public static BattleManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.you = new Player("Player");
        Debug.Log(this.you.Deck);
        this.opponent = new Player("Enemy");
    }

    private void Start()
    {
        // Use this for initialization
        battleLayer = 9;
        boardLayer = LayerMask.GetMask("Board");
        ChooseRandomSetting();
        GameStart();
    }

    private void ChooseRandomSetting() {
        Transform pool = GameObject.Find("Setting Pool").transform as Transform;
        foreach(Transform child in pool) {
            child.gameObject.SetActive(false);
        }
        pool.GetChild(UnityEngine.Random.Range(0, pool.childCount)).gameObject.SetActive(true);
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
                if (mouseDownCreature.Owner.HasTurn && mouseDownCreature.CanAttack > 0)
                {
                    validTargets = GetValidTargets(mouseDownCreature);
                    //to-do: don't show attack arrow unless mouse no longer in bounds of board creature
                    attackArrow.SetPosition(0, hit.collider.transform.position);
                    attackArrow.enabled = true; //this is being used as a validity check!!
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            bool cast = Physics.Raycast(ray, out hit, 100f);
            if (cast && hit.collider.gameObject.layer == battleLayer && mouseDownCreature != null) //use battle layer mask
            {
                CheckFight(mouseDownCreature, hit);
            }
            else if (CheckPlayCard(ray, hit))
            {
                //do something?
            }
            else if (cast && hit.collider.name == "End Turn")
            {
                NextTurn();
            }
            else if (cast && hit.collider.name == "Surrender")
            {
                Surrender();
            }
            //reset state
            mouseDownCreature = null;
            mouseUpCreature = null;
            attackArrow.enabled = false;
            validTargets = null;
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

    private void GameStart()
    {
        players = new List<Player>();
        players.Add(this.you);
        players.Add(this.opponent);

        Board.Instance().AddPlayer(you);
        Board.Instance().AddPlayer(opponent);   //to-do make this into for loop
        Debug.Log(players.Count + " players in play.");

        turnIndex = UnityEngine.Random.Range(0, players.Count);
        turnCount = 0;
        NextTurn();
    }


    private void NextTurn()
    {
		activePlayer.SetHasTurn(false);
        BattleSingleton.Instance.SendChallengeEndTurnRequest();

        turnCount++;
        turnIndex++;
        activePlayer = players[turnIndex % players.Count];

        //do some turn transition render
        activePlayer.NewTurn();
    }

    private void Surrender() {
        Debug.LogWarning("Surrender action pressed.");
    }

    private List<BoardCreature> GetValidTargets(BoardCreature attacker)
    {
        allTargets = new List<BoardCreature>();
        foreach (Player player in players)
        {
            if (player == attacker.Owner)
                continue;
            BoardCreature[] fieldCreatures = Board.Instance().GetField(player).GetCreatures();
            for (int i = 0; i < fieldCreatures.Length; i++)
            {
                if (fieldCreatures[i] != null)
                    allTargets.Add(fieldCreatures[i]);
            }

        }

        //Debug.LogWarning(allTargets.Count + " enemies found.");
        List<BoardCreature> priorityTargets = new List<BoardCreature>();
        for (int j = 0; j < allTargets.Count; j++)
        {
            if (allTargets[j].HasAbility("taunt"))
                priorityTargets.Add(allTargets[j]);
        }

        if (priorityTargets != null && priorityTargets.Count > 0)
        {
            return priorityTargets;
        }
        else
        {
            //Debug.LogWarning("No priority targets found. " + priorityTargets.Count);
            return allTargets;
        }
    }


    private void CheckFight(BoardCreature attacker, RaycastHit hit)
    {

        if (!attackArrow.enabled)
            return;
        mouseUpCreature = hit.collider.GetComponent<BoardCreature>();
        if (mouseUpCreature == mouseDownCreature)
        {
            //show creature details
            Debug.Log("Show details.");
        }
        else if (validTargets.Contains(mouseUpCreature))
        {
            mouseDownCreature.Fight(mouseUpCreature);
        }
    }


    private bool CheckPlayCard(Ray ray, RaycastHit hit)
    {

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
        else if (target.card.GetType() == typeof(SpellCard))
        {
            SpellCard spell = target.card as SpellCard;
            if (spell.Targeted)
            {
                if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Battle")))
                {
                    this.PlayTargetedSpell(spell, target, hit);
                    return true;
                }
                else
                {
                    ActionManager.Instance.ResetTarget();
                    return false;
                }
            }
            else  //not targeted spell, play freely
            {
                this.UseCard(target.card.Owner, target); //change to own board spell func
                return true;
            }
        }
        else if (target.card.GetType() == typeof(WeaponCard))
        {
            this.UseCard(target.card.Owner, target);    //to-do: change to own weapon func
            return true;
        }
        else if (Physics.Raycast(ray, out hit, 100f, boardLayer) && hit.collider.name.Contains(target.card.Owner.Name))
        {
            //place card
            Debug.Log("Mouse up with board playable card.");
            this.PlayCardToBoard(target, hit);
            return true;
        }
        else
        {
            //no good activation events, return to hand or original pos/rot in collection
            ActionManager.Instance.ResetTarget();
            return false;
        }
    }

    public void PlayCardToBoard(CardObject cardObject, RaycastHit hit)
    {
        Player player = cardObject.card.Owner;

        //only called for creature or structure
        Transform targetPosition = hit.collider.transform;
        string lastChar = hit.collider.name.Substring(hit.collider.name.Length - 1);
        int index = Int32.Parse(lastChar);

        FXPoolManager.Instance.PlayEffect("Spawn", hit.collider.transform.position + new Vector3(0f, 0f, -0.1f));
        BoardCreature created = CreateBoardCreature(cardObject, player, hit.collider.transform.position);
        Board.Instance().PlaceCreature(created, index);
        UseCard(player, cardObject);
    }

    public void PlayTargetedSpell(SpellCard spell, CardObject target, RaycastHit hit)
    {
        BoardCreature targetedCreature = hit.collider.GetComponent<BoardCreature>();
        spell.Activate(targetedCreature, "l_bolt");
        UseCard(target.card.Owner, target);
    }


    public void UseCard(Player player, CardObject cardObject)
    {

        player.PlayCard(cardObject);    //removes card from hand, spend mana
        GameObject.Destroy(cardObject.gameObject);
        SoundManager.Instance.PlaySound("Play", transform.position);
    }


    public BoardCreature CreateBoardCreature(CardObject cardObject, Player owner, Vector3 pos)
    {

        GameObject created = new GameObject(cardObject.card.Name);
        created.transform.position = pos;
        BoardCreature creature = created.AddComponent<BoardCreature>();
        creature.Initialize(cardObject, owner);
        return creature;
    }

    public PlayerState GetPlayerState()
    {
        return this.you.GeneratePlayerState();
    }

    public PlayerState GetOpponentState()
    {
        return this.opponent.GeneratePlayerState();
    }
}
