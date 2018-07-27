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

    private List<ChallengeCard> deadCards;

    public static EffectManager Instance { get; private set; }

    public const string EFFECT_CARD_DIE = "EFFECT_CARD_DIE";
    public const string EFFECT_CARD_DIE_AFTER_DEATH_RATTLE = "EFFECT_CARD_DIE_AFTER_DEATH_RATTLE";
    public const string EFFECT_PLAYER_AVATAR_DIE = "EFFECT_PLAYER_AVATAR_DIE";
    public const string EFFECT_RANDOM_TARGET = "EFFECT_RANDOM_TARGET";
    public const string EFFECT_CHANGE_TURN_DRAW_CARD = "EFFECT_CHANGE_TURN_DRAW_CARD";
    public const string EFFECT_DRAW_CARD = "EFFECT_DRAW_CARD";

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
        EFFECT_RANDOM_TARGET,
        EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
    };

    private static readonly List<string> EFFECT_L_PRIORITY_ORDER = new List<string>
    {
        EFFECT_CHANGE_TURN_DRAW_CARD,
        EFFECT_DRAW_CARD,

        // Start turn.
        Card.BUFF_CATEGORY_UNSTABLE_POWER,

        // End turn.
        Card.CARD_ABILITY_END_TURN_HEAL_TEN,
        Card.CARD_ABILITY_END_TURN_HEAL_TWENTY,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_END_TURN_DRAW_CARD,
        Card.CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD,

        // Battlecry.
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX,
        Card.CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT,
        Card.CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY,
        Card.CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY,
        Card.CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY,
        Card.CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE,
        Card.CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES,
        Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD,

        // Deathrattle.
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN,
        Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,

        EFFECT_PLAYER_AVATAR_DIE,
    };

    private static readonly List<string> EFFECTS_END_TURN = new List<string>
    {
        Card.CARD_ABILITY_END_TURN_HEAL_TEN,
        Card.CARD_ABILITY_END_TURN_HEAL_TWENTY,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD,
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
        Card.CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX,
        Card.CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT,
        Card.CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY,
        Card.CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY,
        Card.CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY,
        Card.CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE,
        Card.CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES,
        Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD,
    };

    private static readonly List<string> EFFECT_DEATH_RATTLE = new List<string>
    {
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN,
        Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY,
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

        private ChallengeCard card;
        public ChallengeCard Card => card;

        public Effect(
            string playerId,
            string name,
            string cardId,
            int spawnRank
        )
        {
            this.playerId = playerId;
            this.name = name;
            this.cardId = cardId;
            this.spawnRank = spawnRank;
        }

        public void SetValue(int value)
        {
            this.value = value;
        }

        public void SetCard(ChallengeCard card)
        {
            this.card = card;
        }
    }

    private void Awake()
    {
        Instance = this;

        this.isWaiting = false;

        this.hQueue = new List<Effect>();
        this.mQueue = new List<Effect>();
        this.lQueue = new List<Effect>();

        this.deadCards = new List<ChallengeCard>();
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

    private void ProcessMQueue()
    {
        if (this.mQueue.Count <= 0)
        {
            Debug.LogError("Process queue called when queue is empty.");
            return;
        }

        Effect effect = this.mQueue[0];
        this.mQueue.RemoveAt(0);

        switch (effect.Name)
        {
            case EFFECT_RANDOM_TARGET:
                EffectRandomTarget(effect);
                break;
            case EFFECT_CARD_DIE_AFTER_DEATH_RATTLE:
                EffectCardDieAfterDeathRattle(effect);
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect: {0}.", effect.Name));
                break;
        }
    }

    private void EffectCardDieAfterDeathRattle(Effect effect)
    {
        BoardCreature boardCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        if (boardCreature == null)
        {
            Debug.LogError("Invalid effect - board creature does not exist.");
            return;
        }

        Board.Instance.RemoveCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );
        boardCreature.Die();
    }

    private void EffectRandomTarget(Effect effect)
    {
        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(effect.PlayerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET);
        challengeMove.SetRank(BattleManager.Instance.GetDeviceMoveRank());
        BattleManager.Instance.AddDeviceMove(challengeMove);

        this.isWaiting = true;
        StartCoroutine("WaitForAttackRandom", new object[1] { challengeMove.Rank });

        if (!DeveloperPanel.IsServerEnabled())
        {
            Targetable randomTargetable = Board.Instance.GetOpponentRandomTargetable(effect.PlayerId);

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(effect.PlayerId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET);
            challengeMove.SetRank(BattleManager.Instance.GetServerMoveRank());

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCard(effect.Card);
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

        switch (effect.Name)
        {
            case EFFECT_PLAYER_AVATAR_DIE:
                Debug.LogWarning("Game over");
                break;
            case Card.CARD_ABILITY_END_TURN_HEAL_TEN:
                AbilityHeal(effect, 10);
                break;
            case Card.CARD_ABILITY_END_TURN_HEAL_TWENTY:
                AbilityHeal(effect, 20);
                break;
            case Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN:
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN:
                AbilityAttackInFront(effect, 10);
                break;
            case Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY:
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY:
                AbilityAttackInFront(effect, 20);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN:
                AbilityDeathRattleAttackFace(effect, 10);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY:
                AbilityDeathRattleAttackRandomThree(effect);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY:
                AbilityDeathRattleDamageAllOpponentCreatures(effect, 20);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY:
                AbilityDeathRattleDamageAllCreatures(effect, 30);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD:
                AbilityDeathRattleDrawCard(effect);
                break;
            case EFFECT_CHANGE_TURN_DRAW_CARD:
            case EFFECT_DRAW_CARD:
            case Card.CARD_ABILITY_END_TURN_DRAW_CARD:
            case Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD:
            case Card.CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD:
                AbilityDrawCard(effect);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX:
                AbilityHealFriendlyMax(effect);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT:
                AbilitySilenceInFront(effect);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY:
                AbilityTauntAdjacentFriendly(effect);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY:
                AbilityDamagePlayerFace(effect, 20);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY:
                AbilityHealAdjacent(effect, 20);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY:
                AbilityHealAdjacent(effect, 40);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY:
                AbilityHealAllCreatures(effect, 40);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE:
                AbilityReviveHighestCostCreature(effect);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES:
                AbilitySilenceAllOpponentCreatures(effect);
                break;
            case Card.BUFF_CATEGORY_UNSTABLE_POWER:
                BuffUnstablePower(effect);
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect: {0}.", effect.Name));
                break;
        }
    }

    private void AbilityHeal(Effect effect, int amount)
    {
        BoardCreature boardCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        boardCreature.Heal(amount);
    }

    private void AbilityDrawCard(Effect effect)
    {
        Player player = BattleManager.Instance.GetPlayerById(
            effect.PlayerId
        );

        int rank = player.DrawCardDevice();

        this.isWaiting = true;
        StartCoroutine("WaitForDrawCard", new object[1] { rank });

        if (!DeveloperPanel.IsServerEnabled())
        {
            player.DrawCardsMock(1);
        }
    }

    private void AbilityAttackInFront(Effect effect, int amount)
    {
        BoardCreature attackingCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        List<Effect> effects = new List<Effect>();

        Transform defendingTransform = Board.Instance.GetInFrontBoardPlaceByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        BoardCreature defendingCreature = Board.Instance.GetInFrontCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        string effectName = "FanMeteorsVFX";   //TODO: use attackingCreature.CreatureCard.GetEffectPrefab();
        FXPoolManager.Instance.PlayEffectLookAt(effectName, attackingCreature.transform, defendingTransform);

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
        Player targetedPlayer = Board.Instance.GetOpponentByPlayerId(
            effect.PlayerId
        );

        targetedPlayer.TakeDamage(amount);

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
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        BoardCreature boardCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(playerId, cardId);

        List<Effect> effects = new List<Effect>();

        for (int i = 0; i < 3; i += 1)
        {
            Effect newEffect = new Effect(
                playerId,
                    EFFECT_RANDOM_TARGET,
                    effect.CardId,
                    effect.SpawnRank
            );
            effect.SetCard(boardCreature.GetChallengeCard());
            effects.Add(effect);
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

    private void AbilityDeathRattleDamageAllOpponentCreatures(Effect effect, int amount)
    {
        string playerId = effect.PlayerId;

        List<Effect> effects = new List<Effect>();

        List<BoardCreature> opponentCreatures = Board.Instance.GetOpponentAliveCreaturesByPlayerId(playerId);

        foreach (BoardCreature opponentCreature in opponentCreatures)
        {
            int damageTaken = opponentCreature.TakeDamage(amount);

            effects.AddRange(GetEffectsOnCreatureDamageTaken(opponentCreature, damageTaken));

            if (opponentCreature.Health <= 0)
            {
                effects.AddRange(GetEffectsOnCreatureDeath(opponentCreature));
            }
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

    private void AbilityDeathRattleDamageAllCreatures(Effect effect, int amount)
    {
        string playerId = effect.PlayerId;

        List<Effect> effects = new List<Effect>();

        List<BoardCreature> playerCreatures = Board.Instance.GetAliveCreaturesByPlayerId(playerId);
        List<BoardCreature> opponentCreatures = Board.Instance.GetOpponentAliveCreaturesByPlayerId(playerId);

        foreach (BoardCreature boardCreature in playerCreatures)
        {
            int damageTaken = boardCreature.TakeDamage(amount);

            effects.AddRange(GetEffectsOnCreatureDamageTaken(boardCreature, damageTaken));

            if (boardCreature.Health <= 0)
            {
                effects.AddRange(GetEffectsOnCreatureDeath(boardCreature));
            }
        }
        foreach (BoardCreature boardCreature in opponentCreatures)
        {
            int damageTaken = boardCreature.TakeDamage(amount);

            effects.AddRange(GetEffectsOnCreatureDamageTaken(boardCreature, damageTaken));

            if (boardCreature.Health <= 0)
            {
                effects.AddRange(GetEffectsOnCreatureDeath(boardCreature));
            }
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
        string playerId = effect.PlayerId;

        Player player = BattleManager.Instance.GetPlayerById(playerId);

        BoardCreature boardCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            playerId,
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

        this.isWaiting = true;
        AddToQueues(effects);

        int rank = player.DrawCardDevice();

        StartCoroutine("WaitForDrawCard", new object[1] { rank });

        if (!DeveloperPanel.IsServerEnabled())
        {
            boardCreature.Owner.DrawCardsMock(1);
        }
    }

    private IEnumerator WaitForDrawCard(object[] args)
    {
        int moveRank = (int)args[0];
        while (BattleManager.Instance.ProcessMoveQueue() != moveRank)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // this.isWaiting will be unset by OnDrawCardFinish().
    }

    private void AbilityHealFriendlyMax(Effect effect)
    {
        string playerId = effect.PlayerId;

        List<BoardCreature> boardCreatures = Board.Instance.GetAliveCreaturesByPlayerId(playerId);
        foreach (BoardCreature boardCreature in boardCreatures)
        {
            boardCreature.HealMax();
        }
    }

    private void AbilitySilenceInFront(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        BoardCreature attackingCreature = Board.Instance.GetCreatureByPlayerIdAndCardId(
            playerId,
            cardId
        );

        List<Effect> effects = new List<Effect>();
        // TODO: use pooling so don't instantiate?
        //string effectName = "VFX/FanMeteorsVFX"; //attackingCreature.CreatureCard.GetEffectPrefab();
        //GameObject effectVFX = Instantiate(
        //    ResourceSingleton.Instance.GetEffectPrefabByName(effectName),
        //    attackingCreature.transform.position,
        //    Quaternion.identity
        //);
        //effectVFX.transform.parent = attackingCreature.transform;
        //effectVFX.transform.Translate(Vector3.back * 1 + Vector3.up * 1, Space.Self);

        Transform defendingTransform = Board.Instance.GetInFrontBoardPlaceByPlayerIdAndCardId(
            playerId,
            cardId
        );
        //if (defendingTransform)
        //{
        //    effectVFX.transform.LookAt(defendingTransform);
        //}

        //if (effectVFX == null)
        //{
        //    Debug.LogError("Should render attack in front effect.");
        //}

        BoardCreature defendingCreature = Board.Instance.GetInFrontCreatureByPlayerIdAndCardId(
            playerId,
            cardId
        );

        if (defendingCreature != null)
        {
            defendingCreature.Silence();
        }

        AddToQueues(effects);
    }

    private void AbilityTauntAdjacentFriendly(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        List<BoardCreature> adjacentCreatures =
            Board.Instance.GetAdjacentCreaturesByPlayerIdAndCardId(
                playerId,
                cardId
            );

        foreach (BoardCreature adjacentCreature in adjacentCreatures)
        {
            adjacentCreature.GrantTaunt();
        }

        // No AddToQueues for now.
    }

    private void AbilityDamagePlayerFace(Effect effect, int amount)
    {
        string playerId = effect.PlayerId;

        Player player = BattleManager.Instance.GetPlayerById(playerId);
        player.TakeDamage(amount);

        // No AddToQueues for now.
    }

    private void AbilityHealAdjacent(Effect effect, int amount)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        List<BoardCreature> adjacentCreatures =
            Board.Instance.GetAdjacentCreaturesByPlayerIdAndCardId(
                playerId,
                cardId
            );

        foreach (BoardCreature adjacentCreature in adjacentCreatures)
        {
            adjacentCreature.Heal(amount);
        }

        List<Effect> effects = new List<Effect>();

        AddToQueues(effects);
    }

    private void AbilityHealAllCreatures(Effect effect, int amount)
    {
        string playerId = effect.PlayerId;

        List<Effect> effects = new List<Effect>();

        List<BoardCreature> playerCreatures = Board.Instance.GetAliveCreaturesByPlayerId(playerId);
        List<BoardCreature> opponentCreatures = Board.Instance.GetOpponentAliveCreaturesByPlayerId(playerId);

        foreach (BoardCreature boardCreature in playerCreatures)
        {
            boardCreature.Heal(amount);
        }
        foreach (BoardCreature boardCreature in opponentCreatures)
        {
            boardCreature.Heal(amount);
        }
    }

    private List<ChallengeCard> GetDeadCardsByPlayerId(string playerId)
    {
        return new List<ChallengeCard>(
            this.deadCards.Where(deadCard => deadCard.PlayerId == playerId)
        );
    }

    private ChallengeCard CleanCardForSummon(string playerId, ChallengeCard dirtyCard)
    {
        int spawnRank = BattleManager.Instance.GetNewSpawnRank();
        ChallengeCard spawnCard = JsonUtility.FromJson<ChallengeCard>(
            JsonUtility.ToJson(dirtyCard)
        );

        spawnCard.SetId(playerId + "-" + spawnRank);
        spawnCard.SetCost(spawnCard.CostStart);
        spawnCard.SetAttack(spawnCard.AttackStart);
        spawnCard.SetHealth(spawnCard.HealthStart);
        spawnCard.SetHealthMax(spawnCard.HealthStart);
        spawnCard.SetIsFrozen(0);
        spawnCard.SetIsSilenced(0);

        if (spawnCard.GetAbilities().Contains(Card.CARD_ABILITY_CHARGE))
        {
            spawnCard.SetCanAttack(1);
        }
        else
        {
            spawnCard.SetCanAttack(0);
        }

        return spawnCard;
    }

    private void AbilityReviveHighestCostCreature(Effect effect)
    {
        string playerId = effect.PlayerId;

        List<BoardCreature> aliveCreatures = Board.Instance.GetAliveCreaturesByPlayerId(playerId);
        List<ChallengeCard> sortedDeadCards = GetDeadCardsByPlayerId(playerId);

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(effect.PlayerId);

        if (sortedDeadCards.Count <= 0)
        {
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_NO_CREATURE);
        }
        else if (aliveCreatures.Count < 6)
        {
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE);
        }
        else
        {
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL);
        }

        challengeMove.SetRank(BattleManager.Instance.GetDeviceMoveRank());
        BattleManager.Instance.AddDeviceMove(challengeMove);

        this.isWaiting = true;
        StartCoroutine("WaitForSummonCreature", new object[1] { challengeMove.Rank });

        if (!DeveloperPanel.IsServerEnabled())
        {
            int highestCost = -1;
            ChallengeCard reviveCard = null;

            for (int i = sortedDeadCards.Count - 1; i >= 0; i -= 1)
            {
                ChallengeCard currentCard = sortedDeadCards[i];
                int currentCost = currentCard.CostStart;
                if (currentCost > highestCost)
                {
                    highestCost = currentCost;
                    reviveCard = currentCard;
                }
            }

            if (reviveCard == null)
            {
                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(effect.PlayerId);
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_NO_CREATURE);
                challengeMove.SetRank(BattleManager.Instance.GetServerMoveRank());

                BattleManager.Instance.ReceiveChallengeMove(challengeMove);
            }
            else
            {
                ChallengeCard spawnCard = CleanCardForSummon(playerId, reviveCard);

                int spawnIndex = Board.Instance.GetAvailableFieldIndexByPlayerId(playerId);

                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(effect.PlayerId);
                challengeMove.SetRank(BattleManager.Instance.GetServerMoveRank());

                ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
                moveAttributes.SetCard(spawnCard);
                moveAttributes.SetFieldId(playerId);

                if (spawnIndex < 0)
                {
                    challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL);
                }
                else
                {
                    challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE);
                    moveAttributes.SetFieldIndex(spawnIndex);
                }

                challengeMove.SetMoveAttributes(moveAttributes);

                BattleManager.Instance.ReceiveChallengeMove(challengeMove);
            }
        }
    }

    private IEnumerator WaitForSummonCreature(object[] args)
    {
        int moveRank = (int)args[0];
        while (BattleManager.Instance.ProcessMoveQueue() != moveRank)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void AbilitySilenceAllOpponentCreatures(Effect effect)
    {
        string playerId = effect.PlayerId;

        List<Effect> effects = new List<Effect>();

        List<BoardCreature> opponentCreatures = Board.Instance.GetOpponentAliveCreaturesByPlayerId(playerId);

        foreach (BoardCreature boardCreature in opponentCreatures)
        {
            boardCreature.Silence();
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

    public void OnDrawCardFinish()
    {
        this.isWaiting = false;
    }

    public void OnSummonCreatureFinish()
    {
        this.isWaiting = false;
    }

    public void OnStartTurn(string playerId)
    {
        Player player = BattleManager.Instance.GetPlayerById(playerId);

        player.Avatar.OnStartTurn();

        List<Effect> effects = new List<Effect>();

        // Draw a card for player to start its turn.
        effects.Add(
            new Effect(
                playerId,
                EFFECT_CHANGE_TURN_DRAW_CARD,
                null, // Does not matter.
                0 // Does not matter.
            )
        );

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

        List<BoardCreature> opponentCreatures = Board.Instance.GetOpponentAliveCreaturesByPlayerId(playerId);
        foreach (BoardCreature opponentCreature in opponentCreatures)
        {
            if (opponentCreature.HasAbility(Card.CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD))
            {
                effects.Add(
                    new Effect(
                        Board.Instance.GetOpponentIdByPlayerId(playerId),
                        Card.CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD,
                        opponentCreature.GetCardId(),
                        opponentCreature.SpawnRank
                    )
                );
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
                    int damageDone;

                    if (attackingCreature.HasAbility(Card.CARD_ABILITY_LETHAL))
                    {
                        damageDone = defendingCreature.TakeDamageWithLethal();
                    }
                    else
                    {
                        damageDone = defendingCreature.TakeDamage(attackingCreature.Attack);
                    }
                    effects.AddRange(GetEffectsOnCreatureDamageDealt(attackingCreature, damageDone));
                    effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

                    int damageTaken;

                    if (defendingCreature.HasAbility(Card.CARD_ABILITY_LETHAL))
                    {
                        damageTaken = attackingCreature.TakeDamageWithLethal();
                    }
                    else
                    {
                        damageTaken = attackingCreature.TakeDamage(defendingCreature.Attack);
                    }
                    effects.AddRange(GetEffectsOnCreatureDamageDealt(defendingCreature, damageTaken));
                    effects.AddRange(GetEffectsOnCreatureDamageTaken(attackingCreature, damageTaken));

                    if (attackingCreature.Health <= 0)
                    {
                        effects.AddRange(GetEffectsOnCreatureDeath(attackingCreature));
                    }
                    if (defendingCreature.Health <= 0)
                    {
                        effects.AddRange(GetEffectsOnCreatureDeath(defendingCreature));
                    }

                    int adjacentAttack = 0;
                    if (attackingCreature.HasAbility(Card.CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_TEN))
                    {
                        adjacentAttack = 10;
                    }
                    else if (attackingCreature.HasAbility(Card.CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_ATTACK))
                    {
                        adjacentAttack = attackingCreature.Attack;
                    }

                    if (adjacentAttack > 0)
                    {
                        List<BoardCreature> adjacentCreatures = Board.Instance.GetAdjacentCreaturesByPlayerIdAndCardId(
                            defendingCreature.Owner.Id,
                            defendingCreature.GetCardId()
                        );

                        foreach (BoardCreature adjacentCreature in adjacentCreatures)
                        {
                            damageDone = adjacentCreature.TakeDamage(adjacentAttack);
                            effects.AddRange(GetEffectsOnCreatureDamageDealt(attackingCreature, damageDone));
                            effects.AddRange(GetEffectsOnCreatureDamageTaken(adjacentCreature, damageDone));

                            if (adjacentCreature.Health <= 0)
                            {
                                effects.AddRange(GetEffectsOnCreatureDeath(adjacentCreature));
                            }
                        }
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

                    // Need to call redraw to update outline for can attack.
                    attackingCreature.Redraw();

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

    public void OnRandomTarget(
        string playerId,
        ChallengeCard card,
        string fieldId,
        string targetId
    )
    {
        if (card.Name == Card.CARD_NAME_BOMBSHELL_BOMBADIER)
        {
            Targetable attackingTargetable = Board.Instance.GetTargetableByPlayerIdAndCardId(playerId, card.Id);
            Targetable defendingTargetable = Board.Instance.GetTargetableByPlayerIdAndCardId(fieldId, targetId);

            if (attackingTargetable.GetType() == typeof(PlayerAvatar))
            {
                Debug.LogError("Player avatar cannot have death rattle.");
                return;
            }

            if (defendingTargetable.GetType() == typeof(BoardCreature))
            {
                BoardCreature defendingCreature = defendingTargetable as BoardCreature;

                List<Effect> effects = new List<Effect>();
                GameObject bombObject = FXPoolManager.Instance.PlayEffect("ExplosivePropVFX", attackingTargetable.transform.position);

                this.isWaiting = true;
                LeanTween.move(bombObject, defendingTargetable.transform.position, ActionManager.TWEEN_DURATION * 3)
                         .setEaseInOutCirc()
                         .setOnComplete(() =>
                         {
                             int damageDone = defendingCreature.TakeDamage(20);
                             effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

                             if (defendingCreature.Health <= 0)
                             {
                                 effects.AddRange(GetEffectsOnCreatureDeath(defendingCreature));
                             }

                             bombObject.SetActive(false);

                             AddToQueues(effects);
                             this.isWaiting = false;
                             this.isDirty = true;
                         });
            }
            else if (defendingTargetable.GetType() == typeof(PlayerAvatar))
            {
                PlayerAvatar defendingAvatar = defendingTargetable as PlayerAvatar;

                List<Effect> effects = new List<Effect>();
                GameObject bombObject = FXPoolManager.Instance.PlayEffect("ExplosivePropVFX", attackingTargetable.transform.position);

                this.isWaiting = true;
                LeanTween.move(bombObject, defendingTargetable.transform.position, ActionManager.TWEEN_DURATION * 3)
                         .setEaseInOutCirc()
                         .setOnComplete(() =>
                         {
                             int damageDone = defendingAvatar.TakeDamage(20);
                             //effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

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
                             bombObject.SetActive(false);

                             AddToQueues(effects);
                             this.isWaiting = false;
                             this.isDirty = true;
                         });
            }
            else
            {
                Debug.LogError("Unsupported.");
            }
        }
        else if (card.Name == SpellCard.SPELL_NAME_SPRAY_N_PRAY)
        {
            PlayerAvatar attackingTargetable = BattleManager.Instance.GetPlayerById(playerId).Avatar;
            Targetable defendingTargetable = Board.Instance.GetTargetableByPlayerIdAndCardId(fieldId, targetId);


            if (defendingTargetable.GetType() == typeof(BoardCreature))
            {
                BoardCreature defendingCreature = defendingTargetable as BoardCreature;

                List<Effect> effects = new List<Effect>();
                GameObject bombObject = FXPoolManager.Instance.PlayEffect("ExplosivePropVFX", attackingTargetable.transform.position);

                this.isWaiting = true;
                LeanTween.move(bombObject, defendingTargetable.transform.position, ActionManager.TWEEN_DURATION * 3)
                         .setEaseInOutCirc()
                         .setOnComplete(() =>
                         {
                             int damageDone = defendingCreature.TakeDamage(20);
                             effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

                             if (defendingCreature.Health <= 0)
                             {
                                 effects.AddRange(GetEffectsOnCreatureDeath(defendingCreature));
                             }

                             bombObject.SetActive(false);

                             AddToQueues(effects);
                             this.isWaiting = false;
                             this.isDirty = true;
                         });
            }
            else if (defendingTargetable.GetType() == typeof(PlayerAvatar))
            {
                PlayerAvatar defendingAvatar = defendingTargetable as PlayerAvatar;

                List<Effect> effects = new List<Effect>();
                GameObject bombObject = FXPoolManager.Instance.PlayEffect("ExplosivePropVFX", attackingTargetable.transform.position);

                this.isWaiting = true;
                LeanTween.move(bombObject, defendingTargetable.transform.position, ActionManager.TWEEN_DURATION * 3)
                         .setEaseInOutCirc()
                         .setOnComplete(() =>
                         {
                             int damageDone = defendingAvatar.TakeDamage(20);
                             //effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

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
                             bombObject.SetActive(false);

                             AddToQueues(effects);
                             this.isWaiting = false;
                             this.isDirty = true;
                         });
            }
            else
            {
                Debug.LogError("Unsupported.");
            }
        }
        else
        {
            Debug.LogError("Invalid card for move random target.");
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
            case SpellCard.SPELL_NAME_TOUCH_OF_ZEUS:
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
            case SpellCard.SPELL_NAME_DEATH_NOTICE:
                effects = SpellTargetedDeathNotice(playerId, targetedCreature);
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

        targetedCreature.Redraw();

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

    private List<Effect> SpellTargetedDeathNotice(string playerId, BoardCreature targetedCreature)
    {
        targetedCreature.DeathNotice();

        List<Effect> effects = new List<Effect>();

        if (targetedCreature.Health <= 0)
        {
            effects.AddRange(GetEffectsOnCreatureDeath(targetedCreature));
        }

        return effects;
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
            case SpellCard.SPELL_NAME_RAZE_TO_ASHES:
                effects = SpellUntargetedRazeToAshes(playerId);
                break;
            case SpellCard.SPELL_NAME_GREEDY_FINGERS:
                effects = SpellUntargetedGreedyFingers(playerId);
                break;
            case SpellCard.SPELL_NAME_SILENCE_OF_THE_LAMBS:
                effects = SpellUntargetedSilenceOfTheLambs(playerId);
                break;
            case SpellCard.SPELL_NAME_MUDSLINGING:
                effects = SpellUntargetedMudslinging(playerId);
                break;
            case SpellCard.SPELL_NAME_SPRAY_N_PRAY:
                effects = SpellUntargetedSprayNPray(playerId, spellCard.GetChallengeCard());
                break;
            case SpellCard.SPELL_NAME_GRAVE_DIGGING:
                effects = SpellUntargetedGraveDigging(playerId);
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

    private List<Effect> SpellUntargetedRazeToAshes(string playerId)
    {
        List<BoardCreature> opponentAliveCreatures =
            Board.Instance.GetOpponentAliveCreaturesByPlayerId(playerId);

        List<Effect> effects = new List<Effect>();

        foreach (BoardCreature targetedCreature in opponentAliveCreatures)
        {
            int damageTaken = targetedCreature.TakeDamage(50);
            effects.AddRange(GetEffectsOnCreatureDamageTaken(targetedCreature, damageTaken));

            if (targetedCreature.Health <= 0)
            {
                effects.AddRange(GetEffectsOnCreatureDeath(targetedCreature));
            }
        }

        return effects;
    }

    private List<Effect> SpellUntargetedGreedyFingers(string playerId)
    {
        List<Effect> effects = new List<Effect>();

        for (int i = 0; i < 3; i += 1)
        {
            effects.Add(
                new Effect(
                    playerId,
                    EFFECT_DRAW_CARD,
                    null, // Does not matter.
                    0 // Does not matter.
                )
            );
        }

        return effects;
    }

    private List<Effect> SpellUntargetedSilenceOfTheLambs(string playerId)
    {
        List<Effect> effects = new List<Effect>();

        List<BoardCreature> playerCreatures = Board.Instance.GetAliveCreaturesByPlayerId(playerId);
        List<BoardCreature> opponentCreatures = Board.Instance.GetOpponentAliveCreaturesByPlayerId(playerId);

        foreach (BoardCreature boardCreature in playerCreatures)
        {
            boardCreature.Silence();
        }
        foreach (BoardCreature boardCreature in opponentCreatures)
        {
            boardCreature.Silence();
        }

        return effects;
    }

    private List<Effect> SpellUntargetedMudslinging(string playerId)
    {
        List<Effect> effects = new List<Effect>();

        List<BoardCreature> playerCreatures = Board.Instance.GetAliveCreaturesByPlayerId(playerId);

        foreach (BoardCreature boardCreature in playerCreatures)
        {
            boardCreature.GrantTaunt();
        }

        return effects;
    }

    private List<Effect> SpellUntargetedSprayNPray(string playerId, ChallengeCard card)
    {
        List<Effect> effects = new List<Effect>();

        for (int i = 0; i < 3; i += 1)
        {
            Effect newEffect = new Effect(
                playerId,
                EFFECT_RANDOM_TARGET,
                card.Id, // Does not matter.
                i
            );
            newEffect.SetCard(card);
            effects.Add(newEffect);
        }

        return effects;
    }

    private List<Effect> SpellUntargetedGraveDigging(string playerId)
    {
        List<BoardCreature> aliveCreatures = Board.Instance.GetAliveCreaturesByPlayerId(playerId);
        List<ChallengeCard> sortedDeadCards = GetDeadCardsByPlayerId(playerId);

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(playerId);

        if (sortedDeadCards.Count <= 0)
        {
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_NO_CREATURE);
        }
        else if (aliveCreatures.Count < 6)
        {
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE);
        }
        else
        {
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL);
        }

        challengeMove.SetRank(BattleManager.Instance.GetDeviceMoveRank());
        BattleManager.Instance.AddDeviceMove(challengeMove);

        this.isWaiting = true;
        StartCoroutine("WaitForSummonCreature", new object[1] { challengeMove.Rank });

        if (!DeveloperPanel.IsServerEnabled())
        {
            if (sortedDeadCards.Count <= 0)
            {
                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(playerId);
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_NO_CREATURE);
                challengeMove.SetRank(BattleManager.Instance.GetServerMoveRank());

                BattleManager.Instance.ReceiveChallengeMove(challengeMove);
            }
            else
            {
                ChallengeCard reviveCard = sortedDeadCards.Last();

                ChallengeCard spawnCard = CleanCardForSummon(playerId, reviveCard);

                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(playerId);
                challengeMove.SetRank(BattleManager.Instance.GetServerMoveRank());

                int spawnIndex = Board.Instance.GetAvailableFieldIndexByPlayerId(playerId);

                ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
                moveAttributes.SetCard(spawnCard);
                moveAttributes.SetFieldId(playerId);

                if (spawnIndex < 0)
                {
                    challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL);
                }
                else
                {
                    challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE);
                    moveAttributes.SetFieldIndex(spawnIndex);
                }

                challengeMove.SetMoveAttributes(moveAttributes);

                BattleManager.Instance.ReceiveChallengeMove(challengeMove);
            }
        }

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
            Effect newEffect = new Effect(
                boardCreature.Owner.Id,
                Card.CARD_ABILITY_LIFE_STEAL,
                boardCreature.GetCardId(),
                boardCreature.SpawnRank
            );
            newEffect.SetValue(amount);
            effects.Add(newEffect);
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
        deadCards.Add(boardCreature.GetChallengeCard());

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

    public static bool IsWaiting()
    {
        if (Instance != null)
        {
            return Instance.isWaiting;
        }
        else
        {
            return false;
        }
    }

    public void SetDeadCards(List<ChallengeCard> deadCards)
    {
        Debug.Log(string.Format("Dead cards count: {0}", deadCards.Count));
        this.deadCards = deadCards;
    }
}
