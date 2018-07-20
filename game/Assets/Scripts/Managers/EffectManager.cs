using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EffectManager : MonoBehaviour
{
    private List<Effect> hQueue; // High priority.
    private List<Effect> mQueue; // Medium priority.
    private List<Effect> lQueue; // Low priority.

    private bool isWaiting;
    private bool isDirty;

    private UnityAction callback;

    public static EffectManager Instance { get; private set; }

    public const string EFFECT_CARD_DIE = "EFFECT_CARD_DIE";
    public const string EFFECT_CARD_DIE_AFTER_DEATH_RATTLE = "EFFECT_CARD_DIE_AFTER_DEATH_RATTLE";
    public const string EFFECT_PLAYER_AVATAR_DIE = "EFFECT_PLAYER_AVATAR_DIE";
    public const string EFFECT_DEATH_RATTLE_ATTACK_RANDOM = "EFFECT_DEATH_RATTLE_ATTACK_RANDOM";

    private static readonly List<string> EFFECT_H_PRIORITY_ORDER = new List<string>
    {
        // Deal damage.
        Card.CARD_ABILITY_LIFE_STEAL,

        // Take damage.
        Card.CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY,

        // Die should be the last "now" effect since all other
        // "now" should finish before it removes card from board.
        EFFECT_CARD_DIE,
    };

    private static readonly List<string> EFFECT_M_PRIORITY_ORDER = new List<string>
    {
        EFFECT_DEATH_RATTLE_ATTACK_RANDOM,
        EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
    };

    private static readonly List<string> EFFECT_L_PRIORITY_ORDER = new List<string>
    {
        // Start turn.
        Card.BUFF_CATEGORY_UNSTABLE_POWER,

        // End turn.
        Card.CARD_ABILITY_END_TURN_HEAL_TEN,
        Card.CARD_ABILITY_END_TURN_HEAL_TWENTY,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_END_TURN_DRAW_CARD,

        // Battlecry.
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD,

        // Deathrattle.
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,

        EFFECT_PLAYER_AVATAR_DIE,
    };

    private static readonly List<string> EFFECTS_END_TURN = new List<string>
    {
        Card.CARD_ABILITY_END_TURN_HEAL_TEN,
        Card.CARD_ABILITY_END_TURN_HEAL_TWENTY,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_END_TURN_DRAW_CARD,
    };

    private static readonly List<string> EFFECTS_START_TURN = new List<string>
    {
        Card.BUFF_CATEGORY_UNSTABLE_POWER,
    };

    private static readonly List<string> EFFECTS_BATTLE_CRY = new List<string>
    {
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD,
    };

    private static readonly List<string> EFFECT_DEATH_RATTLE = new List<string>
    {
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
    };

    private static readonly List<string> EFFECT_DAMAGE_TAKEN = new List<string>
    {
        Card.CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY,
    };

    private class Effect
    {
        private string playerId;
        public string PlayerId => playerId;

        private string name;
        public string Name => name;

        private string cardId;
        public string CardId => cardId;

        private int spawnRank;
        public int SpawnRank => spawnRank;

        private int value;
        public int Value => value;

        public Effect(
            string playerId,
            string name,
            string cardId,
            int spawnRank,
            int value = 0
        )
        {
            this.playerId = playerId;
            this.name = name;
            this.cardId = cardId;
            this.spawnRank = spawnRank;
            this.value = value;
        }

        public void SetValue(int value)
        {
            this.value = value;
        }
    }

    private void Awake()
    {
        Instance = this;

        this.hQueue = new List<Effect>();
        this.mQueue = new List<Effect>();
        this.lQueue = new List<Effect>();

        this.isWaiting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isWaiting)
        {
            return;
        }

        if (
            this.hQueue.Count <= 0 &&
            this.mQueue.Count <= 0 &&
            this.lQueue.Count <= 0
        )
        {
            if (this.callback != null)
            {
                // TODO: callback should check dirty.
                UnityAction action = this.callback;
                this.callback = null;
                action();
            }
            else
            {
                if (this.isDirty)
                {
                    BattleManager.Instance.ComparePlayerStates();
                    this.isDirty = false;
                }
            }

            BattleManager.Instance.ProcessMoveQueue();
        }
        else if (this.hQueue.Count > 0)
        {
            ProcessHQueue();
        }
        else if (this.mQueue.Count > 0)
        {
            ProcessMQueue();
        }
        else
        {
            ProcessLQueue();
        }
    }

    private void AddToQueues(Effect effect)
    {
        AddToQueues(new List<Effect> { effect });
    }

    private void AddToQueues(List<Effect> effects)
    {
        List<Effect> hEffects = new List<Effect>(
            effects.Where(effect => EFFECT_H_PRIORITY_ORDER.Contains(effect.Name))
        );
        List<Effect> mEffects = new List<Effect>(
            effects.Where(effect => EFFECT_M_PRIORITY_ORDER.Contains(effect.Name))
        );
        List<Effect> lEffects = new List<Effect>(
            effects.Where(effect => EFFECT_L_PRIORITY_ORDER.Contains(effect.Name))
        );

        if (hEffects.Count + mEffects.Count + lEffects.Count != effects.Count)
        {
            Debug.LogError("Somehow have different number of effects after partition than before");
        }

        hEffects.Sort(delegate (Effect a, Effect b)
        {
            int aOrder = EFFECT_H_PRIORITY_ORDER.IndexOf(a.Name);
            int bOrder = EFFECT_H_PRIORITY_ORDER.IndexOf(b.Name);

            if (a.SpawnRank == b.SpawnRank)
            {
                return aOrder < bOrder ? -1 : 1;
            }
            else
            {
                return a.SpawnRank < b.SpawnRank ? -1 : 1;
            }
        });
        mEffects.Sort(delegate (Effect a, Effect b)
        {
            int aOrder = EFFECT_M_PRIORITY_ORDER.IndexOf(a.Name);
            int bOrder = EFFECT_M_PRIORITY_ORDER.IndexOf(b.Name);

            if (a.SpawnRank == b.SpawnRank)
            {
                return aOrder < bOrder ? -1 : 1;
            }
            else
            {
                return a.SpawnRank < b.SpawnRank ? -1 : 1;
            }
        });
        lEffects.Sort(delegate (Effect a, Effect b)
        {
            int aOrder = EFFECT_L_PRIORITY_ORDER.IndexOf(a.Name);
            int bOrder = EFFECT_L_PRIORITY_ORDER.IndexOf(b.Name);

            if (a.SpawnRank == b.SpawnRank)
            {
                return aOrder < bOrder ? -1 : 1;
            }
            else
            {
                return a.SpawnRank < b.SpawnRank ? -1 : 1;
            }
        });

        this.hQueue.AddRange(hEffects);
        this.mQueue.AddRange(mEffects);
        this.lQueue.AddRange(lEffects);
    }

    private void ProcessHQueue()
    {
        if (this.hQueue.Count <= 0)
        {
            Debug.LogError("Process queue called when queue is empty.");
            return;
        }

        Effect effect = this.hQueue[0];
        this.hQueue.RemoveAt(0);

        BoardCreature boardCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        if (boardCreature == null)
        {
            Debug.LogError("Invalid effect - board creature does not exist.");
            return;
        }

        switch (effect.Name)
        {
            case Card.CARD_ABILITY_LIFE_STEAL:
                AbilityLifeSteal(effect);
                break;
            case Card.CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY:
                AbilityDamageTakenDamagePlayerFace(effect, 30);
                break;
            case EFFECT_CARD_DIE:
                Board.Instance.RemoveCreatureByPlayerIdAndCardId(
                    effect.PlayerId,
                    effect.CardId
                );
                boardCreature.Die();
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect: {0}.", effect.Name));
                break;
        }
    }

    private void ProcessMQueue()
    {
        if (this.mQueue.Count <= 0)
        {
            Debug.LogError("Process queue called when queue is empty.");
            return;
        }

        Effect effect = this.mQueue[0];
        this.mQueue.RemoveAt(0);

        BoardCreature boardCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        if (boardCreature == null)
        {
            Debug.LogError("Invalid effect - board creature does not exist.");
            return;
        }

        switch (effect.Name)
        {
            case EFFECT_DEATH_RATTLE_ATTACK_RANDOM:
                EffectDeathRattleAttackRandom(effect);
                break;
            case EFFECT_CARD_DIE_AFTER_DEATH_RATTLE:
                Board.Instance.RemoveCreatureByPlayerIdAndCardId(
                    effect.PlayerId,
                    effect.CardId
                );
                boardCreature.Die();
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect: {0}.", effect.Name));
                break;
        }
    }

    private void ProcessLQueue()
    {
        if (this.lQueue.Count <= 0)
        {
            Debug.LogError("Process queue called when queue is empty.");
            return;
        }

        Effect effect = this.lQueue[0];
        this.lQueue.RemoveAt(0);

        BoardCreature boardCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        if (boardCreature == null)
        {
            Debug.LogError("Invalid effect - board creature does not exist.");
            return;
        }

        switch (effect.Name)
        {
            case EFFECT_PLAYER_AVATAR_DIE:
                Debug.LogWarning("Game over");
                break;
            case Card.CARD_ABILITY_END_TURN_HEAL_TEN:
                boardCreature.Heal(10);
                break;
            case Card.CARD_ABILITY_END_TURN_HEAL_TWENTY:
                boardCreature.Heal(20);
                break;
            case Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN:
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN:
                AbilityAttackInFront(effect, 10);
                break;
            case Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY:
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY:
                AbilityAttackInFront(effect, 20);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY:
                AbilityDeathRattleAttackFace(effect, 20);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY:
                AbilityDeathRattleAttackRandomThree(effect);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD:
                AbilityDeathRattleDrawCard(effect);
                break;
            case Card.CARD_ABILITY_END_TURN_DRAW_CARD:
            case Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD:
                ChallengeMove challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(effect.PlayerId);
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD);
                challengeMove.SetRank(BattleManager.Instance.GetDeviceMoveRank());
                BattleManager.Instance.AddDeviceMove(challengeMove);

                this.isWaiting = true;
                StartCoroutine("WaitForDrawCard", new object[1] { challengeMove.Rank });

                if (!DeveloperPanel.IsServerEnabled())
                {
                    boardCreature.Owner.DrawCards(1);
                }
                break;
            case Card.BUFF_CATEGORY_UNSTABLE_POWER:
                BuffUnstablePower(effect);
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect: {0}.", effect.Name));
                break;
        }
    }

    private void EffectDeathRattleAttackRandom(Effect effect)
    {
        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(effect.PlayerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DEATH_RATTLE_ATTACK_RANDOM_TARGET);
        challengeMove.SetRank(BattleManager.Instance.GetDeviceMoveRank());
        BattleManager.Instance.AddDeviceMove(challengeMove);

        this.isWaiting = true;
        StartCoroutine("WaitForAttackRandom", new object[1] { challengeMove.Rank });

        if (!DeveloperPanel.IsServerEnabled())
        {
            Targetable randomTargetable = Board.Instance.GetOpponentRandomTargetable(effect.PlayerId);

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(effect.PlayerId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DEATH_RATTLE_ATTACK_RANDOM_TARGET);
            challengeMove.SetRank(BattleManager.Instance.GetServerMoveRank());

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCardId(effect.CardId);
            moveAttributes.SetFieldId(randomTargetable.Owner.Id);
            moveAttributes.SetTargetId(randomTargetable.GetCardId());
            challengeMove.SetMoveAttributes(moveAttributes);

            BattleManager.Instance.ReceiveChallengeMove(challengeMove);
        }
    }

    private IEnumerator WaitForAttackRandom(object[] args)
    {
        int moveRank = (int)args[0];
        while (BattleManager.Instance.ProcessMoveQueue() != moveRank)
        {
            yield return new WaitForSeconds(0.1f);
        }

        this.isWaiting = false;
    }

    private void AbilityDamageTakenDamagePlayerFace(Effect effect, int amount)
    {
        Player player = BattleManager.Instance.GetPlayerById(effect.PlayerId);
        player.TakeDamage(amount);
    }

    private void AbilityLifeSteal(Effect effect)
    {
        Player player = BattleManager.Instance.GetPlayerById(effect.PlayerId);
        player.Heal(effect.Value);
    }

    private void AbilityAttackInFront(Effect effect, int amount)
    {
        BoardCreature attackingCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );
        BoardCreature defendingCreature = Board.Instance.GetInFrontCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        List<Effect> effects = new List<Effect>();
        // TODO: use pooling so don't instantiate?
        string effectName = "VFX/FanMeteorsVFX"; //attackingCreature.CreatureCard.GetEffectPrefab();
        GameObject effectVFX = Instantiate(
            ResourceSingleton.Instance.GetEffectPrefabByName(effectName),
            attackingCreature.transform.position,
            Quaternion.identity
        );
        effectVFX.transform.parent = attackingCreature.transform;
        effectVFX.transform.Translate(Vector3.back * 1 + Vector3.up * 1, Space.Self);

        Transform defendingTransform = Board.Instance.GetInFrontBoardPlaceByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );
        if (defendingTransform)
        {
            effectVFX.transform.LookAt(defendingTransform);
        }

        if (effectVFX == null)
        {
            Debug.LogError("Should render attack in front effect.");
        }

        if (defendingCreature != null)
        {
            int damageDone = defendingCreature.TakeDamage(amount);
            effects.AddRange(GetEffectsOnCreatureDamageDealt(attackingCreature, damageDone));
            effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

            if (defendingCreature.Health <= 0)
            {
                effects.AddRange(GetEffectsOnCreatureDeath(defendingCreature));
            }
        }

        AddToQueues(effects);
    }

    private void AbilityDeathRattleAttackFace(Effect effect, int amount)
    {
        List<Effect> effects = new List<Effect>();

        effects.Add(
            new Effect(
                effect.PlayerId,
                EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
                effect.CardId,
                effect.SpawnRank
            )
        );

        AddToQueues(effects);
    }

    private void AbilityDeathRattleAttackRandomThree(Effect effect)
    {
        List<Effect> effects = new List<Effect>();

        for (int i = 0; i < 3; i += 1)
        {
            effects.Add(
                new Effect(
                    effect.PlayerId,
                    EFFECT_DEATH_RATTLE_ATTACK_RANDOM,
                    effect.CardId,
                    effect.SpawnRank
                )
            );
        }

        effects.Add(
            new Effect(
                effect.PlayerId,
                EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
                effect.CardId,
                effect.SpawnRank
            )
        );

        AddToQueues(effects);
    }

    private void AbilityDeathRattleDrawCard(Effect effect)
    {
        BoardCreature boardCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        List<Effect> effects = new List<Effect>();

        effects.Add(
            new Effect(
                effect.PlayerId,
                EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
                effect.CardId,
                effect.SpawnRank
            )
        );

        AddToQueues(effects);

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(effect.PlayerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD);
        challengeMove.SetRank(BattleManager.Instance.GetDeviceMoveRank());
        BattleManager.Instance.AddDeviceMove(challengeMove);

        this.isWaiting = true;
        StartCoroutine("WaitForDrawCard", new object[1] { challengeMove.Rank });

        if (!DeveloperPanel.IsServerEnabled())
        {
            boardCreature.Owner.DrawCards(1);
        }
    }

    private void BuffUnstablePower(Effect effect)
    {
        BoardCreature boardCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        boardCreature.SetHealth(0);

        List<Effect> effects = new List<Effect>();

        effects.AddRange(GetEffectsOnCreatureDeath(boardCreature));

        AddToQueues(effects);
    }

    private IEnumerator WaitForDrawCard(object[] args)
    {
        int moveRank = (int)args[0];
        while (BattleManager.Instance.ProcessMoveQueue() != moveRank)
        {
            yield return new WaitForSeconds(0.1f);
        }

        this.isWaiting = false;
    }

    public void OnStartTurn(string playerId)
    {
        Player player = BattleManager.Instance.GetPlayerById(playerId);

        if (!DeveloperPanel.IsServerEnabled())
        {
            player.DrawCards(1);
        }

        player.Avatar.OnStartTurn();

        List<Effect> effects = new List<Effect>();

        List<BoardCreature> boardCreatures = Board.Instance.GetAliveCreaturesByPlayerId(playerId);
        foreach (BoardCreature boardCreature in boardCreatures)
        {
            // Handles redrawing creature.
            boardCreature.OnStartTurn();

            foreach (string effectName in EFFECTS_START_TURN)
            {
                if (boardCreature.HasAbility(effectName) || boardCreature.HasBuff(effectName))
                {
                    effects.Add(
                        new Effect(
                            playerId,
                            effectName,
                            boardCreature.GetCardId(),
                            boardCreature.SpawnRank
                        )
                    );
                }
            }
        }

        AddToQueues(effects);
        this.isDirty = true;
    }

    public void OnEndTurn(string playerId, UnityAction callback)
    {
        // We do not set isDirty here because the susbequent
        // call to OnStartTurn will set it.
        //this.isDirty = true;
        this.callback = callback;

        Player player = BattleManager.Instance.GetPlayerById(playerId);

        //player.Avatar.OnEndTurn();

        List<Effect> effects = new List<Effect>();

        List<BoardCreature> boardCreatures = Board.Instance.GetAliveCreaturesByPlayerId(playerId);
        foreach (BoardCreature boardCreature in boardCreatures)
        {
            // Handles redrawing creature.
            boardCreature.OnEndTurn();

            foreach (string effectName in EFFECTS_END_TURN)
            {
                if (boardCreature.HasAbility(effectName) || boardCreature.HasBuff(effectName))
                {
                    effects.Add(
                        new Effect(
                            playerId,
                            effectName,
                            boardCreature.GetCardId(),
                            boardCreature.SpawnRank
                        )
                    );
                }
            }
        }

        AddToQueues(effects);
    }

    public void OnCreaturePlay(string playerId, string cardId)
    {
        BoardCreature boardCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            playerId,
            cardId
        );

        if (boardCreature == null)
        {
            Debug.LogError("On play called on board creature that does not exist.");
            return;
        }

        List<Effect> effects = new List<Effect>();

        foreach (string effectName in EFFECTS_BATTLE_CRY)
        {
            if (boardCreature.HasAbility(effectName) || boardCreature.HasBuff(effectName))
            {
                effects.Add(
                    new Effect(
                        playerId,
                        effectName,
                        boardCreature.GetCardId(),
                        boardCreature.SpawnRank
                    )
                );
            }
        }

        AddToQueues(effects);
        this.isDirty = true;
    }

    public void OnCreatureAttack(
        Targetable attackingTargetable,
        Targetable defendingTargetable
    )
    {
        if (
            attackingTargetable.GetType() == typeof(BoardCreature) &&
            defendingTargetable.GetType() == typeof(BoardCreature)
        )
        {
            BoardCreature attackingCreature = attackingTargetable as BoardCreature;
            BoardCreature defendingCreature = defendingTargetable as BoardCreature;

            if (attackingCreature.CanAttack <= 0)
            {
                Debug.LogError("Fight called when canAttack is 0 or below!");
                return;
            }
            else if (attackingCreature.IsFrozen > 0)
            {
                Debug.LogError("Fight called when isFrozen is greater than 0!");
                return;
            }
            else
            {
                attackingCreature.DecrementCanAttack();
            }

            List<Effect> effects = new List<Effect>();

            this.isWaiting = true;
            attackingCreature.FightAnimationWithCallback(defendingCreature, new UnityAction(
                () =>
                {
                    int damageDone = defendingCreature.TakeDamage(attackingCreature.Attack);
                    effects.AddRange(GetEffectsOnCreatureDamageDealt(attackingCreature, damageDone));
                    effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

                    int damageTaken = attackingCreature.TakeDamage(defendingCreature.Attack);
                    effects.AddRange(GetEffectsOnCreatureDamageDealt(defendingCreature, damageTaken));
                    effects.AddRange(GetEffectsOnCreatureDamageTaken(attackingCreature, damageTaken));

                    defendingCreature.Redraw();
                    attackingCreature.Redraw();

                    if (attackingCreature.Health <= 0)
                    {
                        effects.AddRange(GetEffectsOnCreatureDeath(attackingCreature));
                    }
                    if (defendingCreature.Health <= 0)
                    {
                        effects.AddRange(GetEffectsOnCreatureDeath(defendingCreature));
                    }

                    AddToQueues(effects);
                    this.isWaiting = false;
                    this.isDirty = true;
                }
            ));
        }
        else if (
            attackingTargetable.GetType() == typeof(BoardCreature) &&
            defendingTargetable.GetType() == typeof(PlayerAvatar)
        )
        {
            BoardCreature attackingCreature = attackingTargetable as BoardCreature;
            PlayerAvatar defendingAvatar = defendingTargetable as PlayerAvatar;

            if (attackingCreature.CanAttack <= 0)
            {
                Debug.LogError("Fight called when canAttack is 0 or below!");
                return;
            }
            else if (attackingCreature.IsFrozen > 0)
            {
                Debug.LogError("Fight called when isFrozen is greater than 0!");
                return;
            }
            else
            {
                attackingCreature.DecrementCanAttack();
            }

            List<Effect> effects = new List<Effect>();

            //int damageDone = attackingCreature.Fight(defendingAvatar);  //TakeDamage inside, int damageDone = defendingAvatar.TakeDamage(attackingCreature.Attack);

            this.isWaiting = true;
            attackingCreature.FightAnimationWithCallback(defendingAvatar, new UnityAction(
                () =>
                {
                    int damageDone = defendingAvatar.TakeDamage(attackingCreature.Attack);
                    effects.AddRange(GetEffectsOnCreatureDamageDealt(attackingCreature, damageDone));
                    //effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

                    //int damageReceived = attackingCreature.TakeDamage(defendingCreature.Attack);
                    //effects.AddRange(GetEffectsOnCreatureDamageDealt(defendingCreature, damageDone));
                    //effects.AddRange(GetEffectsOnCreatureDamageTaken(attackingCreature, damageDone));

                    defendingAvatar.Redraw();
                    attackingCreature.Redraw();

                    //if (attackingCreature.Health <= 0)
                    //{
                    //    effects.Add(
                    //        new Effect(
                    //            attackingCreature.Owner.Id,
                    //            EFFECT_CARD_DIE,
                    //            attackingCreature.GetCardId(),
                    //            attackingCreature.SpawnRank
                    //        )
                    //    );

                    //    effects.AddRange(GetEffectsOnCreatureDeath(attackingCreature));
                    //}
                    if (defendingAvatar.Health <= 0)
                    {
                        effects.Add(
                            new Effect(
                                defendingAvatar.Owner.Id,
                                EFFECT_PLAYER_AVATAR_DIE,
                                defendingAvatar.GetCardId(),
                                0
                            )
                        );
                    }

                    AddToQueues(effects);
                    this.isWaiting = false;
                    this.isDirty = true;
                }
            ));
        }
        else
        {
            Debug.LogError("Unsupported.");
        }
    }

    public void OnDeathRattleAttackRandomTarget(
        Targetable attackingTargetable,
        Targetable defendingTargetable
    )
    {
        if (attackingTargetable.GetType() == typeof(PlayerAvatar))
        {
            Debug.LogError("Player avatar cannot have death rattle.");
            return;
        }

        BoardCreature attackingCreature = attackingTargetable as BoardCreature;
        if (!attackingCreature.HasAbility(Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY))
        {
            Debug.LogError("Board creature does not have a death rattle attack random ability.");
            return;
        }


        if (defendingTargetable.GetType() == typeof(BoardCreature))
        {
            BoardCreature defendingCreature = defendingTargetable as BoardCreature;

            List<Effect> effects = new List<Effect>();

            // TODO: animate as bomb or whatever.

            int damageDone = defendingCreature.TakeDamage(20);

            effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

            defendingCreature.Redraw();
            attackingCreature.Redraw();

            if (defendingCreature.Health <= 0)
            {
                effects.AddRange(GetEffectsOnCreatureDeath(defendingCreature));
            }

            AddToQueues(effects);
        }
        else if (defendingTargetable.GetType() == typeof(PlayerAvatar))
        {
            PlayerAvatar defendingAvatar = defendingTargetable as PlayerAvatar;

            List<Effect> effects = new List<Effect>();

            // TODO: animate as bomb or whatever.

            attackingCreature.Fight(defendingAvatar);

            int damageDone = defendingAvatar.TakeDamage(20);
            //effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

            defendingAvatar.Redraw();
            attackingCreature.Redraw();

            if (defendingAvatar.Health <= 0)
            {
                effects.Add(
                    new Effect(
                        defendingAvatar.Owner.Id,
                        EFFECT_PLAYER_AVATAR_DIE,
                        defendingAvatar.GetCardId(),
                        0
                    )
                );
            }

            AddToQueues(effects);
            this.isDirty = true;
        }
        else
        {
            Debug.LogError("Unsupported.");
        }
    }

    public void OnSpellTargetedPlay(
        BattleCardObject battleCardObject,
        BoardCreature targetedCreature
    )
    {
        string playerId = battleCardObject.Owner.Id;

        SpellCard spellCard = battleCardObject.Card as SpellCard;

        List<Effect> effects = new List<Effect>();

        switch (spellCard.Name)
        {
            case SpellCard.SPELL_NAME_LIGHTNING_BOLT:
                effects = SpellTargetedLightningBolt(playerId, targetedCreature);
                break;
            case SpellCard.SPELL_NAME_UNSTABLE_POWER:
                effects = SpellTargetedUnstablePower(playerId, targetedCreature);
                break;
            case SpellCard.SPELL_NAME_DEEP_FREEZE:
                effects = SpellTargetedDeepFreeze(playerId, targetedCreature);
                break;
            case SpellCard.SPELL_NAME_WIDESPREAD_FROSTBITE:
                effects = SpellTargetedWidespreadFrostbite(playerId, targetedCreature);
                break;
            default:
                Debug.LogError(string.Format("Invalid targeted spell with name: {0}.", spellCard.Name));
                break;
        }

        AddToQueues(effects);
        this.isDirty = true;
    }

    private List<Effect> SpellTargetedLightningBolt(string playerId, BoardCreature targetedCreature)
    {
        SoundManager.Instance.PlaySound("ShockSFX", targetedCreature.transform.position);
        FXPoolManager.Instance.PlayEffect("LightningBoltVFX", targetedCreature.transform.position);

        List<Effect> effects = new List<Effect>();

        int damageTaken = targetedCreature.TakeDamage(30);
        effects.AddRange(GetEffectsOnCreatureDamageTaken(targetedCreature, damageTaken));

        targetedCreature.Redraw();

        if (targetedCreature.Health <= 0)
        {
            effects.AddRange(GetEffectsOnCreatureDeath(targetedCreature));
        }

        return effects;
    }

    private List<Effect> SpellTargetedUnstablePower(string playerId, BoardCreature targetedCreature)
    {
        targetedCreature.AddAttack(30);
        targetedCreature.AddBuff(Card.BUFF_CATEGORY_UNSTABLE_POWER);

        // No triggered effects on unstable power for now.
        return new List<Effect>();
    }

    private List<Effect> SpellTargetedDeepFreeze(string playerId, BoardCreature targetedCreature)
    {
        List<Effect> effects = new List<Effect>();

        int damageTaken = targetedCreature.TakeDamage(10);
        effects.AddRange(GetEffectsOnCreatureDamageTaken(targetedCreature, damageTaken));

        if (targetedCreature.Health <= 0)
        {
            effects.AddRange(GetEffectsOnCreatureDeath(targetedCreature));
        }
        else
        {
            targetedCreature.Freeze(1);
        }

        return effects;
    }

    private List<Effect> SpellTargetedWidespreadFrostbite(string playerId, BoardCreature targetedCreature)
    {
        targetedCreature.Freeze(2);

        BoardCreature inFrontCreature = Board.Instance.GetInFrontCreatureByPlayerIdAndCardId(
            targetedCreature.Owner.Id,
            targetedCreature.GetCardId()
        );

        if (inFrontCreature != null)
        {
            inFrontCreature.Freeze(2);
        }

        List<BoardCreature> adjacentCreatures =
            Board.Instance.GetAdjacentCreaturesByPlayerIdAndCardId(
                targetedCreature.Owner.Id,
                targetedCreature.GetCardId()
            );

        foreach (BoardCreature adjacentCreature in adjacentCreatures)
        {
            adjacentCreature.Freeze(1);
        }

        // No triggered effects on freeze for now.
        return new List<Effect>();
    }

    public void OnSpellUntargetedPlay(BattleCardObject battleCardObject)
    {
        string playerId = battleCardObject.Owner.Id;

        SpellCard spellCard = battleCardObject.Card as SpellCard;

        List<Effect> effects = new List<Effect>();

        switch (spellCard.Name)
        {
            case SpellCard.SPELL_NAME_RIOT_UP:
                effects = SpellUntargetedRiotUp(playerId);
                break;
            case SpellCard.SPELL_NAME_BRR_BRR_BLIZZARD:
                effects = SpellUntargetedBrrBrrBlizzard(playerId);
                break;
            default:
                Debug.LogError(string.Format("Invalid untargeted spell with name: {0}.", spellCard.Name));
                break;
        }

        AddToQueues(effects);
        this.isDirty = true;
    }

    private List<Effect> SpellUntargetedRiotUp(string playerId)
    {
        List<BoardCreature> boardCreatures = Board.Instance.GetAliveCreaturesByPlayerId(playerId);
        foreach (BoardCreature boardCreature in boardCreatures)
        {
            boardCreature.GrantShield();
        }

        // No triggered effects on grant shield for now.
        return new List<Effect>();
    }

    private List<Effect> SpellUntargetedBrrBrrBlizzard(string playerId)
    {
        List<BoardCreature> opponentAliveCreatures =
            Board.Instance.GetOpponentAliveCreaturesByPlayerId(playerId);

        foreach (BoardCreature aliveCreature in opponentAliveCreatures)
        {
            aliveCreature.Freeze(1);
        }

        // No triggered effects on freeze for now.
        return new List<Effect>();
    }

    private List<Effect> GetEffectsOnCreatureDamageDealt(
        BoardCreature boardCreature,
        int amount
    )
    {
        List<Effect> effects = new List<Effect>();

        if (boardCreature.HasAbility(Card.CARD_ABILITY_LIFE_STEAL))
        {
            effects.Add(
                new Effect(
                    boardCreature.Owner.Id,
                    Card.CARD_ABILITY_LIFE_STEAL,
                    boardCreature.GetCardId(),
                    boardCreature.SpawnRank,
                    amount
                )
            );
        }

        return effects;
    }

    private List<Effect> GetEffectsOnCreatureDamageTaken(
        BoardCreature boardCreature,
        int amount
    )
    {
        List<Effect> effects = new List<Effect>();

        foreach (string effectName in EFFECT_DAMAGE_TAKEN)
        {
            if (boardCreature.HasAbility(effectName) || boardCreature.HasBuff(effectName))
            {
                effects.Add(
                    new Effect(
                        boardCreature.Owner.Id,
                        effectName,
                        boardCreature.GetCardId(),
                        boardCreature.SpawnRank
                    )
                );
            }
        }

        return effects;
    }

    private List<Effect> GetEffectsOnCreatureDeath(BoardCreature boardCreature)
    {
        List<Effect> effects = new List<Effect>();

        foreach (string effectName in EFFECT_DEATH_RATTLE)
        {
            if (boardCreature.HasAbility(effectName) || boardCreature.HasBuff(effectName))
            {
                effects.Add(
                    new Effect(
                        boardCreature.Owner.Id,
                        effectName,
                        boardCreature.GetCardId(),
                        boardCreature.SpawnRank
                    )
                );
            }
        }

        // If no death ratlle effects, add in card die effect.
        // If there are death rattle effects, they will handle card die.
        if (effects.Count <= 0)
        {
            effects.Add(
                new Effect(
                    boardCreature.Owner.Id,
                    EFFECT_CARD_DIE,
                    boardCreature.GetCardId(),
                    boardCreature.SpawnRank
                )
            );
        }

        return effects;
    }
}
