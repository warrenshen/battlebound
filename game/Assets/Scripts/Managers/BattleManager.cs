﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
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
    public int boardLayer;
    //private List<HistoryItem> history;

    [SerializeField]
    private Targetable mouseDownTargetable;
    private Targetable mouseUpTargetable;
    private List<Targetable> validTargets; //used to store/cache valid targets

    public CurvedLineRenderer attackCommand;
    public int stencilCount;

    public static BattleManager Instance { get; private set; }

    public PlayerState GetPlayerState()
    {
        return this.you.GeneratePlayerState();
    }

    public PlayerState GetOpponentState()
    {
        return this.opponent.GeneratePlayerState();
    }

    public Player getPlayerById(string playerId)
    {
        return this.playerIdToPlayer[playerId];
    }

    private void Awake()
    {
        Instance = this;

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

                this.you = new Player(BattleSingleton.Instance.PlayerState, "Player");
                this.opponent = new Player(BattleSingleton.Instance.OpponentState, "Enemy");
                this.initialized = true;
            }
        }
        else
        {
            this.you = new Player("Player", "Player");
            this.opponent = new Player("Enemy", "Enemy");
        }

        attackCommand.SetWidth(0);
    }

    private void Start()
    {
        if (!InspectorControlPanel.Instance.DevelopmentMode || this.initialized)
        {
            this.players = new List<Player>();
            this.players.Add(this.you);
            this.players.Add(this.opponent);
            this.stencilCount = 1;

            this.playerIdToPlayer = new Dictionary<string, Player>();
            this.playerIdToPlayer[this.you.Id] = this.you;
            this.playerIdToPlayer[this.opponent.Id] = this.opponent;

            // Use this for initialization
            battleLayer = 9;
            boardLayer = LayerMask.GetMask("Board");

            ChooseRandomSetting();
            GameStart();
        }
    }

    private void GameStart()
    {
        if (!InspectorControlPanel.Instance.DevelopmentMode)
        {
            this.you.BeginMulligan(this.you.PopCardsFromDeck(3), true);
            this.opponent.BeginMulligan(this.opponent.PopCardsFromDeck(3), true);

            turnIndex = UnityEngine.Random.Range(0, players.Count);
            activePlayer = players[turnIndex % players.Count];
            activePlayer.RenderTurnStart();
        }
        else
        {
            turnIndex = this.players.FindIndex(player => player.HasTurn);
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

    private void Update()
    {
        WatchMouseActions();
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
                return;
            mouseDownTargetable = hit.collider.GetComponent<Targetable>();
            if (mouseDownTargetable == null)
            {
                Debug.LogError("Raycast hit an object in battle layer that is not of class Targetable...");
                return;
            }
            if (!mouseDownTargetable.Owner.HasTurn || mouseDownTargetable.CanAttack <= 0)
                return;

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
            if (cast && hit.collider.gameObject.layer == battleLayer && mouseDownTargetable != null) //use battle layer mask
            {
                CheckFight(mouseDownTargetable, hit);
            }
            else if (CheckPlayCard(ray, hit))
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
        if (activePlayer == you)
            ActionManager.Instance.SetCursor(0);
        else
            ActionManager.Instance.SetCursor(2);
    }

    private void OnEndTurnClick()
    {
        if (this.you.Mode != Player.PLAYER_STATE_MODE_NORMAL)   //dont allow end turn button click in mulligan or non-normal state
            return;

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
        activePlayer.EndTurn();

        turnIndex++;
        activePlayer = players[turnIndex % players.Count];

        //do some turn transition render
        GameObject.Find("Turn Indicator").GetComponent<TextMeshPro>().text = string.Format("{0} Turn", activePlayer.Name);
        SetPassiveCursor();
        //to-do: action manager set active = false, but that makes singleplayer broken

        activePlayer.NewTurn();
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


    private void CheckFight(Targetable attacker, RaycastHit hit)
    {
        if (!attackCommand.enabled)
        {
            return;
        }

        mouseUpTargetable = hit.collider.GetComponent<Targetable>();
        if (mouseUpTargetable == mouseDownTargetable)
        {
            //show creature details
            Debug.Log("Show details.");
        }
        else if (validTargets != null && validTargets.Count > 0 && validTargets.Contains(mouseUpTargetable))
        {
            if (InspectorControlPanel.Instance.DevelopmentMode)
            {
                CardAttackAttributes attributes = new CardAttackAttributes(
                    mouseUpTargetable.GetPlayerId(),
                    mouseUpTargetable.GetCardId()
                );
                BattleSingleton.Instance.SendChallengeCardAttackRequest(
                    mouseDownTargetable.GetCardId(),
                    attributes
                );
            }

            mouseDownTargetable.Fight(mouseUpTargetable);
        }
    }

    private float GetCardDisplacement(CardObject target)
    {
        Vector3 initial = target.reset.position;
        Vector3 release = target.transform.localPosition;
        Vector3 diff = initial - release;
        return diff.sqrMagnitude;
    }


    private bool CheckPlayCard(Ray ray, RaycastHit hit)
    {
        if (!ActionManager.Instance.HasDragTarget())
            return false;

        CardObject target = ActionManager.Instance.GetDragTarget();
        float minThreshold = 20;
        //Debug.Log(String.Format("amt={0}, threshold={1}, mousepos={2}, resetpos={3}", GetCardDisplacement(target), Screen.height * minThreshold, target.transform.localPosition, target.reset.position));
        if (target.Card.Cost > target.Owner.Mana)
        {
            //can't play card due to mana
            ActionManager.Instance.ResetTarget();
            return false;
        }
        else if (GetCardDisplacement(target) < minThreshold)
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
        else if (Physics.Raycast(ray, out hit, 100f, boardLayer) && hit.collider.name.Contains(target.Owner.Name))
        {
            //place card
            Debug.Log("Mouse up with board playable card.");
            target.Owner.PlayCard(target);
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

    private IEnumerator EnemyPlayCardToBoardAnim(object[] args)
    {
        CardObject cardObject = (CardObject)args[0];
        int fieldIndex = (int)args[1];

        Transform pivotPoint = GameObject.Find("EnemyPlayCardFixed").transform;

        LeanTween.move(cardObject.gameObject, pivotPoint.position, 0.4f).setEaseInQuad();
        LeanTween.rotate(cardObject.gameObject, pivotPoint.rotation.eulerAngles, 0.4f).setEaseInQuad();
        yield return new WaitForSeconds(0.4f);

        //flash or something
        yield return new WaitForSeconds(1);

        PlayCardToBoard(cardObject, fieldIndex);
    }

    public void AnimateDrawCard(Player player, CardObject cardObject)
    {
        StartCoroutine("DrawCardAnimation", new object[2] { player, cardObject });
    }

    private IEnumerator DrawCardAnimation(object[] args)
    {
        Player player = args[0] as Player;
        CardObject cardObject = args[1] as CardObject;

        GameObject deckObject = GameObject.Find(String.Format("{0} Deck", player.Name));
        cardObject.transform.position = deckObject.transform.position;
        cardObject.transform.rotation = deckObject.transform.rotation;
        //done initializing to match deck orientation

        string fixedPointName = String.Format("{0}DrawCardFixed", player.Name);
        GameObject fixedPoint = GameObject.Find(fixedPointName);

        float tweenTime = 0.3F;
        LeanTween.rotate(cardObject.gameObject, fixedPoint.transform.rotation.eulerAngles, tweenTime).setEaseInQuad();
        LeanTween.move(cardObject.gameObject, fixedPoint.transform.position, tweenTime).setEaseInQuad();
        yield return new WaitForSeconds(tweenTime);


        yield return new WaitForSeconds(tweenTime);
        player.Hand.RepositionCards();
    }

    public void AnimateDrawCardForMulligan(Player player, CardObject cardObject, int position)
    {
        string targetPointName = String.Format("{0} Mulligan Holder {1}", player.Name, position);
        GameObject targetPoint = GameObject.Find(targetPointName);
        cardObject.transform.position = targetPoint.transform.position;
        cardObject.transform.localScale = Vector3.zero;

        float tweenTime = 0.3F;
        LeanTween.scale(cardObject.gameObject, cardObject.reset.scale, tweenTime);
        LeanTween.rotate(cardObject.gameObject, Camera.main.transform.rotation.eulerAngles, tweenTime).setEaseInQuad();
        LeanTween.move(cardObject.gameObject, targetPoint.transform.position + Vector3.up * 2.3F + Vector3.back * 0.2F, tweenTime).setEaseInQuad();
        cardObject.visual.Redraw();
    }

    public void HideMulliganOverlay(Player player)
    {
        StartCoroutine("AnimateHideMulliganOverlay", player);
    }

    private IEnumerator AnimateHideMulliganOverlay(Player player)
    {
        float tweenTime = 0.3F;
        GameObject overlay = GameObject.Find(String.Format("{0} Mulligan Overlay", player.Name));
        LeanTween.scale(overlay, Vector3.zero, tweenTime);
        yield return new WaitForSeconds(tweenTime);
        overlay.SetActive(false);
    }

    public void ToggleMulliganCard(CardObject cardObject)
    {
        if (cardObject.Owner.KeptMulliganCards.Contains(cardObject.Card))
        {
            cardObject.Owner.KeptMulliganCards.Remove(cardObject.Card);
            cardObject.Owner.RemovedMulliganCards.Add(cardObject.Card);

            //apply symbol/icon
            cardObject.visual.SetGrayscale(true);
            cardObject.visual.SetOutline(false);
        }
        else
        {
            cardObject.Owner.RemovedMulliganCards.Remove(cardObject.Card);
            cardObject.Owner.KeptMulliganCards.Add(cardObject.Card);
            //visuals
            cardObject.visual.SetGrayscale(false);
            cardObject.visual.SetOutline(true);
        }
    }

    public void FinishedMulligan()
    {
        this.you.PlayMulligan(this.opponent.Mode);
    }

    /*
     * Play card to board after receiving play card move from server. 
     */
    public void PlayCardToBoard(CardObject cardObject, int index)
    {
        SpawnCardToBoard(cardObject, index);
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
        Player player = cardObject.Owner;
        SpawnCardToBoard(cardObject, index);


        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            PlayCardAttributes attributes = new PlayCardAttributes(index);
            BattleSingleton.Instance.SendChallengePlayCardRequest(
                cardObject.Card.Id,
                attributes
            );
        }
    }

    private void SpawnCardToBoard(CardObject cardObject, int fieldIndex)
    {
        cardObject.visual.Renderer.enabled = false;
        SoundManager.Instance.PlaySound("PlayCardSFX", transform.position);
        Board.Instance.CreateAndPlaceCreature(cardObject, fieldIndex);
    }

    /*
     * Play spell card after user on-device drags card from hand to field. 
     */
    public void PlayTargetedSpell(CardObject cardObject, RaycastHit hit)
    {
        BoardCreature targetedCreature = hit.collider.GetComponent<BoardCreature>();
        SpellCard spellCard = cardObject.Card as SpellCard;
        spellCard.Activate(targetedCreature);
        UseCard(cardObject.Owner, cardObject);

        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            PlaySpellTargetedAttributes attributes = new PlaySpellTargetedAttributes(
                targetedCreature.Owner.Id,
                targetedCreature.CreatureCard.Id
            );
            BattleSingleton.Instance.SendChallengePlaySpellTargetedRequest(
                cardObject.Card.Id,
                attributes
            );
        }
    }

    /*
     * Play spell card after receiving play card move from server. 
     */
    public void PlayTargetedSpell(CardObject target, BoardCreature victimCreature)
    {
        ((SpellCard)target.Card).Activate(victimCreature);
        UseCard(target.Owner, target);
    }

    public void UseCard(Player player, CardObject cardObject)
    {
        GameObject.Destroy(cardObject.gameObject);
        SoundManager.Instance.PlaySound("PlayCardSFX", transform.position);
    }

    public void ReceiveMovePlayMulligan(string playerId, List<int> deckCardIndices)
    {
        this.you.ShowMulligan(deckCardIndices, this.opponent);
    }

    public void ReceiveMoveEndTurn(string playerId)
    {
        NextTurn();
    }

    public void ReceiveMoveDrawCard(string playerId, Card card)
    {
        this.activePlayer.AddDrawnCard(card);
    }

    public void ReceiveMovePlayMinion(string playerId, string cardId, CreatureCard card, int handIndex, int fieldIndex)
    {
        if (!InspectorControlPanel.Instance.DevelopmentMode)
        {
            CardObject target = opponent.Hand.GetCardObjectByCardId(cardId);
            if (target == null)
            {
                Debug.LogError(String.Format("Server demanded card to play, but none of id {0} was found.", cardId));
                return;
            }
            opponent.PlayCard(target);
            StartCoroutine("EnemyPlayCardToBoardAnim", new object[2] { target, fieldIndex });
        }
        else
        {
            CardObject target = opponent.Hand.GetCardObjectByIndex(handIndex);
            target.InitializeCard(opponent, card);
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

        if (!InspectorControlPanel.Instance.DevelopmentMode)
        {
            CardObject target = opponent.Hand.GetCardObjectByCardId(cardId);
            if (target == null)
            {
                Debug.LogError(String.Format("Demanded card to play, but none of id {0} was found.", cardId));
                return;
            }
            opponent.PlayCard(target);
            PlayTargetedSpell(target, victimCreature);
        }
        else
        {
            int opponentHandIndex = opponent.GetOpponentHandIndex(handIndex);
            CardObject target = opponent.Hand.GetCardObjectByIndex(opponentHandIndex);
            target.InitializeCard(opponent, card);
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
        if (!InspectorControlPanel.Instance.DevelopmentMode)
        {
            CardObject target = opponent.Hand.GetCardObjectByCardId(cardId);
            if (target == null)
            {
                Debug.LogError(String.Format("Demanded card to play, but none of id {0} was found.", cardId));
                return;
            }
            opponent.PlayCard(target);
        }
        else
        {
            int opponentHandIndex = opponent.GetOpponentHandIndex(handIndex);
            CardObject target = opponent.Hand.GetCardObjectByIndex(opponentHandIndex);
            target.InitializeCard(opponent, card);
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
        Targetable attacker = Board.Instance.GetTargetableByPlayerIdAndCardId(playerId, cardId);
        Targetable defender = Board.Instance.GetTargetableByPlayerIdAndCardId(fieldId, targetId);
        attacker.Fight(defender);
    }

    public void ReceiveChallengeEndState(ChallengeEndState challengeEndState)
    {

    }
}
