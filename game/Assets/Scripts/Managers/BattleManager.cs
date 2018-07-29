using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using TMPro;

[System.Serializable]
public class BattleManager : MonoBehaviour
{
    private bool initialized;
    public bool Initialized => initialized;

    private Player you;
    public Player You => you;
    private Player opponent;
    public Player Opponent => opponent;

    [SerializeField]
    private Player activePlayer;
    public Player ActivePlayer => activePlayer;

    private int turnIndex;
    private List<Player> players;

    private Dictionary<string, Player> playerIdToPlayer;
    public Dictionary<string, Player> PlayerIdToPlayer => playerIdToPlayer;

    public int battleLayer;
    public int boardOrBattleLayer;
    //private List<HistoryItem> history;

    [SerializeField]
    private Targetable mouseDownTargetable;
    private Targetable mouseUpTargetable;
    private List<Targetable> validTargets; //used to store/cache valid targets

    public CurvedLineRenderer attackCommand;

    private int mode;

    private const int BATTLE_STATE_NORMAL_MODE = 0;
    private const int BATTLE_STATE_MULLIGAN_MODE = 1;

    private int spawnCount;
    public int SpawnCount => spawnCount;

    private int deviceMoveCount;
    private int serverMoveCount;

    private List<ChallengeMove> serverMoveQueue;
    private List<ChallengeMove> deviceMoveQueue;

    private List<ChallengeMove> serverMoves;

    // Cached transforms.
    [SerializeField]
    private Transform enemyPlayCardFixedTransform;
    [SerializeField]
    private Transform enemyDrawCardFixedTransform;
    [SerializeField]
    private Transform playerDrawCardFixedTransform;
    [SerializeField]
    private BasicButton endTurnButton;

    [SerializeField]
    private GameObject endOverlay;

    [SerializeField]
    private List<CardObject> xpCardObjects;

    public static BattleManager Instance { get; private set; }

    public Player GetPlayerById(string playerId)
    {
        return this.playerIdToPlayer[playerId];
    }

    private void Awake()
    {
        Instance = this;

        if (DeveloperPanel.IsServerEnabled())
        {
            if (!BattleSingleton.Instance.ChallengeStarted)
            {
                if (SparkSingleton.Instance.IsAuthenticated)
                {
                    SendFindMatchRequest();
                }
                else
                {
                    SparkSingleton.Instance.AddAuthenticatedCallback(new UnityAction(SendFindMatchRequest));
                }
                this.initialized = false;
                return;
            }
            else
            {
                Debug.Log("BattleManager in Connected Development Mode.");
                this.initialized = true;
            }
        }

        attackCommand.SetWidth(0);
    }

    private void SendFindMatchRequest()
    {
        BattleSingleton.Instance.SendFindMatchRequest(MatchmakingManager.MATCH_TYPE_CASUAL, "Deck1");
    }

    private void Update()
    {
        if (this.mode != BATTLE_STATE_MULLIGAN_MODE)
        {
            WatchMouseActions();
        }
    }

    private void Start()
    {
        // Initialize server and device move counts to be that of the server,
        // since we could be resuming a battle.
        this.serverMoveCount = BattleSingleton.Instance.MoveCount;
        this.deviceMoveCount = BattleSingleton.Instance.MoveCount;

        this.serverMoveQueue = new List<ChallengeMove>();
        this.deviceMoveQueue = new List<ChallengeMove>();
        this.serverMoves = new List<ChallengeMove>();

        this.playerIdToPlayer = new Dictionary<string, Player>();
        this.players = new List<Player>();

        battleLayer = LayerMask.NameToLayer("Battle");
        boardOrBattleLayer = LayerMask.GetMask(new string[2] { "Board", "Battle" });

        ChooseRandomSetting();

        if (DeveloperPanel.IsServerEnabled())
        {
            if (this.initialized)
            {
                this.spawnCount = BattleSingleton.Instance.SpawnCount;

                this.you = new Player(BattleSingleton.Instance.PlayerState, "Player");
                this.opponent = new Player(BattleSingleton.Instance.OpponentState, "Enemy");

                Board.Instance.RegisterPlayer(this.you, BattleSingleton.Instance.PlayerState.Field);
                Board.Instance.RegisterPlayer(this.opponent, BattleSingleton.Instance.OpponentState.Field);
                Board.Instance.RegisterPlayerOpponent(this.you.Id, this.opponent.Id);
                Board.Instance.RegisterPlayerOpponent(this.opponent.Id, this.you.Id);

                this.playerIdToPlayer[this.you.Id] = this.you;
                this.playerIdToPlayer[this.opponent.Id] = this.opponent;

                this.you.Initialize(BattleSingleton.Instance.PlayerState);
                this.opponent.Initialize(BattleSingleton.Instance.OpponentState);

                this.players.Add(this.you);
                this.players.Add(this.opponent);

                GameStart();
            }
        }
        else
        {
            this.spawnCount = 0;

            this.you = new Player("Player", "Player");
            this.opponent = new Player("Enemy", "Enemy");

            Board.Instance.RegisterPlayer(this.you);
            Board.Instance.RegisterPlayer(this.opponent);
            Board.Instance.RegisterPlayerOpponent(this.you.Id, this.opponent.Id);
            Board.Instance.RegisterPlayerOpponent(this.opponent.Id, this.you.Id);

            this.playerIdToPlayer[this.you.Id] = this.you;
            this.playerIdToPlayer[this.opponent.Id] = this.opponent;

            this.players.Add(this.you);
            this.players.Add(this.opponent);

            GameStart();
        }
    }

