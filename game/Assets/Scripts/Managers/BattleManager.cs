using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

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
    private Player opponent;

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
    public int stencilCount;

    public Dictionary<string, CardTemplate> cardTemplates;
    private int mode;

    private const int BATTLE_STATE_NORMAL_MODE = 0;
    private const int BATTLE_STATE_MULLIGAN_MODE = 1;

    private int spawnCount;
    public int SpawnCount => spawnCount;

    private int deviceMoveCount;
    private int serverMoveCount;

    private List<ChallengeMove> serverMoveQueue;
    private List<ChallengeMove> deviceMoveQueue;

    public static BattleManager Instance { get; private set; }

    public Player GetPlayerById(string playerId)
    {
        return this.playerIdToPlayer[playerId];
    }

    private void Awake()
    {
        Instance = this;

        string codexPath = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "codex.txt";
        this.cardTemplates = CodexHelper.ParseFile(codexPath);

        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            if (!BattleSingleton.Instance.ChallengeStarted)
            {
                //throw new Exception("Challenge not started - did you enter this scene from the matchmaking scene?");
                BattleSingleton.Instance.SendFindMatchRequest(Matchmaking.MATCH_TYPE_CASUAL, "Deck1");
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

        this.playerIdToPlayer = new Dictionary<string, Player>();
        this.players = new List<Player>();

        this.stencilCount = 1;
        battleLayer = LayerMask.NameToLayer("Battle");
        boardOrBattleLayer = LayerMask.GetMask(new string[2] { "Board", "Battle" });

        ChooseRandomSetting();

        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            if (this.initialized)
            {
                this.spawnCount = BattleSingleton.Instance.SpawnCount;

                this.you = new Player(BattleSingleton.Instance.PlayerState, "Player");
                this.opponent = new Player(BattleSingleton.Instance.OpponentState, "Enemy");

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

            this.playerIdToPlayer[this.you.Id] = this.you;
            this.playerIdToPlayer[this.opponent.Id] = this.opponent;

            this.players.Add(this.you);
            this.players.Add(this.opponent);

            GameStart();
        }
    }

    private void GameStart()
    {
        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            turnIndex = this.players.FindIndex(player => player.HasTurn);
            activePlayer = players[turnIndex % players.Count];

            if (this.you.IsModeMulligan())
            {
                this.you.ResumeMulligan(
                    BattleSingleton.Instance.GetMulliganCards(this.you.Id)
                );
                this.opponent.ResumeMulligan(
                    BattleSingleton.Instance.GetMulliganCards(this.opponent.Id)
                );
                this.mode = BATTLE_STATE_MULLIGAN_MODE;
            }
            else
            {
                HideMulliganOverlay(this.you);
                HideMulliganOverlay(this.opponent);
                activePlayer.RenderTurnStart();
            }
        }
        else
        {
            this.you.BeginMulligan(this.you.PopCardsFromDeck(3));
            this.opponent.BeginMulligan(this.opponent.PopCardsFromDeck(3));
            this.mode = BATTLE_STATE_MULLIGAN_MODE;

            turnIndex = UnityEngine.Random.Range(0, players.Count);
            activePlayer = players[turnIndex % players.Count];
            activePlayer.RenderTurnStart();
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

            if (!mouseDownTargetable.Owner.HasTurn || mouseDownTargetable.CanAttack <= 0)
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
        if (this.activePlayer == this.you)
        {
            ActionManager.Instance.SetCursor(0);
        }
        else
        {
            ActionManager.Instance.SetCursor(2);
        }
    }

    private void OnEndTurnClick()
    {
        if (this.you.Mode != Player.PLAYER_STATE_MODE_NORMAL)   //dont allow end turn button click in mulligan or non-normal state
        {
            return;
        }

        if (InspectorControlPanel.Instance.DevelopmentMode)
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
        EffectManager.Instance.OnEndTurn(
            activePlayer.Id,
            new UnityAction(ActualNextTurn)
        );
    }

    private void ActualNextTurn()
    {
        activePlayer.EndTurn();

        turnIndex++;
        activePlayer = players[turnIndex % players.Count];

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(activePlayer.Id);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD);
        challengeMove.SetRank(GetDeviceMoveRank());
        AddDeviceMove(challengeMove);

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
                continue;
            BoardCreature[] fieldCreatures = Board.Instance.GetFieldByPlayerId(player.Id).GetCreatures();
            for (int i = 0; i < fieldCreatures.Length; i++)
            {
                if (fieldCreatures[i] != null)
                    allTargets.Add(fieldCreatures[i]);
            }
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

            ChallengeMove challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(attackingTargetable.GetPlayerId());
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CARD_ATTACK);
            challengeMove.SetRank(GetDeviceMoveRank());
            AddDeviceMove(challengeMove);

            if (InspectorControlPanel.Instance.DevelopmentMode)
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
                moveAttributes.SetFieldId(defendingTargetable.Owner.Id);
                moveAttributes.SetTargetId(defendingTargetable.GetCardId());
                challengeMove.SetMoveAttributes(moveAttributes);

                ReceiveChallengeMove(challengeMove);
            }

            EffectManager.Instance.OnCreatureAttack(
                attackingTargetable,
                defendingTargetable
            );

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
        if (target.Card.Cost > target.Owner.Mana)
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
                    target.Owner.PlayCard(target);
                    this.PlayTargetedSpell(target, hit);
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
                this.UseCard(target.Owner, target); //change to own board spell func
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

    private IEnumerator EnemyPlayCardToBoardAnim(object[] args)
    {
        BattleCardObject battleCardObject = (BattleCardObject)args[0];
        int fieldIndex = (int)args[1];

        Transform pivotPoint = GameObject.Find("EnemyPlayCardFixed").transform;

        LeanTween.move(battleCardObject.gameObject, pivotPoint.position, 0.4f).setEaseInQuad();
        LeanTween.rotate(battleCardObject.gameObject, pivotPoint.rotation.eulerAngles, 0.4f).setEaseInQuad();
        yield return new WaitForSeconds(0.4f);

        //flash or something
        yield return new WaitForSeconds(1);

        PlayCardToBoard(battleCardObject, fieldIndex);
    }

    public void AnimateDrawCard(Player player, BattleCardObject battleCardObject)
    {
        StartCoroutine("DrawCardAnimation", new object[2] { player, battleCardObject });
    }

    private IEnumerator DrawCardAnimation(object[] args)
    {
        Player player = args[0] as Player;
        BattleCardObject battleCardObject = args[1] as BattleCardObject;

        GameObject deckObject = GameObject.Find(String.Format("{0} Deck", player.Name));
        battleCardObject.transform.position = deckObject.transform.position;
        battleCardObject.transform.rotation = deckObject.transform.rotation;
        //done initializing to match deck orientation

        string fixedPointName = String.Format("{0}DrawCardFixed", player.Name);
        GameObject fixedPoint = GameObject.Find(fixedPointName);

        LeanTween.rotate(battleCardObject.gameObject, fixedPoint.transform.rotation.eulerAngles, ActionManager.TWEEN_DURATION).setEaseInQuad();
        LeanTween.move(battleCardObject.gameObject, fixedPoint.transform.position, ActionManager.TWEEN_DURATION).setEaseInQuad();
        yield return new WaitForSeconds(ActionManager.TWEEN_DURATION);


        yield return new WaitForSeconds(ActionManager.TWEEN_DURATION);
        player.Hand.RepositionCards();
    }

    public void AnimateDrawCardForMulligan(Player player, BattleCardObject battleCardObject, int position)
    {
        string targetPointName = String.Format("{0} Mulligan Holder {1}", player.Name, position);
        GameObject targetPoint = GameObject.Find(targetPointName);
        battleCardObject.transform.position = targetPoint.transform.position;
        battleCardObject.transform.localScale = Vector3.zero;

        LeanTween.scale(battleCardObject.gameObject, battleCardObject.reset.scale, ActionManager.TWEEN_DURATION);
        LeanTween.rotate(battleCardObject.gameObject, Camera.main.transform.rotation.eulerAngles, ActionManager.TWEEN_DURATION).setEaseInQuad();
        LeanTween.move(battleCardObject.gameObject, targetPoint.transform.position + Vector3.up * 2.3F + Vector3.back * 0.2F, ActionManager.TWEEN_DURATION).setEaseInQuad();
        battleCardObject.visual.Redraw();
    }

    public void HideMulliganOverlay(Player player)
    {
        // If not in connected mode, automatically perform mulligan for opponent.
        if (!InspectorControlPanel.Instance.DevelopmentMode && player.Id == this.you.Id)
        {
            ChallengeMove challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(this.opponent.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN);
            challengeMove.SetRank(BattleManager.Instance.GetServerMoveRank());
            BattleManager.Instance.ReceiveChallengeMove(challengeMove);
        }

        StartCoroutine("AnimateHideMulliganOverlay", player);
    }

    private IEnumerator AnimateHideMulliganOverlay(Player player)
    {
        GameObject overlay = GameObject.Find(String.Format("{0} Mulligan Overlay", player.Name));
        LeanTween.scale(overlay, Vector3.zero, ActionManager.TWEEN_DURATION);
        yield return new WaitForSeconds(ActionManager.TWEEN_DURATION);
        overlay.SetActive(false);

        if (!player.IsModeMulligan())
        {
            //do some turn transition render
            SetBoardCenterText(string.Format("{0} Turn", this.activePlayer.Name));
            SetPassiveCursor();
            this.activePlayer.MulliganNewTurn();
            this.mode = BATTLE_STATE_NORMAL_MODE;
        }
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
        this.opponent.AdvanceMulliganState();
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

            if (InspectorControlPanel.Instance.DevelopmentMode)
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
        int spawnCount = this.spawnCount;
        this.spawnCount += 1;

        battleCardObject.visual.Renderer.enabled = false;
        SoundManager.Instance.PlaySound("PlayCardSFX", transform.position);

        Board.Instance.CreateAndPlaceCreature(battleCardObject, fieldIndex, spawnCount);
        //OnPlay(battleCardObject.Owner.Id, battleCardObject.Card.Id);
    }

    /*
     * Play spell card after user on-device drags card from hand to field. 
     */
    public void PlayTargetedSpell(BattleCardObject battleCardObject, RaycastHit hit)
    {
        BoardCreature targetedCreature = hit.collider.GetComponent<BoardCreature>();
        SpellCard spellCard = battleCardObject.Card as SpellCard;
        spellCard.Activate(targetedCreature);
        UseCard(battleCardObject.Owner, battleCardObject);

        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            PlaySpellTargetedAttributes attributes = new PlaySpellTargetedAttributes(
                targetedCreature.Owner.Id,
                targetedCreature.CreatureCard.Id
            );
            BattleSingleton.Instance.SendChallengePlaySpellTargetedRequest(
                battleCardObject.Card.Id,
                attributes
            );
        }
    }

    /*
     * Play spell card after receiving play card move from server. 
     */
    public void PlayTargetedSpell(BattleCardObject spellCardObject, BoardCreature victimCreature)
    {
        ((SpellCard)spellCardObject.Card).Activate(victimCreature);
        UseCard(spellCardObject.Owner, spellCardObject);
    }

    public void UseCard(Player player, BattleCardObject battleCardObject)
    {
        GameObject.Destroy(battleCardObject.gameObject);
        SoundManager.Instance.PlaySound("PlayCardSFX", transform.position);
    }

    public void ReceiveMovePlayMulligan(string playerId, List<int> deckCardIndices)
    {
        if (playerId != this.opponent.Id)
        {
            Debug.LogError("Play mulligan move received from server that is not from opponent.");
            return;
        }

        this.opponent.PlayMulliganByIndices(deckCardIndices, this.you.Mode);
        this.you.AdvanceMulliganState();
    }

    public void ReceiveMoveEndTurn(string playerId)
    {
        NextTurn();
    }

    public void ReceiveMoveDrawCard(string playerId, Card card)
    {
        Player player = GetPlayerById(playerId);
        player.AddDrawnCard(card);
    }

    public void ReceiveMovePlayMinion(string playerId, string cardId, CreatureCard card, int handIndex, int fieldIndex)
    {
        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            BattleCardObject target = opponent.Hand.GetCardObjectByIndex(handIndex);
            target.Initialize(opponent, card);
            opponent.PlayCard(target);
            StartCoroutine("EnemyPlayCardToBoardAnim", new object[2] { target, fieldIndex });
        }
        else
        {
            BattleCardObject target = opponent.Hand.GetCardObjectByCardId(cardId);
            if (target == null)
            {
                Debug.LogError(String.Format("Server demanded card to play, but none of id {0} was found.", cardId));
                return;
            }
            opponent.PlayCard(target);
            StartCoroutine("EnemyPlayCardToBoardAnim", new object[2] { target, fieldIndex });
        }
    }

    public void ReceiveMovePlaySpellTargeted(
        string playerId,
        string cardId,
        SpellCard card,
        int handIndex,
        string fieldId,
        string targetId
    )
    {
        BoardCreature victimCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(fieldId, targetId);

        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            int opponentHandIndex = opponent.GetOpponentHandIndex(handIndex);
            BattleCardObject target = opponent.Hand.GetCardObjectByIndex(opponentHandIndex);
            target.Initialize(opponent, card);
            opponent.PlayCard(target);
            PlayTargetedSpell(target, victimCreature);
        }
        else
        {
            BattleCardObject target = opponent.Hand.GetCardObjectByCardId(cardId);
            if (target == null)
            {
                Debug.LogError(String.Format("Demanded card to play, but none of id {0} was found.", cardId));
                return;
            }
            opponent.PlayCard(target);
            PlayTargetedSpell(target, victimCreature);
        }
    }

    public void ReceiveMovePlaySpellUntargeted(
        string playerId,
        string cardId,
        SpellCard card,
        int handIndex
    )
    {
        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            int opponentHandIndex = opponent.GetOpponentHandIndex(handIndex);
            BattleCardObject target = opponent.Hand.GetCardObjectByIndex(opponentHandIndex);
            target.Initialize(opponent, card);
            opponent.PlayCard(target);
        }
        else
        {
            BattleCardObject target = opponent.Hand.GetCardObjectByCardId(cardId);
            if (target == null)
            {
                Debug.LogError(String.Format("Demanded card to play, but none of id {0} was found.", cardId));
                return;
            }
            opponent.PlayCard(target);
        }
    }

    public void ReceiveMoveCardAttack(
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

        Targetable attacker = Board.Instance.GetTargetableByPlayerIdAndCardId(playerId, cardId);
        Targetable defender = Board.Instance.GetTargetableByPlayerIdAndCardId(fieldId, targetId);
        attacker.Fight(defender);
    }

    public void ReceiveChallengeEndState(ChallengeEndState challengeEndState)
    {

    }

    public void ReceiveChallengeMove(ChallengeMove challengeMove)
    {
        Debug.Log("Server move queue: " + challengeMove.Rank);
        Debug.Log(JsonUtility.ToJson(challengeMove));
        this.serverMoveQueue.Add(challengeMove);
    }

    public void AddDeviceMove(ChallengeMove challengeMove)
    {
        Debug.Log("Device move queue: " + challengeMove.Rank);
        Debug.Log(JsonUtility.ToJson(challengeMove));
        this.deviceMoveQueue.Add(challengeMove);
    }

    // Challenge moves that cannot be predicted by device.
    private static List<string> OPPONENT_SERVER_CHALLENGE_MOVES = new List<string>
    {
        ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN,
        ChallengeMove.MOVE_CATEGORY_END_TURN,
        ChallengeMove.MOVE_CATEGORY_PLAY_MINION,
        ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED,
        ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED,
    };

    // Challenge moves to skip since device has already performed them.
    private static List<string> PLAYER_SKIP_CHALLENGE_MOVES = new List<string>
    {
        ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN,
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
            serverMove.PlayerId == this.opponent.Id &&
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
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_END_TURN)
        {
            ReceiveMoveEndTurn(
                serverMove.PlayerId
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD)
        {
            Player owner = BattleManager.Instance.PlayerIdToPlayer[serverMove.PlayerId];
            Card card = serverMove.Attributes.Card.GetCard();

            ReceiveMoveDrawCard(
                serverMove.PlayerId,
                card
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_MINION)
        {
            Player owner = BattleManager.Instance.PlayerIdToPlayer[serverMove.PlayerId];
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
            Player owner = BattleManager.Instance.PlayerIdToPlayer[serverMove.PlayerId];
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
            Player owner = BattleManager.Instance.PlayerIdToPlayer[serverMove.PlayerId];
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

        if (this.deviceMoveQueue.Count <= 0 && this.deviceMoveQueue.Count <= 0)
        {
            PlayerState devicePlayerState = GetPlayerState();
            PlayerState deviceOpponentState = GetOpponentState();
            PlayerState serverPlayerState = BattleSingleton.Instance.PlayerState;
            PlayerState serverOpponentState = BattleSingleton.Instance.OpponentState;

            if (InspectorControlPanel.Instance.DevelopmentMode)
            {
                if (!serverPlayerState.Equals(devicePlayerState))
                {
                    Debug.LogWarning("Server vs device player state mismatch.");
                    Debug.LogWarning("Server: " + JsonUtility.ToJson(serverPlayerState));
                    Debug.LogWarning("Device: " + JsonUtility.ToJson(devicePlayerState));
                    Debug.LogWarning("First diff: " + serverPlayerState.FirstDiff(devicePlayerState));
                }
                else
                {
                    Debug.Log("Server vs device player state match.");
                    Debug.Log("State: " + JsonUtility.ToJson(serverPlayerState));
                }

                if (!serverOpponentState.Equals(deviceOpponentState))
                {
                    Debug.LogWarning("Server vs device opponent state mismatch.");
                    Debug.LogWarning("Server: " + JsonUtility.ToJson(serverOpponentState));
                    Debug.LogWarning("Device: " + JsonUtility.ToJson(deviceOpponentState));
                    Debug.LogWarning("First diff: " + serverOpponentState.FirstDiff(deviceOpponentState));
                }
                else
                {
                    Debug.Log("Server vs device opponent state match.");
                    Debug.Log("State: " + JsonUtility.ToJson(serverOpponentState));
                }
            }
            else
            {
                Debug.Log("Player state: " + JsonUtility.ToJson(devicePlayerState));
                Debug.Log("Opponent state: " + JsonUtility.ToJson(deviceOpponentState));
            }
        }

        return serverMove.Rank;
    }

    private PlayerState GetPlayerState()
    {
        return this.you.GeneratePlayerState();
    }

    public PlayerState GetOpponentState()
    {
        return this.opponent.GeneratePlayerState();
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
}
