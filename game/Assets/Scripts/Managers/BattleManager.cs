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
    public Player ActivePlayer => activePlayer;

    private int turnIndex;
    private int turnCount;
    private List<Player> players;

    private Dictionary<string, Player> playerIdToPlayer;
    public Dictionary<string, Player> PlayerIdToPlayer => playerIdToPlayer;

    public int battleLayer;
    public int boardLayer;
    //private List<HistoryItem> history;

    private BoardCreature mouseDownCreature;
    private Targetable mouseUpCreature;
    private List<Targetable> validTargets; //used to store/cache valid targets

    public CurvedLineRenderer attackCommand;

    public static BattleManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;

        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            if (!BattleSingleton.Instance.ChallengeStarted)
            {
                throw new Exception("Challenge not started - did you enter this scene from the matchmaking scene?");
            }
            else
            {
                Debug.Log("BattleManager in Connected Development Mode.");
            }

            this.you = new Player(BattleSingleton.Instance.PlayerState, "Player");
            this.opponent = new Player(BattleSingleton.Instance.OpponentState, "Enemy");
        }
        else
        {
            this.you = new Player("Player", "Player");
            this.opponent = new Player("Enemy", "Enemy");
        }

        this.playerIdToPlayer = new Dictionary<string, Player>();
        this.playerIdToPlayer[this.you.Id] = this.you;
        this.playerIdToPlayer[this.opponent.Id] = this.opponent;
        attackCommand.SetWidth(0);
    }

    private void Start()
    {
        // Use this for initialization
        battleLayer = 9;
        boardLayer = LayerMask.GetMask("Board");
        ChooseRandomSetting();
        GameStart();
    }

    private void ChooseRandomSetting()
    {
        Transform pool = GameObject.Find("Setting Pool").transform as Transform;
        foreach (Transform child in pool)
        {
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
                    attackCommand.SetPointPositions(mouseDownCreature.transform.position, hit.point);
                    attackCommand.SetWidth(1.33f);
                    //attackCommand.lineRenderer.enabled = true; //this is being used as a validity check!!
                    Cursor.SetCursor(ActionManager.Instance.cursors[4], Vector2.zero, CursorMode.Auto);
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
                OnEndTurnClick();
            }
            else if (cast && hit.collider.name == "Surrender")
            {
                Surrender();
            }
            //reset state
            attackCommand.SetWidth(0);
            mouseDownCreature = null;
            mouseUpCreature = null;
            validTargets = null;
            Cursor.SetCursor(ActionManager.Instance.cursors[0], Vector2.zero, CursorMode.Auto);
        }
        else if (attackCommand.lineRenderer.startWidth > 0 && Input.GetMouseButton(0))
        {
            RaycastHit hit;
            bool cast = Physics.Raycast(ray, out hit, 100f);
            if (cast)
            {
                attackCommand.SetPointPositions(mouseDownCreature.transform.position, hit.point);
                if (hit.collider.gameObject.layer == battleLayer && mouseDownCreature != null)
                    Cursor.SetCursor(ActionManager.Instance.cursors[5], Vector2.zero, CursorMode.Auto);
                else
                    Cursor.SetCursor(ActionManager.Instance.cursors[4], Vector2.zero, CursorMode.Auto);
            }
        }
    }

    private void GameStart()
    {
        players = new List<Player>();
        players.Add(this.you);
        players.Add(this.opponent);

        InitializeAvatar(you);
        InitializeAvatar(opponent);
        Board.Instance().AddPlayer(you);
        Board.Instance().AddPlayer(opponent);   //to-do make this into for loop
        Debug.Log(players.Count + " players in play.");

        if (!InspectorControlPanel.Instance.DevelopmentMode)
        {
            turnIndex = UnityEngine.Random.Range(0, players.Count);
            turnCount = 0;
            NextTurn();
        }
        else
        {
            turnIndex = this.players.FindIndex(player => player.HasTurn);
            turnCount = 0;
            activePlayer = players[turnIndex % players.Count];

            //do some turn transition render
            activePlayer.NewTurn();
        }
    }
    private void InitializeAvatar(Player player)
    {
        PlayerAvatar avatar = GameObject.Find(String.Format("{0} Avatar", player.Name)).GetComponent<PlayerAvatar>();
        avatar.Initialize(player);
    }

    private void OnEndTurnClick()
    {
        if (!InspectorControlPanel.Instance.DevelopmentMode)
        {
            NextTurn();
        }
        else
        {
            if (this.activePlayer.Id == this.you.Id)
            {
                BattleSingleton.Instance.SendChallengeEndTurnRequest();
                NextTurn();
            }
        }
    }

    private void NextTurn()
    {
        activePlayer.SetHasTurn(false);
        turnCount++;
        turnIndex++;
        activePlayer = players[turnIndex % players.Count];

        //do some turn transition render
        activePlayer.NewTurn();
    }


    private void Surrender()
    {
        Debug.LogWarning("Surrender action pressed.");
        BattleSingleton.Instance.SendChallengeSurrenderRequest();
    }

    private List<Targetable> GetValidTargets(BoardCreature attacker)
    {
        List<Targetable> allTargets = new List<Targetable>();
        foreach (Player player in players)
        {
            if (player == attacker.Owner)
                continue;
            BoardCreature[] fieldCreatures = Board.Instance().GetFieldByPlayerId(player.Id).GetCreatures();
            for (int i = 0; i < fieldCreatures.Length; i++)
            {
                if (fieldCreatures[i] != null)
                    allTargets.Add(fieldCreatures[i]);
            }
            allTargets.Add(player.avatar);
        }

        //Debug.LogWarning(allTargets.Count + " enemies found.");
        List<Targetable> priorityTargets = new List<Targetable>();
        for (int j = 0; j < allTargets.Count; j++)
        {
            if (allTargets[j].IsAvatar)
                continue;
            BoardCreature creature = allTargets[j] as BoardCreature;
            if (creature.HasAbility("taunt"))
                priorityTargets.Add(creature);
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

        if (!attackCommand.enabled)
            return;
        mouseUpCreature = hit.collider.GetComponent<Targetable>();
        if (mouseUpCreature == mouseDownCreature)
        {
            //show creature details
            Debug.Log("Show details.");
        }
        else if (validTargets.Count > 0 && validTargets.Contains(mouseUpCreature))
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

    /*
     * Play card to board after receiving play card move from server. 
     */
    public void PlayCardToBoard(CardObject cardObject, int index)
    {
        //to-do: animation here
        Player player = cardObject.card.Owner;
        Transform boardPlace = GameObject.Find(String.Format("{0} {1}", player.Name, index)).transform;
        //shared spawning + fx
        SpawnCardToBoard(cardObject, index, boardPlace);
    }
    /*
     * Play card to board after user on-device drags card from hand to field. 
     */
    public void PlayCardToBoard(CardObject cardObject, RaycastHit hit)
    {
        //only called for creature or structure
        Transform targetPosition = hit.collider.transform;
        string lastChar = hit.collider.name.Substring(hit.collider.name.Length - 1);
        int index = Int32.Parse(lastChar);
        Player player = cardObject.card.Owner;
        //shared spawning + fx
        SpawnCardToBoard(cardObject, index, hit.collider.transform);

        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            PlayCardAttributes attributes = new PlayCardAttributes(index);
            BattleSingleton.Instance.SendChallengePlayCardRequest(
                cardObject.card.Id,
                attributes
            );
        }
    }

    private void SpawnCardToBoard(CardObject cardObject, int index, Transform target)
    {
        FXPoolManager.Instance.PlayEffect("Spawn", target.position + new Vector3(0f, 0f, -0.1f));
        BoardCreature created = CreateBoardCreature(cardObject, cardObject.card.Owner, target.position);
        Board.Instance().PlaceCreature(created, index);
        UseCard(cardObject.card.Owner, cardObject);
    }

    public void PlayTargetedSpell(SpellCard spell, CardObject target, RaycastHit hit)
    {
        BoardCreature targetedCreature = hit.collider.GetComponent<BoardCreature>();
        spell.Activate(targetedCreature, "l_bolt");
        UseCard(target.card.Owner, target);
    }


    public void ReceiveMoveEndTurn(string playerId)
    {
        NextTurn();
    }


    public void ReceiveMoveDrawCard(string playerId, Card card)
    {
        this.activePlayer.Hand.AddDrawnCard(card);
    }

    public void ReceiveMovePlayMinion(string playerId, string cardId, Card card, int handIndex, int fieldIndex)
    {
        if (!InspectorControlPanel.Instance.DevelopmentMode)
        {
            CardObject target = opponent.Hand.GetCardById(cardId).wrapper;
            if (target == null)
            {
                Debug.LogError(String.Format("Server demanded card to play, but none of id {0} was found.", cardId));
                return;
            }
            PlayCardToBoard(target, fieldIndex);
        }
        else
        {
            CardObject target = opponent.Hand.GetCardByIndex(handIndex).wrapper;
            target.InitializeCard(card);
            PlayCardToBoard(target, fieldIndex);
            UseCard(card.Owner, target);
        }
    }

    public void ReceiveMovePlaySpell(string playerId, string cardId, Card card)
    {

    }

    public void ReceiveMoveCardAttack(string playerId, string cardId, string targetId)
    {

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
