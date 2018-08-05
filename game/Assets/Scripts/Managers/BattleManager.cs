﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class BattleManager : MonoBehaviour
{
    private static float CARD_DISPLACEMENT_THRESHOLD = 30;

    public int battleLayer;
    public int boardOrBattleLayer;
    //private List<HistoryItem> history;

    private TargetableObject mouseDownTargetableObject;
    private TargetableObject mouseUpTargetableObject;
    private List<TargetableObject> validTargets; //used to store/cache valid targets

    public CurvedLineRenderer attackCommand;

    // Cached transforms.
    [SerializeField]
    private Transform enemyPlayCardFixedTransform;
    [SerializeField]
    private Transform enemyDrawCardFixedTransform;
    [SerializeField]
    private Transform enemyDeckTransform;
    [SerializeField]
    private Transform enemyHandTransform;
    public Transform EnemyHandTransform => enemyHandTransform;

    [SerializeField]
    private Transform playerDrawCardFixedTransform;
    [SerializeField]
    private Transform playerDeckTransform;
    [SerializeField]
    private Transform playerHandTransform;
    public Transform PlayerHandTransform => playerHandTransform;

    private GameObject lightGameObject;
    public GameObject LightGameObject => lightGameObject;
    [SerializeField]
    private GameObject boardMessageGameObject;
    // --

    [SerializeField]
    private BasicButton endTurnButton;

    [SerializeField]
    private GameObject endOverlay;

    [SerializeField]
    private List<CardObject> xpCardObjects;

    public HyperCard.Card HoverCard;
    public Canvas canvas;

    public static BattleManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        this.lightGameObject = GameObject.Find("Point Light");
        this.attackCommand.SetWidth(0);
        this.validTargets = new List<TargetableObject>();
    }

    private void Start()
    {
        battleLayer = LayerMask.NameToLayer("Battle");
        boardOrBattleLayer = LayerMask.GetMask(new string[2] { "Board", "Battle" });
        ChooseRandomSetting();

        if (!FlagHelper.IsServerEnabled())
        {
            BattleState _ = BattleState.Instance();
            Debug.Log("Battle in Local Development Mode.");
            EffectManager.Instance.ReadyUp();
        }
        else if (BattleSingleton.Instance.ChallengeStarted)
        {
            BattleState _ = BattleState.InstantiateWithState(
                BattleSingleton.Instance.PlayerState,
                BattleSingleton.Instance.OpponentState,
                BattleSingleton.Instance.MoveCount,
                BattleSingleton.Instance.SpawnCount,
                BattleSingleton.Instance.InitDeadCards,
                BattleSingleton.Instance.InitServerMoves
            );
            Debug.Log("Battle in Connected Development Mode.");
            BattleState.Instance().GameStart();
        }
        else
        {
            SparkSingleton.Instance.AddAuthenticatedCallback(new UnityAction(SendFindMatchRequest));
        }
    }

    private void SendFindMatchRequest()
    {
        BattleSingleton.Instance.SendFindMatchRequest(
            MatchmakingManager.MATCH_TYPE_RANKED,
            "Deck1"
        );
    }

    private void Update()
    {
        if (BattleState.Instance().IsNormalMode() && !EffectManager.IsWaiting())
        {
            WatchMouseActions();
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
        this.validTargets = GetValidTargets(this.mouseDownTargetableObject);
        //to-do: don't show attack arrow unless mouse no longer in bounds of board creature
        attackCommand.SetPointPositions(this.mouseDownTargetableObject.transform.position, hit.point);
        attackCommand.SetWidth(1.66f);
        //attackCommand.lineRenderer.enabled = true; //this is being used as a validity check!!
        ActionManager.Instance.SetCursor(4);
    }

    private void ComputeOutlineColors()
    {
        if (Input.GetMouseButton(0) && ActionManager.Instance.HasDragTarget())
        {
            BattleCardObject target = ActionManager.Instance.GetDragTarget() as BattleCardObject;
            if (target != null && GetCardDisplacement(target) > CARD_DISPLACEMENT_THRESHOLD)
            {
                target.visual.SetOutlineColors(Color.cyan, Color.green);
            }
            else
            {
                target.visual.SetOutlineColors(target.visual.InitialOutlineStartColor, target.visual.InitialOutlineEndColor);
            }
        }
    }

    private void WatchMouseActions()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        this.ComputeOutlineColors();

        //begin exclusive logic checks
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 100f) || hit.collider.gameObject.layer != battleLayer) //use battle layer mask
            {
                return;
            }

            this.mouseDownTargetableObject = hit.collider.GetComponent<TargetableObject>();
            if (this.mouseDownTargetableObject == null)
            {
                Debug.LogError("Raycast hit an object in battle layer that is not of class Targetable...");
                return;
            }
            //Trigger any events as needed
            this.mouseDownTargetableObject.MouseDown();

            if (
                FlagHelper.IsServerEnabled() &&
                this.mouseDownTargetableObject.GetPlayerId() != BattleState.Instance().You.Id
            )
            {
                return;
            }

            if (!this.mouseDownTargetableObject.GetTargetable().CanAttackNow())
            {
                return;
            }

            //if (this.mouseDownTargetableObject.IsAvatar())
            //{
            //    PlayerAvatar avatar = this.mouseDownTargetableObject.Owner.Avatar;
            //    if (avatar.HasWeapon())
            //    {
            //        AttackStartMade(hit);
            //    }
            //}
            //else  // must be BoardCreature, otherwise would have returned
            //{
            //    AttackStartMade(hit);
            //}
            AttackStartMade(hit);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            bool cast = Physics.Raycast(ray, out hit, 100f);
            if (cast && CheckFight(this.mouseDownTargetableObject, hit)) //use battle layer mask
            {
                //do something?
            }
            else if (cast && CheckPlayCard(ray, hit))
            {
                //do something?
            }

            //reset state
            ActionManager.Instance.SetActive(true);
            this.attackCommand.SetWidth(0);
            this.mouseDownTargetableObject = null;
            this.mouseUpTargetableObject = null;
            this.validTargets = new List<TargetableObject>();
            SetPassiveCursor();
        }
        else if (attackCommand.lineRenderer.startWidth > 0 && Input.GetMouseButton(0))  //battle/fight arrow
        {
            RaycastHit hit;
            bool cast = Physics.Raycast(ray, out hit, 100f);
            if (cast)
            {
                attackCommand.SetPointPositions(this.mouseDownTargetableObject.transform.position, hit.point);

                if (
                    hit.collider.gameObject.layer == battleLayer &&
                    this.mouseDownTargetableObject != null &&
                    hit.collider.gameObject != this.mouseDownTargetableObject.gameObject &&
                    this.validTargets.Contains(hit.collider.GetComponent<TargetableObject>())
                )
                {
                    ActionManager.Instance.SetCursor(5);
                }
                else
                {
                    ActionManager.Instance.SetCursor(4);
                }
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
        if (BattleState.Instance().ActivePlayer.Id == BattleState.Instance().You.Id)
        {
            ActionManager.Instance.SetCursor(0);
        }
        else if (BattleState.Instance().ActivePlayer.Mode != Player.PLAYER_STATE_MODE_NORMAL)
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
        if (BattleState.Instance().ActivePlayer.Mode != Player.PLAYER_STATE_MODE_NORMAL)   // dont allow end turn button click in non-normal state
        {
            return;
        }

        if (FlagHelper.IsServerEnabled() && BattleState.Instance().ActivePlayer.Id != BattleState.Instance().You.Id)
        {
            return;
        }

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(BattleState.Instance().ActivePlayer.Id);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_END_TURN);
        BattleState.Instance().AddServerMove(challengeMove);

        if (FlagHelper.IsServerEnabled() && BattleState.Instance().ActivePlayer.Id == BattleState.Instance().You.Id)
        {
            BattleSingleton.Instance.SendChallengeEndTurnRequest();
        }
    }

    public void ToggleEndTurnButton()
    {
        this.endTurnButton.ToggleState();
    }

    public void SetBoardCenterText(string message)
    {
        // TODO: Cache!
        this.boardMessageGameObject.GetComponent<TextMeshPro>().text = message;
    }

    private void Surrender()
    {
        string playerId = BattleState.Instance().You.Id;
        string opponentId = BattleState.Instance().Opponent.Id;

        BattleSingleton.Instance.SendChallengeSurrenderRequest();

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(playerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SURRENDER_BY_CHOICE);
        BattleState.Instance().AddDeviceMove(challengeMove);

        challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(opponentId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CHALLENGE_OVER);
        BattleState.Instance().AddDeviceMove(challengeMove);

        if (!FlagHelper.IsServerEnabled())
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(playerId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SURRENDER_BY_CHOICE);
            BattleState.Instance().AddServerMove(challengeMove);

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(opponentId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CHALLENGE_OVER);
            BattleState.Instance().AddServerMove(challengeMove);
        }
    }

    private List<TargetableObject> GetValidTargets(TargetableObject attacker)
    {
        List<TargetableObject> targetableObjects = new List<TargetableObject>();
        foreach (Player player in BattleState.Instance().Players)
        {
            Debug.Log(attacker.GetPlayerId());
            if (player.Id == attacker.GetPlayerId())
            {
                continue;
            }

            List<BoardCreature> fieldCreatures = Board.Instance().GetAliveCreaturesByPlayerId(player.Id);
            List<BoardCreatureObject> fieldCreatureObjects = new List<BoardCreatureObject>(
                fieldCreatures.Select(fieldCreature => fieldCreature.GetTargetableObject() as BoardCreatureObject)
            );
            targetableObjects.AddRange(fieldCreatureObjects);
            targetableObjects.Add(player.Avatar.GetTargetableObject());
        }

        List<TargetableObject> priorityTargetableObjects = new List<TargetableObject>();
        foreach (TargetableObject targetableObject in targetableObjects)
        {
            if (targetableObject.IsAvatar())
            {
                continue;
            }

            BoardCreatureObject boardCreatureObject = targetableObject as BoardCreatureObject;
            if (boardCreatureObject.HasAbility(Card.CARD_ABILITY_TAUNT))
            {
                priorityTargetableObjects.Add(boardCreatureObject);
            }
        }

        if (priorityTargetableObjects.Count > 0)
        {
            return priorityTargetableObjects;
        }
        else
        {
            return targetableObjects;
        }
    }

    private bool CheckFight(TargetableObject attacker, RaycastHit hit)
    {
        if (!attackCommand.enabled)
        {
            return false;
        }
        if (hit.collider.gameObject.layer != battleLayer)
        {
            return false;
        }
        if (this.mouseDownTargetableObject == null)
        {
            return false;
        }

        this.mouseUpTargetableObject = hit.collider.GetComponent<TargetableObject>();
        if (this.mouseUpTargetableObject == this.mouseDownTargetableObject)
        {
            //show creature details
            Debug.Log("Show details.");
            return false;
        }

        Debug.Log(validTargets.Count);
        if (validTargets.Count > 0 && validTargets.Contains(this.mouseUpTargetableObject))
        {
            TargetableObject attackingTargetableObject = this.mouseDownTargetableObject;
            TargetableObject defendingTargetableObject = this.mouseUpTargetableObject;

            ChallengeMove challengeMove;

            if (attackingTargetableObject.GetPlayerId() == BattleState.Instance().You.Id)
            {
                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(attackingTargetableObject.GetPlayerId());
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CARD_ATTACK);
                BattleState.Instance().AddDeviceMove(challengeMove);
            }

            if (FlagHelper.IsServerEnabled())
            {
                CardAttackAttributes attributes = new CardAttackAttributes(
                    defendingTargetableObject.GetPlayerId(),
                    defendingTargetableObject.GetCardId()
                );
                BattleSingleton.Instance.SendChallengeCardAttackRequest(
                    attackingTargetableObject.GetCardId(),
                    attributes
                );
            }
            else
            {
                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(attackingTargetableObject.GetPlayerId());
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CARD_ATTACK);

                ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
                moveAttributes.SetCardId(attackingTargetableObject.GetCardId());
                moveAttributes.SetFieldId(defendingTargetableObject.GetPlayerId());
                moveAttributes.SetTargetId(defendingTargetableObject.GetCardId());

                challengeMove.SetMoveAttributes(moveAttributes);
                BattleState.Instance().AddServerMove(challengeMove);
            }

            if (attackingTargetableObject.GetPlayerId() == BattleState.Instance().You.Id)
            {
                EffectManager.Instance.OnCreatureAttack(
                    attackingTargetableObject.GetTargetable(),
                    defendingTargetableObject.GetTargetable()
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

        if (target.GetCost() > target.Owner.Mana)
        {
            Debug.Log("Mana");
            //can't play card due to mana
            ActionManager.Instance.ResetTarget();
            return false;
        }
        else if (GetCardDisplacement(target) < CARD_DISPLACEMENT_THRESHOLD)   //to-do: review this
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
                    if (CanPlayTargetedSpell(target, hit))
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
                        ActionManager.Instance.ResetTarget();
                        return false;
                    }
                }
                else
                {
                    ActionManager.Instance.ResetTarget();
                    return false;
                }
            }
            else if (CanPlayUntargetedSpell(target)) //not targeted spell, play freely
            {
                if (PlayUntargetedSpell(target))
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
        else if (target.Card.GetType() == typeof(WeaponCard))
        {
            target.Owner.Avatar.EquipWeapon(target.Card as WeaponCard);
            target.Owner.PlayCard(target);
            UseCard(target);    //to-do: change to own weapon func
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
        Transform deckTransform;
        if (player.Id == BattleState.Instance().You.Id)
        {
            deckTransform = this.playerDeckTransform;
        }
        else
        {
            deckTransform = this.enemyDeckTransform;
        }

        battleCardObject.transform.position = deckTransform.position;
        battleCardObject.transform.rotation = deckTransform.rotation;
        battleCardObject.visual.Redraw();

        Transform pivotPoint;
        if (player.Id == BattleState.Instance().You.Id)
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
        //battleCardObject.visual.Redraw();

        LeanTween.scale(battleCardObject.gameObject, battleCardObject.reset.scale, CardTween.TWEEN_DURATION);
        LeanTween.rotate(battleCardObject.gameObject, Camera.main.transform.rotation.eulerAngles, CardTween.TWEEN_DURATION).setEaseInQuad();
        CardTween
            .move(battleCardObject, targetPoint.transform.position + Vector3.up * 2.3F + Vector3.back * 0.2F, CardTween.TWEEN_DURATION)
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

                SetBoardCenterText(string.Format("{0} Turn", BattleState.Instance().ActivePlayer.Name));
                SetPassiveCursor();
            });
    }

    public void ToggleMulliganCard(BattleCardObject battleCardObject)
    {
        if (battleCardObject.Owner.KeptMulliganCards.Contains(battleCardObject.Card))
        {
            battleCardObject.Owner.KeptMulliganCards.Remove(battleCardObject.Card);

            //to-do: apply symbol/icon
            battleCardObject.visual.SetGrayscale(true);
            battleCardObject.visual.SetOutline(false);
        }
        else
        {
            battleCardObject.Owner.KeptMulliganCards.Add(battleCardObject.Card);
            //visuals
            battleCardObject.visual.SetGrayscale(false);
            battleCardObject.visual.SetOutline(true);
        }
    }

    public void FinishedMulligan()
    {
        Player you = BattleState.Instance().You;
        if (you.Mode == Player.PLAYER_STATE_MODE_MULLIGAN)
        {
            BattleState.Instance().You.PlayMulligan(BattleState.Instance().Opponent.Mode);
        }
        else
        {
            Debug.LogError("Finish mulligan clicked when not in mulligan active mode.");
        }
    }

    private bool CanPlayTargetedSpell(BattleCardObject battleCardObject, RaycastHit hit)
    {
        if (battleCardObject.Card.GetType() != typeof(SpellCard))
        {
            Debug.LogError("Function passed invalid card - should be spell card");
            return false;
        }

        SpellCard card = battleCardObject.Card as SpellCard;

        if (card.Targeted == false)
        {
            Debug.LogError("Function passed invalid card - should be spell targeted card");
            return false;
        }

        BoardCreatureObject targetedCreatureObject = hit.collider.GetComponent<BoardCreatureObject>();
        if (targetedCreatureObject == null)
        {
            return false;
        }

        string playerId = battleCardObject.Owner.Id;

        if (
            SpellCard.TARGETED_SPELLS_FRIENDLY_ONLY.Contains(battleCardObject.Card.Name)
            && playerId != targetedCreatureObject.GetPlayerId()
        )
        {
            return false;
        }
        else if (
            SpellCard.TARGETED_SPELLS_OPPONENT_ONLY.Contains(battleCardObject.Card.Name)
            && playerId == targetedCreatureObject.GetPlayerId()
        )
        {
            return false;
        }

        return true;
    }

    private bool CanPlayUntargetedSpell(BattleCardObject battleCardObject)
    {
        if (battleCardObject.Card.GetType() != typeof(SpellCard))
        {
            Debug.LogError("Function passed invalid card - should be spell card");
            return false;
        }

        SpellCard card = battleCardObject.Card as SpellCard;

        if (card.Targeted != false)
        {
            Debug.LogError("Function passed invalid card - should be spell untargeted card");
            return false;
        }

        if (card.Name == Card.CARD_NAME_GRAVE_DIGGING)
        {
            return true;
        }
        else if (card.Name == Card.CARD_NAME_THE_SEVEN)
        {
            return true;
        }
        else
        {
            return true;
        }
    }

    private bool CanPlayCardToBoard(BattleCardObject battleCardObject, RaycastHit hit)
    {
        Transform targetPosition = hit.collider.transform;
        string lastChar = hit.collider.name.Substring(hit.collider.name.Length - 1);
        int index = Int32.Parse(lastChar);

        Player player = battleCardObject.Owner;

        return Board.Instance().IsBoardPlaceOpen(player.Id, index);
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

        if (player.Id == BattleState.Instance().Opponent.Id)
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_MINION);

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCardId(battleCardObject.Card.Id);
            moveAttributes.SetCard(battleCardObject.Card.GetChallengeCard());
            moveAttributes.SetFieldIndex(index);

            challengeMove.SetMoveAttributes(moveAttributes);
            BattleState.Instance().AddServerMove(challengeMove);

            return false;
        }
        else
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_MINION);
            BattleState.Instance().AddDeviceMove(challengeMove);

            if (FlagHelper.IsServerEnabled())
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
                BattleState.Instance().AddServerMove(challengeMove);
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
        int spawnRank = BattleState.Instance().GetNewSpawnRank();
        ChallengeCard challengeCard = battleCardObject.GetChallengeCard();
        challengeCard.SetSpawnRank(spawnRank);

        UseCard(battleCardObject);

        Board.Instance().CreateAndPlaceCreature(
            challengeCard,
            fieldIndex,
            true
        );
    }

    /*
     * Play spell card after user on-device drags card from hand to field. 
     */
    public bool PlayTargetedSpell(BattleCardObject battleCardObject, RaycastHit hit)
    {
        BoardCreatureObject targetedCreatureObject = hit.collider.GetComponent<BoardCreatureObject>();

        Player player = battleCardObject.Owner;
        ChallengeMove challengeMove;

        if (player.Id == BattleState.Instance().Opponent.Id)
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED);

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCardId(battleCardObject.Card.Id);
            moveAttributes.SetCard(battleCardObject.Card.GetChallengeCard());
            moveAttributes.SetFieldId(targetedCreatureObject.GetPlayerId());
            moveAttributes.SetTargetId(targetedCreatureObject.GetCardId());

            challengeMove.SetMoveAttributes(moveAttributes);
            BattleState.Instance().AddServerMove(challengeMove);

            return false;
        }
        else
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED);
            BattleState.Instance().AddDeviceMove(challengeMove);

            if (FlagHelper.IsServerEnabled())
            {
                PlaySpellTargetedAttributes attributes = new PlaySpellTargetedAttributes(
                    targetedCreatureObject.GetPlayerId(),
                    targetedCreatureObject.GetCardId()
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
                BattleState.Instance().AddServerMove(challengeMove);
            }

            EffectManager.Instance.OnSpellTargetedPlay(
                battleCardObject,
                targetedCreatureObject.GetTargetable() as BoardCreature
            );
            UseCard(battleCardObject);

            return true;
        }
    }

    /*
     * Play targeted spell card after receiving play card move from server. 
     */
    public void PlayTargetedSpellFromServer(BattleCardObject battleCardObject, BoardCreature targetedCreature)
    {
        EffectManager.Instance.OnSpellTargetedPlay(battleCardObject, targetedCreature);
        UseCard(battleCardObject);
    }

    /*
     * Play spell card after user on-device drags card from hand to field. 
     */
    public bool PlayUntargetedSpell(BattleCardObject battleCardObject)
    {
        Player player = battleCardObject.Owner;
        ChallengeMove challengeMove;

        if (player.Id == BattleState.Instance().Opponent.Id)
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED);

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCardId(battleCardObject.Card.Id);
            moveAttributes.SetCard(battleCardObject.Card.GetChallengeCard());

            challengeMove.SetMoveAttributes(moveAttributes);
            BattleState.Instance().AddServerMove(challengeMove);

            return false;
        }
        else
        {
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(player.Id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED);
            BattleState.Instance().AddDeviceMove(challengeMove);

            if (FlagHelper.IsServerEnabled())
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
                BattleState.Instance().AddServerMove(challengeMove);
            }

            EffectManager.Instance.OnSpellUntargetedPlay(battleCardObject);
            UseCard(battleCardObject);

            return true;
        }
    }

    /*
     * Play untargeted spell card after receiving play card move from server. 
     */
    public void PlayUntargetedSpellFromServer(BattleCardObject battleCardObject)
    {
        EffectManager.Instance.OnSpellUntargetedPlay(battleCardObject);
        UseCard(battleCardObject);
    }

    public void UseCard(BattleCardObject battleCardObject)
    {
        SoundManager.Instance.PlaySound("PlayCardSFX", transform.position);
        battleCardObject.visual.Renderer.enabled = false;
        battleCardObject.Recycle();
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
        title.transform.localScale = Vector3.zero;

        if (won)
        {
            title.text = "Victory";
        }
        else
        {
            title.text = "Defeat";
        }

        LeanTween
            .scale(title.gameObject, Vector3.one, 1)
            .setOnComplete(() =>
            {
                if (won)
                {
                    endOverlay.transform.Find("WinFX").gameObject.SetActive(true);
                }
                else
                {
                    endOverlay.transform.Find("LoseFX").gameObject.SetActive(true);
                }

                LeanTween
                    .scale(title.gameObject, title.transform.localScale / 1.25f, CardTween.TWEEN_DURATION)
                    .setDelay(CardTween.TWEEN_DURATION * 3)
                    .setOnComplete(() =>
                    {
                        LeanTween
                            .moveY(title.gameObject, title.transform.position.y + 3f, CardTween.TWEEN_DURATION)
                           .setOnComplete(() =>
                           {
                               RenderEXPChanges(experienceCards);
                           });
                    });
            });
    }

    private LTDescr AnimateCardPlayed(BattleCardObject battleCardObject)
    {
        LeanTween
            .rotateLocal(battleCardObject.visual.gameObject, new Vector3(0, 180, 0), CardTween.TWEEN_DURATION)
            .setEaseInQuad();
        LeanTween
            .rotate(battleCardObject.gameObject, this.enemyPlayCardFixedTransform.rotation.eulerAngles, CardTween.TWEEN_DURATION)
            .setEaseInQuad();
        return CardTween
                .move(battleCardObject, this.enemyPlayCardFixedTransform.position, CardTween.TWEEN_DURATION)
                .setEaseInQuad();
    }

    public void EnemyPlayCardToBoardAnim(BattleCardObject battleCardObject, int fieldIndex)
    {
        this.AnimateCardPlayed(battleCardObject).setOnComplete(() =>
          {
              CardTween
                .move(battleCardObject, this.enemyPlayCardFixedTransform.position, CardTween.TWEEN_DURATION * 2F)
                .setOnComplete(() =>
                {
                    PlayCardToBoard(battleCardObject, fieldIndex);
                });
          });
    }

    public void EnemyPlaySpellTargetedAnim(BattleCardObject battleCardObject, BoardCreature targetedCreature)
    {
        this.AnimateCardPlayed(battleCardObject).setOnComplete(() =>
        {
            CardTween
                .move(battleCardObject, this.enemyPlayCardFixedTransform.position, CardTween.TWEEN_DURATION * 2F)
                .setOnComplete(() =>
                {
                    PlayTargetedSpellFromServer(battleCardObject, targetedCreature);
                });
        });
    }

    public void EnemyPlaySpellUntargetedAnim(BattleCardObject battleCardObject)
    {
        this.AnimateCardPlayed(battleCardObject).setOnComplete(() =>
        {
            CardTween
                .move(battleCardObject, this.enemyPlayCardFixedTransform.position, CardTween.TWEEN_DURATION * 2F)
                .setOnComplete(() =>
                {
                    PlayUntargetedSpellFromServer(battleCardObject);
                });
        });
    }

    public void ShowOpponentChat(int chatId)
    {
        Debug.Log(GetChatByChatId(chatId));
    }

    public static readonly Dictionary<int, string> CHAT_ID_TO_STRING = new Dictionary<int, string>
    {
        { 0, "GG" },
        { 1, "REKT" },
        { 2, "Wow" },
        { 3, "Prepare yourself" },
        { 4, "Heart of the cards" },
        { 5, "Get good" },
        { 6, "Well played" },
    };

    private static string GetChatByChatId(int chatId)
    {
        if (CHAT_ID_TO_STRING.ContainsKey(chatId))
        {
            return CHAT_ID_TO_STRING[chatId];
        }
        else
        {
            Debug.LogError(string.Format("Invalid chatId {0}", chatId));
            return null;
        }
    }
}