    private void GameStart()
    {
        if (DeveloperPanel.IsServerEnabled())
        {
            this.turnIndex = this.players.FindIndex(player => player.HasTurn);
            this.activePlayer = this.players[turnIndex % players.Count];
        }
        else
        {
            this.turnIndex = UnityEngine.Random.Range(0, players.Count);
            this.activePlayer = players[turnIndex % players.Count];
        }

        Player inactivePlayer = Board.Instance.GetOpponentByPlayerId(this.activePlayer.Id);

        if (DeveloperPanel.IsServerEnabled())
        {
            if (this.you.IsModeMulligan())
            {
                this.you.ResumeMulligan(
                    BattleSingleton.Instance.GetMulliganCards(this.you.Id)
                );
                this.opponent.ResumeMulligan(
                    BattleSingleton.Instance.GetMulliganCards(this.opponent.Id)
                );
                this.mode = BATTLE_STATE_MULLIGAN_MODE;

                activePlayer.RenderGameStart();
                inactivePlayer.RenderGameStart();
            }
            else
            {
                HideMulliganOverlay(this.you);
                HideMulliganOverlay(this.opponent);

                activePlayer.RenderTurnStart();
                inactivePlayer.RenderGameStart();
            }
        }
        else
        {
            this.mode = BATTLE_STATE_MULLIGAN_MODE;

            if (DeveloperPanel.ShouldSkipMulligan())
            {
                HideMulliganOverlay(this.you);
                HideMulliganOverlay(this.opponent);

                this.activePlayer.DrawCardsForce(3);
                inactivePlayer.DrawCardsForce(4);
            }
            else
            {
                this.activePlayer.BeginMulligan(this.activePlayer.PopCardsFromDeck(3));
                inactivePlayer.BeginMulligan(inactivePlayer.PopCardsFromDeck(4));
            }
        }
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

    private void AttackStartMade(RaycastHit hit)
    {
        ActionManager.Instance.SetActive(false);
        validTargets = GetValidTargets(mouseDownTargetable);
        //to-do: don't show attack arrow unless mouse no longer in bounds of board creature
        attackCommand.SetPointPositions(mouseDownTargetable.transform.position, hit.point);
        attackCommand.SetWidth(1.66f);
        //attackCommand.lineRenderer.enabled = true; //this is being used as a validity check!!
        ActionManager.Instance.SetCursor(4);
    }

    private void WatchMouseActions()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 100f) || hit.collider.gameObject.layer != battleLayer) //use battle layer mask
            {
                return;
            }

            mouseDownTargetable = hit.collider.GetComponent<Targetable>();
            if (mouseDownTargetable == null)
            {
                Debug.LogError("Raycast hit an object in battle layer that is not of class Targetable...");
                return;
            }
            //Trigger any events as needed
            mouseDownTargetable.MouseDown();

            if (DeveloperPanel.IsServerEnabled() && mouseDownTargetable.Owner.Id != this.you.Id)
            {
                return;
            }

            if (!mouseDownTargetable.CanAttackNow())
            {
                return;
            }

            if (mouseDownTargetable.IsAvatar)
            {
                PlayerAvatar avatar = mouseDownTargetable.Owner.Avatar;
                if (avatar.HasWeapon())
                {
                    AttackStartMade(hit);
                }
            }
            else  // must be BoardCreature, otherwise would have returned
            {
                AttackStartMade(hit);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            bool cast = Physics.Raycast(ray, out hit, 100f);
            if (cast && CheckFight(mouseDownTargetable, hit)) //use battle layer mask
            {
                //do something?
            }
            else if (cast && CheckPlayCard(ray, hit))
            {
                //do something?
            }
            //reset state
            ActionManager.Instance.SetActive(true);
            attackCommand.SetWidth(0);
            mouseDownTargetable = null;
            mouseUpTargetable = null;
            validTargets = null;
            SetPassiveCursor();
        }
        else if (attackCommand.lineRenderer.startWidth > 0 && Input.GetMouseButton(0))
        {
            RaycastHit hit;
            bool cast = Physics.Raycast(ray, out hit, 100f);
            if (cast)
            {
                attackCommand.SetPointPositions(mouseDownTargetable.transform.position, hit.point);
                if (hit.collider.gameObject.layer == battleLayer && mouseDownTargetable != null &&
                    hit.collider.gameObject != mouseDownTargetable.gameObject && validTargets.Contains(hit.collider.GetComponent<Targetable>()))
                    ActionManager.Instance.SetCursor(5);
                else
                    ActionManager.Instance.SetCursor(4);

                //for debugging
                if (Input.GetKeyUp("space"))
                    Debug.Log(hit.collider.name);
            }
        }
        else
        {
            return;
        }
        //do NOT put anything here, MouseButtonDown/else uses a return!
    }

    public void SetPassiveCursor()
    {
        if (this.activePlayer.Id == this.you.Id)
        {
            ActionManager.Instance.SetCursor(0);
        }
        else if (this.activePlayer.Mode != Player.PLAYER_STATE_MODE_NORMAL)
        {
            ActionManager.Instance.SetCursor(1);
        }
        else
        {
            ActionManager.Instance.SetCursor(2);
        }
    }

    private void OnEndTurnClick()
    {
        if (this.activePlayer.Mode != Player.PLAYER_STATE_MODE_NORMAL)   // dont allow end turn button click in non-normal state
        {
            return;
        }

        if (DeveloperPanel.IsServerEnabled())
        {
            if (this.activePlayer.Id == this.you.Id)
            {
                ChallengeMove challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(activePlayer.Id);
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_END_TURN);
                challengeMove.SetRank(GetDeviceMoveRank());
                AddDeviceMove(challengeMove);

                BattleSingleton.Instance.SendChallengeEndTurnRequest();
                NextTurn();
            }
        }
        else
        {
            NextTurn();
        }
    }

    private void NextTurn()
    {
        activePlayer.EndTurn();
        endTurnButton.ToggleState();

        EffectManager.Instance.OnEndTurn(
            activePlayer.Id,
            new UnityAction(ActualNextTurn)
        );
    }

    private void ActualNextTurn()
    {
        turnIndex++;
        activePlayer = players[turnIndex % players.Count];

        //do some turn transition render
        SetBoardCenterText(string.Format("{0} Turn", activePlayer.Name));
        SetPassiveCursor();
        //to-do: action manager set active = false, but that makes singleplayer broken

        activePlayer.NewTurn();
        EffectManager.Instance.OnStartTurn(activePlayer.Id);
    }

    public void SetBoardCenterText(string message)
    {
        GameObject.Find("Board Message").GetComponent<TextMeshPro>().text = message;
    }

    private void Surrender()
    {
        Debug.LogWarning("Surrender action pressed.");
        BattleSingleton.Instance.SendChallengeSurrenderRequest();
    }

    private List<Targetable> GetValidTargets(Targetable attacker)
    {
        List<Targetable> allTargets = new List<Targetable>();
        foreach (Player player in players)
        {
            if (player == attacker.Owner)
            {
                continue;
            }

            List<BoardCreature> fieldCreatures = Board.Instance.GetAliveCreaturesByPlayerId(player.Id);
            allTargets.AddRange(fieldCreatures);
            allTargets.Add(player.Avatar);
        }

        //Debug.LogWarning(allTargets.Count + " enemies found.");
        List<Targetable> priorityTargets = new List<Targetable>();
        for (int j = 0; j < allTargets.Count; j++)
        {
            if (allTargets[j].IsAvatar)
            {
                continue;
            }

            BoardCreature creature = allTargets[j] as BoardCreature;
            if (creature.HasAbility(Card.CARD_ABILITY_TAUNT))
            {
                priorityTargets.Add(creature);
            }
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

    private bool CheckFight(Targetable attacker, RaycastHit hit)
    {
        if (!attackCommand.enabled)
        {
            return false;
        }
        if (hit.collider.gameObject.layer != battleLayer)
        {
            return false;
        }
        if (mouseDownTargetable == null)
        {
            return false;
        }

        mouseUpTargetable = hit.collider.GetComponent<Targetable>();
        if (mouseUpTargetable == mouseDownTargetable)
        {
            //show creature details
            Debug.Log("Show details.");
        }
        else if (validTargets != null && validTargets.Count > 0 && validTargets.Contains(mouseUpTargetable))
        {
            Targetable attackingTargetable = mouseDownTargetable;
            Targetable defendingTargetable = mouseUpTargetable;

            ChallengeMove challengeMove;

            if (attackingTargetable.Owner.Id == this.you.Id)
            {
                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(attackingTargetable.GetPlayerId());
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CARD_ATTACK);
                challengeMove.SetRank(GetDeviceMoveRank());
                AddDeviceMove(challengeMove);
            }

            if (DeveloperPanel.IsServerEnabled())
            {
                CardAttackAttributes attributes = new CardAttackAttributes(
                    defendingTargetable.GetPlayerId(),
                    defendingTargetable.GetCardId()
                );
                BattleSingleton.Instance.SendChallengeCardAttackRequest(
                    attackingTargetable.GetCardId(),
                    attributes
                );
            }
            else
            {
                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(attackingTargetable.GetPlayerId());
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CARD_ATTACK);
                challengeMove.SetRank(GetServerMoveRank());

                ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
                moveAttributes.SetCardId(attackingTargetable.GetCardId());
                moveAttributes.SetFieldId(defendingTargetable.Owner.Id);
                moveAttributes.SetTargetId(defendingTargetable.GetCardId());
                challengeMove.SetMoveAttributes(moveAttributes);

                ReceiveChallengeMove(challengeMove);
            }

            if (attackingTargetable.Owner.Id == this.you.Id)
            {
                EffectManager.Instance.OnCreatureAttack(
                    attackingTargetable,
                    defendingTargetable
                );
            }

            return true;
        }

        return false;
    }

    private float GetCardDisplacement(BattleCardObject target)
    {
        Vector3 initial = target.reset.position;
        Vector3 release = target.transform.localPosition;
        Vector3 delta = initial - release;
        return delta.sqrMagnitude;
    }

    private bool CheckPlayCard(Ray ray, RaycastHit hit)
    {
        if (!ActionManager.Instance.HasDragTarget())
        {
            return false;
        }

        BattleCardObject target = ActionManager.Instance.GetDragTarget() as BattleCardObject;
        float minThreshold = 20;

        //Debug.Log(String.Format("amt={0}, threshold={1}, mousepos={2}, resetpos={3}", GetCardDisplacement(target), Screen.height * minThreshold, target.transform.localPosition, target.reset.position));
        if (target.GetCost() > target.Owner.Mana)
        {
            //can't play card due to mana
            ActionManager.Instance.ResetTarget();
            return false;
        }
        else if (GetCardDisplacement(target) < minThreshold)   //to-do: review this
        {
            //didn't displace card enough to activate
            ActionManager.Instance.ResetTarget();
            return false;
        }
        else if (target.Card.GetType() == typeof(SpellCard))
        {
            SpellCard spell = target.Card as SpellCard;
            if (spell.Targeted)
            {
                if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Battle")))
                {
                    // We'll enter this condition if card is actually played,
                    // otherwise the "playing" will be handled elsewhere.
                    if (PlayTargetedSpell(target, hit))
                    {
                        target.Owner.PlayCard(target);
                    }
                    return true;
                }
                else
                {
                    Debug.Log("Returning");
                    ActionManager.Instance.ResetTarget();
                    return false;
                }
            }
            else  //not targeted spell, play freely
            {
                if (PlayUntargetedSpell(target))
                {
                    target.Owner.PlayCard(target);
                }
                return true;
            }
        }
        else if (target.Card.GetType() == typeof(WeaponCard))
        {
            target.Owner.Avatar.EquipWeapon(target.Card as WeaponCard);
            target.Owner.PlayCard(target);
            this.UseCard(target.Owner, target);    //to-do: change to own weapon func
            return true;
        }
        else if (Physics.Raycast(ray, out hit, 100f, boardOrBattleLayer) &&
                 hit.collider.gameObject.layer == LayerMask.NameToLayer("Board") &&
                 hit.collider.name.Contains(target.Owner.Name))
        {
            //place card
            if (CanPlayCardToBoard(target, hit))
            {
                // We'll enter this condition if card is actually played,
                // otherwise the "playing" will be handled elsewhere.
                if (PlayCardToBoard(target, hit))
                {
                    target.Owner.PlayCard(target);
                }
                return true;
            }
            else
            {
                ActionManager.Instance.ResetTarget();
                return false;
            }
        }
        else
        {
            //no good activation events, return to hand or original pos/rot in collection
            ActionManager.Instance.ResetTarget();
            return false;
        }
    }

    public LTDescr AnimateDrawCard(Player player, BattleCardObject battleCardObject)
    {
        GameObject deckObject = GameObject.Find(String.Format("{0} Deck", player.Name));
        battleCardObject.transform.position = deckObject.transform.position;
        battleCardObject.transform.rotation = deckObject.transform.rotation;
        battleCardObject.visual.Redraw();

        Transform pivotPoint;
        if (player.Id == this.you.Id)
        {
            pivotPoint = this.playerDrawCardFixedTransform;
        }
        else
        {
            pivotPoint = this.enemyDrawCardFixedTransform;
        }

        LeanTween.rotate(battleCardObject.gameObject, pivotPoint.rotation.eulerAngles, CardTween.TWEEN_DURATION).setEaseInQuad();
        return CardTween.move(battleCardObject, pivotPoint.position, CardTween.TWEEN_DURATION)
                 .setEaseInQuad()
                 .setOnComplete(() =>
        {
            CardTween.move(battleCardObject, pivotPoint.position, CardTween.TWEEN_DURATION)
                 .setOnComplete(() =>
            {
                EffectManager.Instance.OnDrawCardFinish();
                player.Hand.RepositionCards();  //can override completioon behavior by calling setOnComplete again
            });
        });
    }

    public void AnimateDrawCardForMulligan(Player player, BattleCardObject battleCardObject, int position)
    {
        string targetPointName = String.Format("{0} Mulligan Holder {1}", player.Name, position);
        GameObject targetPoint = GameObject.Find(targetPointName);  //to-do cache this?
        battleCardObject.gameObject.SetLayer(LayerMask.NameToLayer("UI"));
        battleCardObject.transform.position = targetPoint.transform.position;
        battleCardObject.transform.localScale = Vector3.zero;

        battleCardObject.visual.SetOutline(true);
        battleCardObject.visual.Redraw();

        LeanTween.scale(battleCardObject.gameObject, battleCardObject.reset.scale, CardTween.TWEEN_DURATION);
        LeanTween.rotate(battleCardObject.gameObject, Camera.main.transform.rotation.eulerAngles, CardTween.TWEEN_DURATION).setEaseInQuad();
        CardTween.move(battleCardObject, targetPoint.transform.position + Vector3.up * 2.3F + Vector3.back * 0.2F, CardTween.TWEEN_DURATION)
                 .setEaseInQuad();
    }

    public void HideMulliganOverlay(Player player)
    {
        GameObject overlay = GameObject.Find(String.Format("{0} Mulligan Overlay", player.Name));

        LeanTween.move(overlay, overlay.transform.position + overlay.transform.up * -3, CardTween.TWEEN_DURATION);
        LeanTween
            .scale(overlay, Vector3.zero, CardTween.TWEEN_DURATION)
            .setOnComplete(() =>
                {
                    overlay.SetActive(false);

                    SetBoardCenterText(string.Format("{0} Turn", this.activePlayer.Name));
                    SetPassiveCursor();

                    if (this.mode != BATTLE_STATE_NORMAL_MODE)
                    {
                        this.activePlayer.MulliganNewTurn();
                        EffectManager.Instance.OnStartTurn(this.activePlayer.Id);
                        this.mode = BATTLE_STATE_NORMAL_MODE;
                    }
                });
    }

    public void ToggleMulliganCard(BattleCardObject battleCardObject)
    {
        if (battleCardObject.Owner.KeptMulliganCards.Contains(battleCardObject.Card))
        {
            battleCardObject.Owner.KeptMulliganCards.Remove(battleCardObject.Card);
            battleCardObject.Owner.RemovedMulliganCards.Add(battleCardObject.Card);

            //to-do: apply symbol/icon
            battleCardObject.visual.SetGrayscale(true);
            battleCardObject.visual.SetOutline(false);
        }
        else
        {
            battleCardObject.Owner.RemovedMulliganCards.Remove(battleCardObject.Card);
            battleCardObject.Owner.KeptMulliganCards.Add(battleCardObject.Card);
            //visuals
            battleCardObject.visual.SetGrayscale(false);
            battleCardObject.visual.SetOutline(true);
        }
    }

    public void FinishedMulligan()
    {
        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(this.you.Id);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN);
        challengeMove.SetRank(GetDeviceMoveRank());
        AddDeviceMove(challengeMove);

        this.you.PlayMulligan(this.opponent.Mode);
    }

    public bool CanPlayCardToBoard(BattleCardObject battleCardObject, RaycastHit hit)
    {
        Transform targetPosition = hit.collider.transform;
        string lastChar = hit.collider.name.Substring(hit.collider.name.Length - 1);
        int index = Int32.Parse(lastChar);

        Player player = battleCardObject.Owner;

        return Board.Instance.IsBoardPlaceOpen(player.Id, index);
    }

    /*
     * Play card to board after user on-device drags card from hand to field. 
     */
    public bool PlayCardToBoard(BattleCardObject battleCardObject, RaycastHit hit)
    {
        //only called for creature or structure
        Transform targetPosition = hit.collider.transform;
        string lastChar = hit.collider.name.Substring(hit.collider.name.Length - 1);
        int index = Int32.Parse(lastChar);
        Player player = battleCardObject.Owner;

        ChallengeMove challengeMove;

        if (player.Id == this.opponent.Id)
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_MINION);
            challengeMove.SetRank(GetServerMoveRank());

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCardId(battleCardObject.Card.Id);
            moveAttributes.SetCard(battleCardObject.Card.GetChallengeCard());
            moveAttributes.SetFieldIndex(index);
            challengeMove.SetMoveAttributes(moveAttributes);

            ReceiveChallengeMove(challengeMove);

            return false;
        }
        else
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_MINION);
            challengeMove.SetRank(GetDeviceMoveRank());
            AddDeviceMove(challengeMove);

            if (DeveloperPanel.IsServerEnabled())
            {
                PlayCardAttributes attributes = new PlayCardAttributes(index);
                BattleSingleton.Instance.SendChallengePlayCardRequest(
                    battleCardObject.Card.Id,
                    attributes
                );
            }
            else
            {
                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(player.Id);
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_MINION);

                ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
                moveAttributes.SetCardId(battleCardObject.Card.Id);
                moveAttributes.SetCard(battleCardObject.Card.GetChallengeCard());
                moveAttributes.SetFieldIndex(index);
                challengeMove.SetMoveAttributes(moveAttributes);

                challengeMove.SetRank(GetServerMoveRank());
                ReceiveChallengeMove(challengeMove);
            }

            SpawnCardToBoard(battleCardObject, index);

            return true;
        }
    }

    /*
     * Play card to board after receiving play card move from server. 
     */
    public void PlayCardToBoard(BattleCardObject battleCardObject, int index)
    {
        SpawnCardToBoard(battleCardObject, index);
    }

    private void SpawnCardToBoard(BattleCardObject battleCardObject, int fieldIndex)
    {
        int spawnRank = GetNewSpawnRank();

        battleCardObject.visual.Renderer.enabled = false;
        SoundManager.Instance.PlaySound("PlayCardSFX", transform.position);

        Board.Instance.CreateAndPlaceCreature(battleCardObject, fieldIndex, spawnRank);
    }

    /*
     * Play spell card after user on-device drags card from hand to field. 
     */
    public bool PlayTargetedSpell(BattleCardObject battleCardObject, RaycastHit hit)
    {
        BoardCreature targetedCreature = hit.collider.GetComponent<BoardCreature>();

        Player player = battleCardObject.Owner;
        ChallengeMove challengeMove;

        if (player.Id == this.opponent.Id)
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED);
            challengeMove.SetRank(GetServerMoveRank());

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCardId(battleCardObject.Card.Id);
            moveAttributes.SetCard(battleCardObject.Card.GetChallengeCard());
            moveAttributes.SetFieldId(targetedCreature.Owner.Id);
            moveAttributes.SetTargetId(targetedCreature.GetCardId());
            challengeMove.SetMoveAttributes(moveAttributes);

            ReceiveChallengeMove(challengeMove);

            return false;
        }
        else
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED);
            challengeMove.SetRank(GetDeviceMoveRank());
            AddDeviceMove(challengeMove);

            if (DeveloperPanel.IsServerEnabled())
            {
                PlaySpellTargetedAttributes attributes = new PlaySpellTargetedAttributes(
                    targetedCreature.Owner.Id,
                    targetedCreature.GetCardId()
                );
                BattleSingleton.Instance.SendChallengePlaySpellTargetedRequest(
                    battleCardObject.Card.Id,
                    attributes
                );
            }
            else
            {
                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(player.Id);
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED);
                challengeMove.SetRank(GetServerMoveRank());
                ReceiveChallengeMove(challengeMove);
            }

            EffectManager.Instance.OnSpellTargetedPlay(battleCardObject, targetedCreature);
            UseCard(battleCardObject.Owner, battleCardObject);

            return true;
        }
    }

    /*
     * Play targeted spell card after receiving play card move from server. 
     */
    public void PlayTargetedSpellFromServer(BattleCardObject battleCardObject, BoardCreature targetedCreature)
    {
        EffectManager.Instance.OnSpellTargetedPlay(battleCardObject, targetedCreature);
        UseCard(battleCardObject.Owner, battleCardObject);
    }

    /*
     * Play spell card after user on-device drags card from hand to field. 
     */
    public bool PlayUntargetedSpell(BattleCardObject battleCardObject)
    {
        Player player = battleCardObject.Owner;
        ChallengeMove challengeMove;

        if (player.Id == this.opponent.Id)
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED);
            challengeMove.SetRank(GetServerMoveRank());

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCardId(battleCardObject.Card.Id);
            moveAttributes.SetCard(battleCardObject.Card.GetChallengeCard());
            challengeMove.SetMoveAttributes(moveAttributes);

            ReceiveChallengeMove(challengeMove);

            return false;
        }
        else
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED);
            challengeMove.SetRank(GetDeviceMoveRank());
            AddDeviceMove(challengeMove);

            if (DeveloperPanel.IsServerEnabled())
            {
                BattleSingleton.Instance.SendChallengePlaySpellUntargetedRequest(
                    battleCardObject.Card.Id
                );
            }
            else
            {
                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(player.Id);
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED);
                challengeMove.SetRank(GetServerMoveRank());
                ReceiveChallengeMove(challengeMove);
            }

            EffectManager.Instance.OnSpellUntargetedPlay(battleCardObject);
            UseCard(battleCardObject.Owner, battleCardObject);

            return true;
        }
    }

    /*
     * Play untargeted spell card after receiving play card move from server. 
     */
    public void PlayUntargetedSpellFromServer(BattleCardObject battleCardObject)
    {
        EffectManager.Instance.OnSpellUntargetedPlay(battleCardObject);
        UseCard(battleCardObject.Owner, battleCardObject);
    }

    public void UseCard(Player player, BattleCardObject battleCardObject)
    {
        battleCardObject.Recycle();
        SoundManager.Instance.PlaySound("PlayCardSFX", transform.position);
    }

    /*
     * @param List<int> deckCardIndices - indices of cards opponent chose to put back in deck
     */
    private void ReceiveMovePlayMulligan(string playerId, List<int> deckCardIndices)
    {
        if (playerId != this.opponent.Id)
        {
            Debug.LogError("Play mulligan move received from server that is not from opponent.");
            return;
        }

        this.opponent.PlayMulliganByIndices(deckCardIndices);
    }

    private void ReceiveMoveFinishMulligan()
    {
        this.you.FinishMulligan();
        this.opponent.FinishMulligan();
        ComparePlayerStates(); // Compare state at the end of mulligan.
    }

    private void ReceiveMoveEndTurn(string playerId)
    {
        if (this.activePlayer.Id != playerId)
        {
            Debug.LogError("Device active player does not match challenge move player.");
            return;
        }

        NextTurn();
    }

    private void ReceiveMoveDrawCard(string playerId, Card card)
    {
        Player player = GetPlayerById(playerId);
        player.AddDrawnCard(card);
        //ComparePlayerStates(); // We cannot call this directly since EffectManager may still be processing after end turn.
    }

    private void ReceiveMoveDrawCardMulligan(string playerId, Card card)
    {
        Player player = GetPlayerById(playerId);
        player.AddDrawnCardMulligan(card);
        //ComparePlayerStates(); // We cannot call this directly since EffectManager may still be processing after end turn.
    }

    private void ReceiveMoveDrawCardHandFull(string playerId, Card card)
    {
        Player player = GetPlayerById(playerId);
        player.AddDrawnCardHandFull(card);
    }

    private void ReceiveMoveDrawCardDeckEmpty(string playerId)
    {
        Player player = GetPlayerById(playerId);
        // TODO: animate and remove debug.
        Debug.Log("Receive move draw card deck empty");
    }

    private void ReceiveMovePlayMinion(string playerId, string cardId, CreatureCard card, int handIndex, int fieldIndex)
    {
        if (this.activePlayer.Id != playerId)
        {
            Debug.LogError("Device active player does not match challenge move player.");
            return;
        }

        if (DeveloperPanel.IsServerEnabled())
        {
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByIndex(handIndex);
            battleCardObject.Reinitialize(card);
            opponent.PlayCard(battleCardObject);
            this.EnemyPlayCardToBoardAnim(battleCardObject, fieldIndex);
        }
        else
        {
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByCardId(cardId);
            if (battleCardObject == null)
            {
                Debug.LogError(String.Format("Server demanded card to play, but none of id {0} was found.", cardId));
                return;
            }
            opponent.PlayCard(battleCardObject);
            this.EnemyPlayCardToBoardAnim(battleCardObject, fieldIndex);
        }
    }

    private void ReceiveMovePlaySpellTargeted(
        string playerId,
        string cardId,
        SpellCard card,
        int handIndex,
        string fieldId,
        string targetId
    )
    {
        if (this.activePlayer.Id != playerId)
        {
            Debug.LogError("Device active player does not match challenge move player.");
            return;
        }

        BoardCreature targetedCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(fieldId, targetId);

        if (DeveloperPanel.IsServerEnabled())
        {
            int opponentHandIndex = opponent.GetOpponentHandIndex(handIndex);
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByIndex(opponentHandIndex);
            battleCardObject.Reinitialize(card);

            opponent.PlayCard(battleCardObject);
            EnemyPlaySpellTargetedAnim(battleCardObject, targetedCreature);
        }
        else
        {
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByCardId(cardId);
            if (battleCardObject == null)
            {
                Debug.LogError(String.Format("Demanded card to play, but none of id {0} was found.", cardId));
                return;
            }

            opponent.PlayCard(battleCardObject);
            EnemyPlaySpellTargetedAnim(battleCardObject, targetedCreature);
        }
    }

    private void ReceiveMovePlaySpellUntargeted(
        string playerId,
        string cardId,
        SpellCard card,
        int handIndex
    )
    {
        if (this.activePlayer.Id != playerId)
        {
            Debug.LogError("Device active player does not match challenge move player.");
            return;
        }

        if (DeveloperPanel.IsServerEnabled())
        {
            int opponentHandIndex = opponent.GetOpponentHandIndex(handIndex);
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByIndex(opponentHandIndex);
            battleCardObject.Reinitialize(card);

            opponent.PlayCard(battleCardObject);
            EnemyPlaySpellUntargetedAnim(battleCardObject);
        }
        else
        {
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByCardId(cardId);
            if (battleCardObject == null)
            {
                Debug.LogError(String.Format("Demanded card to play, but none of id {0} was found.", cardId));
                return;
            }

            opponent.PlayCard(battleCardObject);
            EnemyPlaySpellUntargetedAnim(battleCardObject);
        }
    }

    private void ReceiveMoveCardAttack(
        string playerId,
        string cardId,
        string fieldId,
        string targetId
    )
    {
        if (this.activePlayer.Id != playerId)
        {
            Debug.LogError("Device active player does not match challenge move player.");
            return;
        }

        Targetable attackingTargetable = Board.Instance.GetTargetableByPlayerIdAndCardId(playerId, cardId);
        Targetable defendingTargetable = Board.Instance.GetTargetableByPlayerIdAndCardId(fieldId, targetId);

        EffectManager.Instance.OnCreatureAttack(
            attackingTargetable,
            defendingTargetable
        );
    }

    private void ReceiveMoveRandomTarget(
        string playerId,
        ChallengeCard card,
        string fieldId,
        string targetId
    )
    {
        EffectManager.Instance.OnRandomTarget(
            playerId,
            card,
            fieldId,
            targetId
        );
    }

    private void ReceiveMoveSummonCreature(
        string playerId,
        ChallengeCard challengeCard,
        string fieldId,
        int fieldIndex
    )
    {
        Card card = challengeCard.GetCard();
        if (card.GetType() == typeof(CreatureCard))
        {
            Player player = GetPlayerById(playerId);
            GameObject created = new GameObject(card.Name);
            BattleCardObject battleCardObject = created.AddComponent<BattleCardObject>();
            battleCardObject.Initialize(player, card);

            Board.Instance.CreateAndPlaceCreature(
                battleCardObject,
                fieldIndex,
                challengeCard.SpawnRank,
                false
            );

            GetNewSpawnRank(); // Increment spawn count since summon is a spawn.
            EffectManager.Instance.OnSummonCreatureFinish();
        }
        else
        {
            Debug.LogError("Invalid card category for summon creature move");
        }
    }

    private void ReceiveMoveSummonCreatureFieldFull(
        string playerId,
        CreatureCard card,
        string fieldId
    )
    {
        // TODO
        GetNewSpawnRank(); // Increment spawn count since summon is a spawn.
        EffectManager.Instance.OnSummonCreatureFinish();
    }

    private void ReceiveMoveSummonCreatureNoCreature(string playerId)
    {
        // TODO
        EffectManager.Instance.OnSummonCreatureFinish();
    }


    //{
    //    "id": "C14",
    //  "level": 0,
    //  "levelPrevious": 0,
    //  "exp": 2,
    //  "expMax": 10,
    //  "expPrevious": 1,
    //  "category": 0,
    //  "attack": 60,
    //  "health": 60,
    //  "cost": 70,
    //  "name": "Fireborn Menace",
    //  "description": "Battlecry: Deal 20 damage to any minion in front",
    //  "abilities": [
    //    16
    //  ]
    //}

    public void ReceiveChallengeWon(ChallengeEndState challengeEndState)
    {
        Debug.Log("Challenge won!");
        List<ExperienceCard> experienceCards = challengeEndState.ExperienceCards;
        Debug.Log(JsonUtility.ToJson(experienceCards));

        ShowBattleEndFX(experienceCards, true);
    }

    public void ReceiveChallengeLost(ChallengeEndState challengeEndState)
    {
        Debug.Log("Challenge lost...");
        List<ExperienceCard> experienceCards = challengeEndState.ExperienceCards;
        Debug.Log(JsonUtility.ToJson(experienceCards));

        ShowBattleEndFX(experienceCards, false);
    }

    private void RenderEXPChanges(List<ExperienceCard> experienceCards)
    {
        int index = 0;
        int rowSize = 4;
        foreach (ExperienceCard item in experienceCards)
        {
            Card card = item.GetCard();
            GameObject created = new GameObject(card.Name);
            CardObject cardObject = created.AddComponent<CardObject>();
            cardObject.Initialize(card);

            Vector3 offset = (index % rowSize) * Vector3.right * 1.8f + (index / rowSize) * Vector3.down * 3.6f;
            CardTween.move(cardObject, endOverlay.transform.position + offset, CardTween.TWEEN_DURATION);
            ++index;
        }
    }

    public void ShowBattleEndFX(List<ExperienceCard> experienceCards, bool won)
    {
        endOverlay.SetActive(true);
        TextMeshPro title = endOverlay.transform.Find("Title").GetComponent<TextMeshPro>();

        if (won)
        {
            title.text = "Victory";
            endOverlay.transform.Find("WinFX").gameObject.SetActive(true);
        }
        else
        {
            title.text = "Defeat";
            endOverlay.transform.Find("LoseFX").gameObject.SetActive(true);
        }

        LeanTween.scale(title.gameObject, title.transform.localScale / 1.25f, CardTween.TWEEN_DURATION)
             .setDelay(CardTween.TWEEN_DURATION * 3)
             .setOnComplete(() =>
             {
                 LeanTween.moveY(title.gameObject, title.transform.position.y + 3f, CardTween.TWEEN_DURATION)
                    .setOnComplete(() =>
                    {
                        RenderEXPChanges(experienceCards);
                    });
             });
    }

    public void ReceiveChallengeMove(ChallengeMove challengeMove)
    {
        if (DeveloperPanel.IsLogVerbose())
        {
            Debug.Log("Server move queue: " + challengeMove.Rank);
            Debug.Log(JsonUtility.ToJson(challengeMove));
        }
        this.serverMoveQueue.Add(challengeMove);
    }

    public void AddDeviceMove(ChallengeMove challengeMove)
    {
        if (DeveloperPanel.IsLogVerbose())
        {
            Debug.Log("Device move queue: " + challengeMove.Rank);
            Debug.Log(JsonUtility.ToJson(challengeMove));
        }
        this.deviceMoveQueue.Add(challengeMove);
    }

    // Challenge moves that cannot be predicted by device.
    private static List<string> OPPONENT_SERVER_CHALLENGE_MOVES = new List<string>
    {
        ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN,
        ChallengeMove.MOVE_CATEGORY_FINISH_MULLIGAN,
        ChallengeMove.MOVE_CATEGORY_DRAW_CARD_MULLIGAN,
        ChallengeMove.MOVE_CATEGORY_END_TURN,
        ChallengeMove.MOVE_CATEGORY_PLAY_MINION,
        ChallengeMove.MOVE_CATEGORY_CARD_ATTACK,
        ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED,
        ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED,
    };

    // Challenge moves to skip since device has already performed them.
    private static List<string> PLAYER_SKIP_CHALLENGE_MOVES = new List<string>
    {
        ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN, // What about if ran out of time?
        ChallengeMove.MOVE_CATEGORY_END_TURN,
        ChallengeMove.MOVE_CATEGORY_PLAY_MINION,
        ChallengeMove.MOVE_CATEGORY_CARD_ATTACK,
        ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED,
        ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED,
    };

    /*
     * @return int - rank of move processed, -1 if no move processed
     */
    public int ProcessMoveQueue()
    {
        if (this.serverMoveQueue.Count <= 0)
        {
            return -1;
        }

        ChallengeMove serverMove = this.serverMoveQueue[0];

        if (
            (serverMove.PlayerId == this.opponent.Id || serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD_MULLIGAN || serverMove.Category == ChallengeMove.MOVE_CATEGORY_FINISH_MULLIGAN) &&
            OPPONENT_SERVER_CHALLENGE_MOVES.Contains(serverMove.Category)
        )
        {
            this.deviceMoveCount += 1;
        }
        else if (this.deviceMoveQueue.Count <= 0)
        {
            return -1;
        }
        else
        {
            ChallengeMove deviceMove = this.deviceMoveQueue[0];
            if (
                deviceMove.PlayerId != serverMove.PlayerId ||
                deviceMove.Category != serverMove.Category ||
                deviceMove.Rank != serverMove.Rank
            )
            {
                Debug.LogError("Device move does not match server move.");
                Debug.LogWarning(string.Format("PlayerId: {0} vs {1}.", deviceMove.PlayerId, serverMove.PlayerId));
                Debug.LogWarning(string.Format("Category: {0} vs {1}.", deviceMove.Category, serverMove.Category));
                Debug.LogWarning(string.Format("Rank: {0} vs {1}.", deviceMove.Rank, serverMove.Rank));

                this.deviceMoveQueue.RemoveAt(0);
                this.serverMoveQueue.RemoveAt(0);

                return -1;
            }

            this.deviceMoveQueue.RemoveAt(0);
        }

        this.serverMoves.Add(serverMove);
        this.serverMoveQueue.RemoveAt(0);

        if (
            serverMove.PlayerId == this.you.Id &&
            PLAYER_SKIP_CHALLENGE_MOVES.Contains(serverMove.Category)
        )
        {
            return -1;
        }

        if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN)
        {
            ReceiveMovePlayMulligan(
                serverMove.PlayerId,
                serverMove.Attributes.DeckCardIndices
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_FINISH_MULLIGAN)
        {
            ReceiveMoveFinishMulligan();
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_END_TURN)
        {
            ReceiveMoveEndTurn(
                serverMove.PlayerId
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            ReceiveMoveDrawCard(
                serverMove.PlayerId,
                card
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD_MULLIGAN)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            ReceiveMoveDrawCardMulligan(
                serverMove.PlayerId,
                card
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD_HAND_FULL)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            ReceiveMoveDrawCardHandFull(
                serverMove.PlayerId,
                card
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY)
        {
            ReceiveMoveDrawCardDeckEmpty(
                serverMove.PlayerId
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY)
        {
            Debug.LogError("Not supported.");
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_MINION)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            if (card.GetType() == typeof(CreatureCard))
            {
                ReceiveMovePlayMinion(
                    serverMove.PlayerId,
                    serverMove.Attributes.CardId,
                    card as CreatureCard,
                    serverMove.Attributes.HandIndex,
                    serverMove.Attributes.FieldIndex
                );
            }
            else
            {
                Debug.LogError("Invalid card category for play minion move");
            }
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            if (card.GetType() == typeof(SpellCard))
            {
                ReceiveMovePlaySpellUntargeted(
                    serverMove.PlayerId,
                    serverMove.Attributes.CardId,
                    card as SpellCard,
                    serverMove.Attributes.HandIndex
                );
            }
            else
            {
                Debug.LogError("Invalid card category for play spell general move");
            }
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            if (card.GetType() == typeof(SpellCard))
            {
                ReceiveMovePlaySpellTargeted(
                    serverMove.PlayerId,
                    serverMove.Attributes.CardId,
                    card as SpellCard,
                    serverMove.Attributes.HandIndex,
                    serverMove.Attributes.FieldId,
                    serverMove.Attributes.TargetId
                );
            }
            else
            {
                Debug.LogError("Invalid card category for play spell targeted move");
            }
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_CARD_ATTACK)
        {
            ReceiveMoveCardAttack(
                serverMove.PlayerId,
                serverMove.Attributes.CardId,
                serverMove.Attributes.FieldId,
                serverMove.Attributes.TargetId
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET)
        {
            ReceiveMoveRandomTarget(
                serverMove.PlayerId,
                serverMove.Attributes.Card,
                serverMove.Attributes.FieldId,
                serverMove.Attributes.TargetId
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE)
        {
            ReceiveMoveSummonCreature(
                serverMove.PlayerId,
                serverMove.Attributes.Card,
                serverMove.Attributes.FieldId,
                serverMove.Attributes.FieldIndex
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            if (card.GetType() == typeof(SpellCard))
            {
                ReceiveMoveSummonCreatureFieldFull(
                    serverMove.PlayerId,
                    card as CreatureCard,
                    serverMove.Attributes.FieldId
                );
            }
            else
            {
                Debug.LogError("Invalid card category for summon creature move");
            }
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_NO_CREATURE)
        {
            ReceiveMoveSummonCreatureNoCreature(serverMove.PlayerId);
        }

        return serverMove.Rank;
    }

    private LTDescr AnimateCardPlayed(BattleCardObject battleCardObject)
    {
        LeanTween.rotateLocal(battleCardObject.visual.gameObject, new Vector3(0, 180, 0), CardTween.TWEEN_DURATION)
                 .setEaseInQuad();
        LeanTween.rotate(battleCardObject.gameObject, this.enemyPlayCardFixedTransform.rotation.eulerAngles, CardTween.TWEEN_DURATION)
                 .setEaseInQuad();
        return CardTween.move(battleCardObject, this.enemyPlayCardFixedTransform.position, CardTween.TWEEN_DURATION)
                 .setEaseInQuad();
    }

    private void EnemyPlayCardToBoardAnim(BattleCardObject battleCardObject, int fieldIndex)
    {
        this.AnimateCardPlayed(battleCardObject).setOnComplete(() =>
          {
              CardTween.move(battleCardObject, this.enemyPlayCardFixedTransform.position, CardTween.TWEEN_DURATION)
                 .setOnComplete(() =>
                 {
                     PlayCardToBoard(battleCardObject, fieldIndex);
                 });
          });
    }

    private void EnemyPlaySpellTargetedAnim(BattleCardObject battleCardObject, BoardCreature targetedCreature)
    {
        this.AnimateCardPlayed(battleCardObject).setOnComplete(() =>
        {
            CardTween.move(battleCardObject, this.enemyPlayCardFixedTransform.position, CardTween.TWEEN_DURATION)
                 .setOnComplete(() =>
                 {
                     PlayTargetedSpellFromServer(battleCardObject, targetedCreature);
                 });
        });
    }

    private void EnemyPlaySpellUntargetedAnim(BattleCardObject battleCardObject)
    {
        this.AnimateCardPlayed(battleCardObject).setOnComplete(() =>
        {
            CardTween.move(battleCardObject, this.enemyPlayCardFixedTransform.position, CardTween.TWEEN_DURATION)
                 .setOnComplete(() =>
                 {
                     PlayUntargetedSpellFromServer(battleCardObject);
                 });
        });
    }

    private PlayerState GetPlayerState()
    {
        return this.you.GeneratePlayerState();
    }

    public PlayerState GetOpponentState()
    {
        return this.opponent.GeneratePlayerState();
    }

    public void ComparePlayerStates()
    {
        PlayerState devicePlayerState = BattleManager.Instance.GetPlayerState();
        PlayerState deviceOpponentState = BattleManager.Instance.GetOpponentState();

        if (DeveloperPanel.IsServerEnabled())
        {
            BattleSingleton.Instance.ComparePlayerStates(
                devicePlayerState,
                deviceOpponentState,
                this.deviceMoveCount
            );
        }
    }

    public int GetDeviceMoveRank()
    {
        int rank = this.deviceMoveCount;
        this.deviceMoveCount += 1;
        return rank;
    }

    public int GetServerMoveRank()
    {
        int rank = this.serverMoveCount;
        this.serverMoveCount += 1;
        return rank;
    }

    public bool CanReceiveChallengeMove()
    {
        return true;
    }

    public int GetNewSpawnRank()
    {
        int spawnRank = this.spawnCount;
        this.spawnCount += 1;
        return spawnRank;
    }

    public List<ChallengeMove> GetServerMoves()
    {
        return this.serverMoves;
    }

    public void SetServerMoves(List<ChallengeMove> serverMoves)
    {
        this.serverMoves = serverMoves;
    }
}
