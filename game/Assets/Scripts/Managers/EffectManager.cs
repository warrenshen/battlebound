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

    private bool isReady; // Ready => process queues.
    private int isWaiting; // Waiting > 0 => do not auto-process more server moves.
    private bool isChangingTurn;

    private UnityAction callback;

    private IFXManager fXManager;

    private BattleState battleState;
    public BattleState BattleState => battleState;

    public static EffectManager Instance { get; private set; }

    public const string EFFECT_CARD_DIE = "EFFECT_CARD_DIE";
    public const string EFFECT_CARD_DIE_AFTER_DEATH_RATTLE = "EFFECT_CARD_DIE_AFTER_DEATH_RATTLE";
    public const string EFFECT_STRUCTURE_DIE = "EFFECT_STRUCTURE_DIE";
    public const string EFFECT_PLAYER_AVATAR_DIE = "EFFECT_PLAYER_AVATAR_DIE";
    public const string EFFECT_RANDOM_TARGET = "EFFECT_RANDOM_TARGET";
    public const string EFFECT_CHANGE_TURN_DRAW_CARD = "EFFECT_CHANGE_TURN_DRAW_CARD";
    public const string EFFECT_DRAW_CARD = "EFFECT_DRAW_CARD";
    public const string EFFECT_SUMMON_CREATURE = "EFFECT_SUMMON_CREATURE";
    public const string EFFECT_GOLDENVALLEY_MINE = "EFFECT_GOLDENVALLEY_MINE";

    private static readonly List<string> EFFECT_H_PRIORITY_ORDER = new List<string>
    {
        Card.CARD_ABILITY_LIFE_STEAL,
        Card.CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY,
        Card.CARD_ABILITY_DAMAGE_TAKEN_ATTACK_FACE_BY_TWENTY,
        EFFECT_CARD_DIE,
        EFFECT_STRUCTURE_DIE,
        EFFECT_SUMMON_CREATURE,
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
        Card.CARD_ABILITY_END_TURN_BOOST_RANDOM_FRIENDLY_ZERO_TWENTY,
        Card.CARD_ABILITY_END_TURN_BOOST_RANDOM_FRIENDLY_TEN_TEN,
        Card.CARD_ABILITY_END_TURN_HEAL_FACE_THIRTY,

        // Battlecry.
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_THIRTY,
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
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_RANDOM_FROZEN_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_KILL_RANDOM_FROZEN,
        Card.CARD_ABILITY_BATTLE_CRY_BOOST_ADJACENT_THIRTY_THIRTY,
        Card.CARD_ABILITY_BATTLE_CRY_HEAL_FACE_TWENTY,

        // Deathrattle.
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TEN,
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN,
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY,
        Card.CARD_ABILITY_DEATH_RATTLE_RESUMMON,
        Card.CARD_ABILITY_DEATH_RATTLE_SUMMON_DUSK_DWELLERS,
        Card.CARD_ABILITY_DEATH_RATTLE_SUMMON_SUMMONED_DRAGONS,
        Card.CARD_ABILITY_DEATH_RATTLE_REVIVE_HIGHEST_COST_CREATURE,
        Card.CARD_ABILITY_DEATH_RATTLE_HEAL_FRIENDLY_MAX,
        Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,

        EFFECT_GOLDENVALLEY_MINE,

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
        Card.CARD_ABILITY_END_TURN_BOOST_RANDOM_FRIENDLY_ZERO_TWENTY,
        Card.CARD_ABILITY_END_TURN_BOOST_RANDOM_FRIENDLY_TEN_TEN,
        Card.CARD_ABILITY_END_TURN_HEAL_FACE_THIRTY,
    };

    private static readonly List<string> EFFECTS_START_TURN = new List<string>
    {
        Card.BUFF_CATEGORY_UNSTABLE_POWER,
    };

    private static readonly List<string> EFFECTS_BATTLE_CRY = new List<string>
    {
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_THIRTY,
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
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_RANDOM_FROZEN_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_KILL_RANDOM_FROZEN,
        Card.CARD_ABILITY_BATTLE_CRY_BOOST_ADJACENT_THIRTY_THIRTY,
        Card.CARD_ABILITY_BATTLE_CRY_HEAL_FACE_TWENTY,
    };

    private static readonly List<string> EFFECTS_DEATH_RATTLE = new List<string>
    {
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TEN,
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN,
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
        Card.CARD_ABILITY_DEATH_RATTLE_RESUMMON,
        Card.CARD_ABILITY_DEATH_RATTLE_SUMMON_DUSK_DWELLERS,
        Card.CARD_ABILITY_DEATH_RATTLE_SUMMON_SUMMONED_DRAGONS,
        Card.CARD_ABILITY_DEATH_RATTLE_REVIVE_HIGHEST_COST_CREATURE,
        Card.CARD_ABILITY_DEATH_RATTLE_HEAL_FRIENDLY_MAX,
    };

    private static readonly List<string> EFFECTS_DAMAGE_TAKEN = new List<string>
    {
        Card.CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY,
        Card.CARD_ABILITY_DAMAGE_TAKEN_ATTACK_FACE_BY_TWENTY,
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

        private int fieldIndex;
        public int FieldIndex => fieldIndex;

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
            this.value = 0;
            this.card = null;
            this.fieldIndex = -1;
        }

        public void SetValue(int value)
        {
            this.value = value;
        }

        public void SetCard(ChallengeCard card)
        {
            this.card = card;
        }

        public void SetFieldIndex(int fieldIndex)
        {
            this.fieldIndex = fieldIndex;
        }
    }

    public static bool IsWaiting()
    {
        if (Instance != null)
        {
            return Instance.isWaiting > 0 || !Instance.isReady || Instance.isChangingTurn;
        }
        else
        {
            return false;
        }
    }

    private void Awake()
    {
        Instance = this;

        this.isReady = false;
        this.isWaiting = 0;
        this.isChangingTurn = false;

        this.hQueue = new List<Effect>();
        this.mQueue = new List<Effect>();
        this.lQueue = new List<Effect>();
    }

    private void Start()
    {
        if (BattleSingleton.Instance.IsEnvironmentTest())
        {
            this.fXManager = new FXManagerMock();
        }
        else
        {
            this.fXManager = new FXManager();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.isReady || this.isWaiting > 0)
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
            else if (!this.isChangingTurn)
            {

                BattleState.Instance().ProcessMoveQueue();
            }
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

    public void ReadyUp()
    {
        this.isReady = true;
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

        switch (effect.Name)
        {
            case Card.CARD_ABILITY_LIFE_STEAL:
                AbilityLifeSteal(effect);
                break;
            case Card.CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY:
                AbilityDamageTakenDamagePlayerFace(effect, 30);
                break;
            case Card.CARD_ABILITY_DAMAGE_TAKEN_ATTACK_FACE_BY_TWENTY:
                AbilityDamageTakenAttackFace(effect, 20);
                break;
            case EFFECT_CARD_DIE:
                EffectCardDie(effect);
                break;
            case EFFECT_STRUCTURE_DIE:
                EffectStructureDie(effect);
                break;
            case EFFECT_SUMMON_CREATURE:
                EffectSummonCreature(effect);
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect: {0}.", effect.Name));
                break;
        }
    }

    private void EffectCardDie(Effect effect)
    {
        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );
        BattleState.Instance().AddDeadCard(boardCreature.GetChallengeCard());
        Board.Instance().RemoveCreature(boardCreature);
        boardCreature.Die();
    }

    private void EffectStructureDie(Effect effect)
    {
        BoardStructure boardStructure = Board.Instance().GetStructureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );
        BattleState.Instance().AddDeadCard(boardStructure.GetChallengeCard());
        Board.Instance().RemoveStructure(boardStructure);
        boardStructure.Die();
    }

    private void AbilityLifeSteal(Effect effect)
    {
        Player player = BattleState.Instance().GetPlayerById(effect.PlayerId);
        player.Heal(effect.Value);
    }

    private void AbilityDamageTakenDamagePlayerFace(Effect effect, int amount)
    {
        Player player = BattleState.Instance().GetPlayerById(effect.PlayerId);
        int damageTaken = player.TakeDamage(amount);
        AddToQueues(GetEffectsOnFaceDamageTaken(player, damageTaken));
    }

    private void AbilityDamageTakenAttackFace(Effect effect, int amount)
    {
        Player opponent = BattleState.Instance().GetOpponentByPlayerId(effect.PlayerId);
        int damageTaken = opponent.TakeDamage(amount);
        AddToQueues(GetEffectsOnFaceDamageTaken(opponent, damageTaken));
    }

    private void EffectSummonCreature(Effect effect)
    {
        string playerId = effect.PlayerId;
        int fieldIndex = effect.FieldIndex;
        ChallengeCard dirtyCard = effect.Card;

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(playerId);

        if (fieldIndex < 0)
        {
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL);
        }
        else
        {
            // Note that we only care about the actual non-zero value in local development mode.
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE);
        }

        WaitForServerMove(BattleState.Instance().AddDeviceMove(challengeMove));

        if (!FlagHelper.IsServerEnabled())
        {
            ChallengeCard spawnCard = CleanCardForSummon(playerId, dirtyCard);

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(effect.PlayerId);

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCard(spawnCard);
            moveAttributes.SetFieldId(playerId);

            if (fieldIndex < 0)
            {
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL);
            }
            else
            {
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE);
                moveAttributes.SetFieldIndex(fieldIndex);
            }

            challengeMove.SetMoveAttributes(moveAttributes);
            BattleState.Instance().AddServerMove(challengeMove);
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
        EffectCardDie(effect);
    }

    private void EffectRandomTarget(Effect effect)
    {
        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(effect.PlayerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET);
        WaitForServerMove(BattleState.Instance().AddDeviceMove(challengeMove));

        if (!FlagHelper.IsServerEnabled())
        {
            Targetable randomTargetable = Board.Instance().GetOpponentRandomTargetable(effect.PlayerId);

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(effect.PlayerId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET);

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCard(effect.Card);
            moveAttributes.SetFieldId(randomTargetable.GetPlayerId());
            moveAttributes.SetTargetId(randomTargetable.GetCardId());

            challengeMove.SetMoveAttributes(moveAttributes);
            BattleState.Instance().AddServerMove(challengeMove);
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
                EffectPlayerAvatarDie(effect);
                break;
            case Card.CARD_ABILITY_END_TURN_HEAL_TEN:
            case Card.CARD_ABILITY_END_TURN_HEAL_TWENTY:
            case Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN:
            case Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY:
            case Card.CARD_ABILITY_END_TURN_DRAW_CARD:
            case Card.CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD:
            case Card.CARD_ABILITY_END_TURN_BOOST_RANDOM_FRIENDLY_ZERO_TWENTY:
            case Card.CARD_ABILITY_END_TURN_BOOST_RANDOM_FRIENDLY_TEN_TEN:
            case Card.CARD_ABILITY_END_TURN_HEAL_FACE_THIRTY:
            case EFFECT_GOLDENVALLEY_MINE:
                AbilityEndTurn(effect);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN:
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY:
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_THIRTY:
            case Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD:
            case Card.CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX:
            case Card.CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT:
            case Card.CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY:
            case Card.CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY:
            case Card.CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY:
            case Card.CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY:
            case Card.CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY:
            case Card.CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE:
            case Card.CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES:
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_RANDOM_FROZEN_BY_TWENTY:
            case Card.CARD_ABILITY_BATTLE_CRY_KILL_RANDOM_FROZEN:
            case Card.CARD_ABILITY_BATTLE_CRY_BOOST_ADJACENT_THIRTY_THIRTY:
            case Card.CARD_ABILITY_BATTLE_CRY_HEAL_FACE_TWENTY:
                AbilityBattlecry(effect);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN:
            case Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY:
            case Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TEN:
            case Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY:
            case Card.CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY:
            case Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD:
            case Card.CARD_ABILITY_DEATH_RATTLE_RESUMMON:
            case Card.CARD_ABILITY_DEATH_RATTLE_SUMMON_DUSK_DWELLERS:
            case Card.CARD_ABILITY_DEATH_RATTLE_SUMMON_SUMMONED_DRAGONS:
            case Card.CARD_ABILITY_DEATH_RATTLE_REVIVE_HIGHEST_COST_CREATURE:
            case Card.CARD_ABILITY_DEATH_RATTLE_HEAL_FRIENDLY_MAX:
                AbilityDeathRattle(effect);
                break;
            case EFFECT_CHANGE_TURN_DRAW_CARD:
            case EFFECT_DRAW_CARD:
                AbilityDrawCard(effect);
                break;
            case Card.BUFF_CATEGORY_UNSTABLE_POWER:
                BuffUnstablePower(effect);
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect: {0}.", effect.Name));
                break;
        }
    }

    private void AbilityBattlecry(Effect effect)
    {
        IncrementIsWaiting();
        StartCoroutine("AbilityBattlecryCoroutine", effect);
    }

    private IEnumerator AbilityBattlecryCoroutine(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(playerId, cardId);
        yield return new WaitForSeconds(0.5f);

        boardCreature.Warcry();
        switch (effect.Name)
        {
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN:
                AbilityAttackInFront(effect, 10);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY:
                AbilityAttackInFront(effect, 20);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_THIRTY:
                AbilityAttackInFront(effect, 30);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD:
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
            case Card.CARD_ABILITY_BATTLE_CRY_ATTACK_RANDOM_FROZEN_BY_TWENTY:
            case Card.CARD_ABILITY_BATTLE_CRY_KILL_RANDOM_FROZEN:
                AbilityAttackKillRandomFrozen(effect);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_BOOST_ADJACENT_THIRTY_THIRTY:
                AbilityBoostAdjacentFriendly(effect);
                break;
            case Card.CARD_ABILITY_BATTLE_CRY_HEAL_FACE_TWENTY:
                AbilityHealFace(effect, 20);
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect: {0}.", effect.Name));
                break;
        }

        DecrementIsWaiting();
    }

    private void AbilityDeathRattle(Effect effect)
    {
        IncrementIsWaiting();
        StartCoroutine("AbilityDeathRattleCoroutine", effect);
    }

    private IEnumerator AbilityDeathRattleCoroutine(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(playerId, cardId);
        boardCreature.Deathwish();

        yield return new WaitForSeconds(0.5f);

        switch (effect.Name)
        {
            case Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN:
                AbilityDeathRattleAttackFace(effect, 10);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY:
                AbilityDeathRattleAttackFace(effect, 20);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TEN:
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
            case Card.CARD_ABILITY_DEATH_RATTLE_RESUMMON:
                AbilityDeathRattleResummon(effect);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_SUMMON_DUSK_DWELLERS:
                AbilityDeathRattleSummonDuskDwellers(effect);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_SUMMON_SUMMONED_DRAGONS:
                AbilityDeathRattleSummonSummonedDragon(effect);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_REVIVE_HIGHEST_COST_CREATURE:
                AbilityDeathRattleReviveHighestCostCreature(effect);
                break;
            case Card.CARD_ABILITY_DEATH_RATTLE_HEAL_FRIENDLY_MAX:
                AbilityHealFriendlyMax(effect);
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect: {0}.", effect.Name));
                break;
        }

        DecrementIsWaiting();
    }

    private void AbilityEndTurn(Effect effect)
    {
        IncrementIsWaiting();
        StartCoroutine("AbilityEndTurnCoroutine", effect);
    }

    private IEnumerator AbilityEndTurnCoroutine(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(playerId, cardId);

        yield return new WaitForSeconds(0.5f);

        switch (effect.Name)
        {
            case Card.CARD_ABILITY_END_TURN_HEAL_TEN:
                AbilityHeal(effect, 10);
                break;
            case Card.CARD_ABILITY_END_TURN_HEAL_TWENTY:
                AbilityHeal(effect, 20);
                break;
            case Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN:
                AbilityAttackInFront(effect, 10);
                break;
            case Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY:
                AbilityAttackInFront(effect, 20);
                break;
            case Card.CARD_ABILITY_END_TURN_BOOST_RANDOM_FRIENDLY_ZERO_TWENTY:
            case Card.CARD_ABILITY_END_TURN_BOOST_RANDOM_FRIENDLY_TEN_TEN:
                AbilityBoostRandomFriendly(effect);
                break;
            case Card.CARD_ABILITY_END_TURN_DRAW_CARD:
                AbilityDrawCard(effect);
                break;
            case Card.CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD:
                AbilityDrawCard(effect);
                break;
            case Card.CARD_ABILITY_END_TURN_HEAL_FACE_THIRTY:
                AbilityHealFace(effect, 30);
                break;
            case EFFECT_GOLDENVALLEY_MINE:
                EffectGoldenvalleyMine(effect);
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect: {0}.", effect.Name));
                break;
        }

        DecrementIsWaiting();
    }

    private void EffectPlayerAvatarDie(Effect effect)
    {
        string playerId = effect.PlayerId;
        Player player = BattleState.Instance().GetPlayerById(playerId);
        PlayerAvatar playerAvatar = player.Avatar;
        playerAvatar.Die();

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(BattleState.Instance().GetOpponentIdByPlayerId(playerId));
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CHALLENGE_OVER);
        BattleState.Instance().AddDeviceMove(challengeMove);

        if (!FlagHelper.IsServerEnabled())
        {
            BattleState.Instance().MockChallengeEnd(playerId);
        }
    }

    private void EffectGoldenvalleyMine(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        BoardStructure boardStructure = Board.Instance().GetStructureByPlayerIdAndCardId(
            playerId,
            cardId
        );
        List<BoardCreature> boardCreatures = Board.Instance().GetAliveCreaturesByPlayerId(
            playerId
        );

        if (boardCreatures.Count <= 0)
        {
            return;
        }

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(playerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGETS);
        WaitForServerMove(BattleState.Instance().AddDeviceMove(challengeMove));

        if (!FlagHelper.IsServerEnabled())
        {
            List<BoardCreature> randomCreatures = new List<BoardCreature>();

            for (int i = 0; i < 4; i += 1)
            {
                BoardCreature randomCreature;
                if (BattleSingleton.Instance.IsEnvironmentTest())
                {
                    randomCreature = boardCreatures[0];
                }
                else
                {
                    int randomIndex = UnityEngine.Random.Range(0, boardCreatures.Count);
                    randomCreature = boardCreatures[randomIndex];
                }
                randomCreatures.Add(randomCreature);
            }

            List<string> fieldIds = new List<string>(randomCreatures.Select(boardCreature => boardCreature.GetPlayerId()));
            List<string> cardIds = new List<string>(randomCreatures.Select(boardCreature => boardCreature.GetCardId()));

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(playerId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGETS);

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCard(boardStructure.GetChallengeCard());
            moveAttributes.SetFieldIds(fieldIds);
            moveAttributes.SetTargetIds(cardIds);

            challengeMove.SetMoveAttributes(moveAttributes);
            BattleState.Instance().AddServerMove(challengeMove);
        }
    }

    private void AbilityHeal(Effect effect, int amount)
    {
        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );
        boardCreature.Heal(amount);
    }

    private void AbilityHealFace(Effect effect, int amount)
    {
        Player player = BattleState.Instance().GetPlayerById(effect.PlayerId);
        player.Heal(amount);
    }

    private void AbilityDrawCard(Effect effect)
    {
        Player player = BattleState.Instance().GetPlayerById(
            effect.PlayerId
        );

        WaitForServerMove(player.DrawCardDevice());

        if (!FlagHelper.IsServerEnabled())
        {
            player.DrawCardMock();
        }
    }

    private void AbilityAttackInFront(Effect effect, int amount)
    {
        BoardCreature attackingCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        List<Effect> effects = new List<Effect>();

        Transform defendingTransform = Board.Instance().GetInFrontBoardPlaceByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        BoardCreature defendingCreature = Board.Instance().GetInFrontCreatureByPlayerIdAndCardId(
            effect.PlayerId,
            effect.CardId
        );

        this.fXManager.PlayEffectLookAt(
            "FanMeteorsVFX",
            attackingCreature.GetTargetableTransform(),
            defendingTransform
        );

        if (defendingCreature != null)
        {
            int damageDone = defendingCreature.TakeDamage(amount);
            effects.AddRange(GetEffectsOnCreatureDamageDealt(attackingCreature, damageDone));
            effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));
        }
        AddToQueues(effects);
    }

    private void AbilityBoostAdjacentFriendly(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        List<BoardCreature> adjacentCreatures =
            Board.Instance().GetAdjacentCreaturesByPlayerIdAndCardId(
                playerId,
                cardId
            );

        string buff;
        switch (effect.Name)
        {
            case Card.CARD_ABILITY_BATTLE_CRY_BOOST_ADJACENT_THIRTY_THIRTY:
                buff = Card.BUFF_CATEGORY_THIRTY_THIRTY;
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect for boost adjacent friendly: {0}", effect.Name));
                return;
        }

        foreach (BoardCreature adjacentCreature in adjacentCreatures)
        {
            adjacentCreature.AddBuff(buff);
        }
    }

    private void AbilityBoostRandomFriendly(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(playerId, cardId);
        List<BoardCreature> friendlyCreatures =
            Board.Instance().GetAliveCreaturesByPlayerIdExceptCardId(playerId, cardId);
        if (friendlyCreatures.Count <= 0)
        {
            return;
        }

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(playerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET);
        WaitForServerMove(BattleState.Instance().AddDeviceMove(challengeMove));

        if (!FlagHelper.IsServerEnabled())
        {
            BoardCreature randomCreature;

            if (BattleSingleton.Instance.IsEnvironmentTest())
            {
                randomCreature = friendlyCreatures[0];
            }
            else
            {
                int randomIndex = UnityEngine.Random.Range(0, friendlyCreatures.Count);
                randomCreature = friendlyCreatures[randomIndex];
            }

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(playerId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET);

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCard(boardCreature.GetChallengeCard());
            moveAttributes.SetFieldId(randomCreature.GetPlayerId());
            moveAttributes.SetTargetId(randomCreature.GetCardId());

            challengeMove.SetMoveAttributes(moveAttributes);
            BattleState.Instance().AddServerMove(challengeMove);
        }
    }

    private void AbilityDeathRattleAttackFace(Effect effect, int amount)
    {
        List<Effect> effects = new List<Effect>();

        Player targetedPlayer = BattleState.Instance().GetOpponentByPlayerId(
            effect.PlayerId
        );

        int damageTaken = targetedPlayer.TakeDamage(amount);
        effects.AddRange(GetEffectsOnFaceDamageTaken(targetedPlayer, damageTaken));

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

        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(playerId, cardId);

        List<Effect> effects = new List<Effect>();

        for (int i = 0; i < 3; i += 1)
        {
            Effect newEffect = new Effect(
                playerId,
                    EFFECT_RANDOM_TARGET,
                    effect.CardId,
                    effect.SpawnRank
            );
            newEffect.SetCard(boardCreature.GetChallengeCard());
            effects.Add(newEffect);
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

        List<BoardCreature> opponentCreatures = Board.Instance().GetOpponentAliveCreaturesByPlayerId(playerId);

        foreach (BoardCreature opponentCreature in opponentCreatures)
        {
            int damageTaken = opponentCreature.TakeDamage(amount);
            effects.AddRange(GetEffectsOnCreatureDamageTaken(opponentCreature, damageTaken));
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

        List<BoardCreature> playerCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);
        List<BoardCreature> opponentCreatures = Board.Instance().GetOpponentAliveCreaturesByPlayerId(playerId);

        foreach (BoardCreature boardCreature in playerCreatures)
        {
            int damageTaken = boardCreature.TakeDamage(amount);
            effects.AddRange(GetEffectsOnCreatureDamageTaken(boardCreature, damageTaken));
        }
        foreach (BoardCreature boardCreature in opponentCreatures)
        {
            int damageTaken = boardCreature.TakeDamage(amount);
            effects.AddRange(GetEffectsOnCreatureDamageTaken(boardCreature, damageTaken));
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
        Player player = BattleState.Instance().GetPlayerById(playerId);
        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(
            playerId,
            effect.CardId
        );

        List<Effect> effects = new List<Effect>
        {
            new Effect(
                effect.PlayerId,
                EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
                effect.CardId,
                effect.SpawnRank
            )
        };

        WaitForServerMove(player.DrawCardDevice());

        AddToQueues(effects);

        if (!FlagHelper.IsServerEnabled())
        {
            boardCreature.Owner.DrawCardMock();
        }
    }

    private void AbilityHealFriendlyMax(Effect effect)
    {
        string playerId = effect.PlayerId;

        List<BoardCreature> boardCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);
        foreach (BoardCreature boardCreature in boardCreatures)
        {
            boardCreature.HealMax();
        }
    }

    private void AbilitySilenceInFront(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        BoardCreature attackingCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(
            playerId,
            cardId
        );

        List<Effect> effects = new List<Effect>();

        Transform defendingTransform = Board.Instance().GetInFrontBoardPlaceByPlayerIdAndCardId(
            playerId,
            cardId
        );
        BoardCreature defendingCreature = Board.Instance().GetInFrontCreatureByPlayerIdAndCardId(
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
            Board.Instance().GetAdjacentCreaturesByPlayerIdAndCardId(
                playerId,
                cardId
            );
        foreach (BoardCreature adjacentCreature in adjacentCreatures)
        {
            adjacentCreature.GrantTaunt();
        }
    }

    private void AbilityDamagePlayerFace(Effect effect, int amount)
    {
        string playerId = effect.PlayerId;
        Player player = BattleState.Instance().GetPlayerById(playerId);
        int damageTaken = player.TakeDamage(amount);
        AddToQueues(GetEffectsOnFaceDamageTaken(player, damageTaken));
    }

    private void AbilityHealAdjacent(Effect effect, int amount)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        List<BoardCreature> adjacentCreatures =
            Board.Instance().GetAdjacentCreaturesByPlayerIdAndCardId(
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

        List<BoardCreature> playerCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);
        List<BoardCreature> opponentCreatures = Board.Instance().GetOpponentAliveCreaturesByPlayerId(playerId);

        foreach (BoardCreature boardCreature in playerCreatures)
        {
            boardCreature.Heal(amount);
        }
        foreach (BoardCreature boardCreature in opponentCreatures)
        {
            boardCreature.Heal(amount);
        }
    }

    private ChallengeCard CleanCardForSummon(string playerId, ChallengeCard dirtyCard)
    {
        Player player = BattleState.Instance().GetPlayerById(playerId);
        ChallengeCard spawnCard = JsonUtility.FromJson<ChallengeCard>(
            JsonUtility.ToJson(dirtyCard)
        );

        int spawnRank = BattleState.Instance().GetNewSpawnRank();
        spawnCard.SetSpawnRank(spawnRank);

        spawnCard.SetId(player.GetNewCardId());
        spawnCard.SetPlayerId(playerId);
        spawnCard.SetCost(spawnCard.CostStart);
        spawnCard.SetAttack(spawnCard.AttackStart);
        spawnCard.SetHealth(spawnCard.HealthStart);
        spawnCard.SetHealthMax(spawnCard.HealthStart);
        spawnCard.SetIsFrozen(0);
        spawnCard.SetIsSilenced(0);

        spawnCard.SetAbilities(spawnCard.GetAbilitiesStart());
        spawnCard.SetBuffsHand(new List<string>());
        spawnCard.SetBuffsField(new List<string>());

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

    private ChallengeCard CleanCardForConvert(string playerId, ChallengeCard dirtyCard)
    {
        Player player = BattleState.Instance().GetPlayerById(playerId);
        ChallengeCard spawnCard = JsonUtility.FromJson<ChallengeCard>(
            JsonUtility.ToJson(dirtyCard)
        );

        spawnCard.SetId(player.GetNewCardId());
        spawnCard.SetPlayerId(playerId);

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

    private List<Effect> GetEffectsForReviveHighestCostCreature(Effect effect)
    {
        string playerId = effect.PlayerId;

        List<BoardCreature> aliveCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);
        List<ChallengeCard> sortedDeadCards = BattleState.Instance().GetDeadCardsByPlayerId(playerId);

        if (sortedDeadCards.Count <= 0)
        {
            return new List<Effect>();
        }

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

        Effect summonEffect = new Effect(
            playerId,
            EFFECT_SUMMON_CREATURE,
            null,
            0
        );
        summonEffect.SetCard(sortedDeadCards.Last());
        summonEffect.SetFieldIndex(
            aliveCreatures.Count < 6 ?
            Board.Instance().GetAvailableFieldIndexByPlayerId(playerId) :
            -1
        );

        return new List<Effect> { summonEffect };
    }

    private void AbilityReviveHighestCostCreature(Effect effect)
    {
        AddToQueues(GetEffectsForReviveHighestCostCreature(effect));
    }

    private void AbilitySilenceAllOpponentCreatures(Effect effect)
    {
        string playerId = effect.PlayerId;

        List<Effect> effects = new List<Effect>();
        List<BoardCreature> opponentCreatures = Board.Instance().GetOpponentAliveCreaturesByPlayerId(playerId);

        string opponentId = BattleState.Instance().GetOpponentIdByPlayerId(playerId);
        Vector3 position = Board.Instance().GetFieldCenterByPlayerId(opponentId);
        FXPoolManager.Instance.PlayEffectLookAt("SiphonerVFX", position);

        foreach (BoardCreature boardCreature in opponentCreatures)
        {
            boardCreature.Silence();
        }
    }

    private void AbilityAttackKillRandomFrozen(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;

        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(playerId, cardId);

        List<BoardCreature> frozenCreatures = Board.Instance().GetOpponentFrozenCreaturesByPlayerId(playerId);
        if (frozenCreatures.Count <= 0)
        {
            return;
        }

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(playerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET);
        WaitForServerMove(BattleState.Instance().AddDeviceMove(challengeMove));

        if (!FlagHelper.IsServerEnabled())
        {
            BoardCreature randomCreature;

            if (BattleSingleton.Instance.IsEnvironmentTest())
            {
                randomCreature = frozenCreatures[0];
            }
            else
            {
                int randomIndex = UnityEngine.Random.Range(0, frozenCreatures.Count);
                randomCreature = frozenCreatures[randomIndex];
            }

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(playerId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET);

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCard(boardCreature.GetChallengeCard());
            moveAttributes.SetFieldId(randomCreature.GetPlayerId());
            moveAttributes.SetTargetId(randomCreature.GetCardId());

            challengeMove.SetMoveAttributes(moveAttributes);
            BattleState.Instance().AddServerMove(challengeMove);
        }
    }

    private void AbilityDeathRattleResummon(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;
        int fieldIndex = Board.Instance().GetIndexByPlayerIdAndCardId(playerId, cardId);
        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(playerId, cardId);

        List<Effect> effects = new List<Effect>
        {
            new Effect(
                playerId,
                EFFECT_CARD_DIE,
                cardId,
                effect.SpawnRank
            ),
        };

        Effect summonEffect = new Effect(
            playerId,
            EFFECT_SUMMON_CREATURE,
            null,
            effect.SpawnRank
        );
        summonEffect.SetCard(boardCreature.GetChallengeCard());
        summonEffect.SetFieldIndex(fieldIndex);
        effects.Add(summonEffect);

        AddToQueues(effects);
    }

    private void AbilityDeathRattleSummonDuskDwellers(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;
        int fieldIndex = Board.Instance().GetIndexByPlayerIdAndCardId(playerId, cardId);

        List<Effect> effects = new List<Effect>
        {
            new Effect(
                playerId,
                EFFECT_CARD_DIE,
                cardId,
                effect.SpawnRank
            ),
        };

        CreatureCard duskDweller = new CreatureCard("", Card.CARD_NAME_DUSK_DWELLER, 1);

        Effect summonEffect = new Effect(
            playerId,
            EFFECT_SUMMON_CREATURE,
            null,
            effect.SpawnRank
        );
        summonEffect.SetCard(duskDweller.GetChallengeCard(playerId));
        summonEffect.SetFieldIndex(fieldIndex);
        effects.Add(summonEffect);

        int closestIndex = Board.Instance().GetClosestAvailableIndexByPlayerId(playerId, fieldIndex);
        summonEffect = new Effect(
            playerId,
            EFFECT_SUMMON_CREATURE,
            null,
            effect.SpawnRank + 1
        );
        summonEffect.SetCard(duskDweller.GetChallengeCard(playerId));
        summonEffect.SetFieldIndex(closestIndex);
        effects.Add(summonEffect);

        AddToQueues(effects);
    }

    private void AbilityDeathRattleSummonSummonedDragon(Effect effect)
    {
        string playerId = effect.PlayerId;
        string cardId = effect.CardId;
        int fieldIndex = Board.Instance().GetIndexByPlayerIdAndCardId(playerId, cardId);

        List<Effect> effects = new List<Effect>
        {
            new Effect(
                playerId,
                EFFECT_CARD_DIE,
                cardId,
                effect.SpawnRank
            ),
        };

        CreatureCard summonedDragon = new CreatureCard("", Card.CARD_NAME_TALUSREAVER, 1);

        Effect summonEffect = new Effect(
            playerId,
            EFFECT_SUMMON_CREATURE,
            null,
            effect.SpawnRank
        );
        summonEffect.SetCard(summonedDragon.GetChallengeCard(playerId));
        summonEffect.SetFieldIndex(fieldIndex);
        effects.Add(summonEffect);

        int closestIndex = Board.Instance().GetClosestAvailableIndexByPlayerId(playerId, fieldIndex);
        summonEffect = new Effect(
            playerId,
            EFFECT_SUMMON_CREATURE,
            null,
            effect.SpawnRank + 1
        );
        summonEffect.SetCard(summonedDragon.GetChallengeCard(playerId));
        summonEffect.SetFieldIndex(closestIndex);
        effects.Add(summonEffect);

        AddToQueues(effects);
    }

    private void AbilityDeathRattleReviveHighestCostCreature(Effect effect)
    {
        List<Effect> effects = GetEffectsForReviveHighestCostCreature(effect);
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

    private void BuffUnstablePower(Effect effect)
    {
        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(
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
        DecrementIsWaiting();
        BattleState.Instance().SetIsLocked(false);
    }

    public void OnSummonCreatureFinish()
    {
        DecrementIsWaiting();
    }

    public void OnPlayMulligan(string playerId, List<int> deckCardIndices)
    {
        Player player = BattleState.Instance().GetPlayerById(playerId);
        player.PlayMulliganByIndices(deckCardIndices);
    }

    public void OnDrawMulliganStart()
    {
        IncrementIsWaiting();
    }

    public void OnDrawMulliganFinish(string playerId)
    {
        Player player = BattleState.Instance().GetPlayerById(playerId);
        Player enemy = BattleState.Instance().GetOpponentByPlayerId(playerId);
        if (
            player.Mode == Player.PLAYER_STATE_MODE_MULLIGAN_WAITING &&
            enemy.Mode == Player.PLAYER_STATE_MODE_MULLIGAN_WAITING
        )
        {
            StartCoroutine("MulliganFinishCoroutine");
        }
        else
        {
            DecrementIsWaiting();
        }
    }

    private IEnumerator MulliganFinishCoroutine()
    {
        yield return new WaitForSeconds(1);
        DecrementIsWaiting();
    }

    public void OnFinishMulligan()
    {
        IncrementIsWaiting();
        BattleState.Instance().EndMulligan();
    }

    public void OnFinishMulliganFinish()
    {
        DecrementIsWaiting();
    }

    public void OnStartTurn(string playerId)
    {
        Player player = BattleState.Instance().GetPlayerById(playerId);

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

        List<BoardCreature> boardCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);
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
        this.isChangingTurn = false;
    }

    public void OnEndTurn(string playerId, UnityAction callback)
    {
        this.callback = callback;
        if (this.callback != null)
        {
            this.isChangingTurn = true;
        }

        Player player = BattleState.Instance().GetPlayerById(playerId);

        List<Effect> effects = new List<Effect>();

        List<BoardCreature> boardCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);
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

        List<BoardCreature> opponentCreatures = Board.Instance().GetOpponentAliveCreaturesByPlayerId(playerId);
        foreach (BoardCreature opponentCreature in opponentCreatures)
        {
            if (opponentCreature.HasAbility(Card.CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD))
            {
                effects.Add(
                    new Effect(
                        opponentCreature.GetPlayerId(),
                        Card.CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD,
                        opponentCreature.GetCardId(),
                        opponentCreature.SpawnRank
                    )
                );
            }
        }

        List<BoardStructure> playerStructures = Board.Instance().GetAliveStructuresByPlayerId(playerId);
        foreach (BoardStructure boardStructure in playerStructures)
        {
            if (boardStructure.GetCardName() == Card.CARD_NAME_GOLDENVALLEY_MINE)
            {
                effects.Add(
                    new Effect(
                        boardStructure.GetPlayerId(),
                        EFFECT_GOLDENVALLEY_MINE,
                        boardStructure.GetCardId(),
                        boardStructure.SpawnRank
                    )
                );
            }
        }

        AddToQueues(effects);
    }

    public void OnCreaturePlay(string playerId, string cardId)
    {
        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(
            playerId,
            cardId
        );

        if (boardCreature == null)
        {
            Debug.LogError("On play called on board creature that does not exist.");
            return;
        }

        BoardStructure boardStructure = Board.Instance().GetStructureByCreature(boardCreature);
        if (boardStructure != null)
        {
            if (boardStructure.GetCardName() == Card.CARD_NAME_WARDENS_OUTPOST)
            {
                boardCreature.GrantTaunt();
            }
            else
            {
                Debug.LogError("Unsupported board structure on creature play.");
            }
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
        BattleState.Instance().SetIsLocked(false);
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
            OnCreatureAttackCreature(
                attackingTargetable as BoardCreature,
                defendingTargetable as BoardCreature
            );
        }
        else if (
            attackingTargetable.GetType() == typeof(BoardCreature) &&
            defendingTargetable.GetType() == typeof(PlayerAvatar)
        )
        {
            OnCreatureAttackPlayer(
                attackingTargetable as BoardCreature,
                defendingTargetable as PlayerAvatar
            );
        }
        else if (
            attackingTargetable.GetType() == typeof(BoardCreature) &&
            defendingTargetable.GetType() == typeof(BoardStructure)
        )
        {
            OnCreatureAttackStructure(
                attackingTargetable as BoardCreature,
                defendingTargetable as BoardStructure
            );
        }
        else
        {
            Debug.LogError("Unsupported.");
        }
    }

    private void OnCreatureAttackCreature(
        BoardCreature attackingCreature,
        BoardCreature defendingCreature
    )
    {
        Player attackingPlayer = BattleState.Instance().GetPlayerById(attackingCreature.GetPlayerId());
        Player defendingPlayer = BattleState.Instance().GetPlayerById(defendingCreature.GetPlayerId());

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

        IncrementIsWaiting();
        attackingCreature.FightAnimationWithCallback(
            defendingCreature,
            new UnityAction(() =>
            {
                int damageDone;

                if (attackingCreature.HasAbility(Card.CARD_ABILITY_LETHAL))
                {
                    damageDone = defendingCreature.TakeDamageWithLethal();
                }
                else
                {
                    damageDone = defendingCreature.TakeDamage(attackingCreature.GetAttack());
                }

                List<Effect> effects = new List<Effect>();

                effects.AddRange(GetEffectsOnCreatureDamageDealt(attackingCreature, damageDone));
                effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

                if (
                    attackingCreature.HasAbility(Card.CARD_ABILITY_ICY) &&
                    defendingCreature.Health > 0 &&
                    defendingCreature.IsFrozen <= 0
                )
                {
                    defendingCreature.Freeze(1);
                }

                if (
                    attackingCreature.HasAbility(Card.CARD_ABILITY_PIERCING) &&
                    defendingCreature.Health <= 0
                )
                {
                    int attackingDamagePierce = attackingCreature.GetAttack() - damageDone;
                    int attackingDamageDoneFace = defendingPlayer.TakeDamage(attackingDamagePierce);
                    effects.AddRange(GetEffectsOnFaceDamageTaken(defendingPlayer, attackingDamageDoneFace));
                }

                int damageTaken;

                if (defendingCreature.HasAbility(Card.CARD_ABILITY_LETHAL))
                {
                    damageTaken = attackingCreature.TakeDamageWithLethal();
                }
                else
                {
                    damageTaken = attackingCreature.TakeDamage(defendingCreature.GetAttack());
                }

                effects.AddRange(GetEffectsOnCreatureDamageDealt(defendingCreature, damageTaken));
                effects.AddRange(GetEffectsOnCreatureDamageTaken(attackingCreature, damageTaken));

                if (
                    defendingCreature.HasAbility(Card.CARD_ABILITY_ICY) &&
                    attackingCreature.Health > 0 &&
                    attackingCreature.IsFrozen <= 1
                )
                {
                    attackingCreature.Freeze(2);
                }

                if (
                    defendingCreature.HasAbility(Card.CARD_ABILITY_PIERCING) &&
                    attackingCreature.Health <= 0
                )
                {
                    int defendingDamagePierce = defendingCreature.GetAttack() - damageTaken;
                    int defendingDamageDoneFace = attackingPlayer.TakeDamage(defendingDamagePierce);
                    effects.AddRange(GetEffectsOnFaceDamageTaken(attackingPlayer, defendingDamageDoneFace));
                }

                int adjacentAttack = 0;
                if (attackingCreature.HasAbility(Card.CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_TEN))
                {
                    adjacentAttack = 10;
                }
                else if (attackingCreature.HasAbility(Card.CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_ATTACK))
                {
                    adjacentAttack = attackingCreature.GetAttack();
                }

                if (adjacentAttack > 0)
                {
                    List<BoardCreature> adjacentCreatures = Board.Instance().GetAdjacentCreaturesByPlayerIdAndCardId(
                    defendingCreature.GetPlayerId(),
                        defendingCreature.GetCardId()
                    );

                    foreach (BoardCreature adjacentCreature in adjacentCreatures)
                    {
                        damageDone = adjacentCreature.TakeDamage(adjacentAttack);
                        effects.AddRange(GetEffectsOnCreatureDamageDealt(attackingCreature, damageDone));
                        effects.AddRange(GetEffectsOnCreatureDamageTaken(adjacentCreature, damageDone));
                    }
                }

                AddToQueues(effects);
                DecrementIsWaiting();
            }
        ));
    }

    private void OnCreatureAttackPlayer(
        BoardCreature attackingCreature,
        PlayerAvatar defendingAvatar
    )
    {
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

        IncrementIsWaiting();
        attackingCreature.FightAnimationWithCallback(
            defendingAvatar,
            new UnityAction(() =>
            {
                List<Effect> effects = new List<Effect>();

                int damageDone = defendingAvatar.TakeDamage(attackingCreature.GetAttack());
                effects.AddRange(GetEffectsOnCreatureDamageDealt(attackingCreature, damageDone));
                effects.AddRange(GetEffectsOnFaceDamageTaken(defendingAvatar.GetPlayer(), damageDone));

                // Need to call redraw to update outline for can attack.
                attackingCreature.Redraw();

                AddToQueues(effects);
                DecrementIsWaiting();
            }
        ));
    }

    private void OnCreatureAttackStructure(
        BoardCreature attackingCreature,
        BoardStructure defendingStructure
    )
    {
        List<BoardCreature> boardCreatures = Board.Instance().GetCreaturesByStructure(defendingStructure);
        if (boardCreatures.Count > 0)
        {
            Debug.LogError("Creature cannot attack structure with creatures in front of it!");
            return;
        }

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

        IncrementIsWaiting();
        attackingCreature.FightAnimationWithCallback(
            defendingStructure,
            new UnityAction(() =>
            {
                int damageDone = defendingStructure.TakeDamage(attackingCreature.GetAttack());
                effects.AddRange(GetEffectsOnCreatureDamageDealt(attackingCreature, damageDone));
                effects.AddRange(GetEffectsOnStructureDamageTaken(defendingStructure, damageDone));

                // Need to call redraw to update outline for can attack.
                attackingCreature.Redraw();

                AddToQueues(effects);
                DecrementIsWaiting();
            }
        ));
    }

    public void OnStructurePlay(
        string playerId,
        string cardId
    )
    {
        BoardStructure boardStructure = Board.Instance().GetStructureByPlayerIdAndCardId(
            playerId,
            cardId
        );

        if (boardStructure == null)
        {
            Debug.LogError("On play called on board creature that does not exist.");
            return;
        }

        List<Effect> effects = new List<Effect>();

        List<BoardCreature> boardCreatures = Board.Instance().GetCreaturesByStructure(
            boardStructure
        );

        if (boardStructure.GetCardName() == Card.CARD_NAME_WARDENS_OUTPOST)
        {
            foreach (BoardCreature boardCreature in boardCreatures)
            {
                boardCreature.GrantTaunt();
            }
        }

        AddToQueues(effects);
        BattleState.Instance().SetIsLocked(false);
    }

    public void OnRandomTarget(
        string playerId,
        ChallengeCard challengeCard,
        string fieldId,
        string targetId
    )
    {
        string cardId = challengeCard.Id;

        if (fieldId == null || targetId == null)
        {
            Debug.LogError("Invalid parameters given to function.");
            return;
        }

        if (challengeCard.Name == Card.CARD_NAME_BOMBSHELL_BOMBADIER)
        {
            Targetable attackingTargetable = Board.Instance().GetTargetableByPlayerIdAndCardId(playerId, cardId);
            Targetable defendingTargetable = Board.Instance().GetTargetableByPlayerIdAndCardId(fieldId, targetId);

            if (attackingTargetable.GetType() == typeof(PlayerAvatar))
            {
                Debug.LogError("Player avatar cannot have death rattle.");
                return;
            }

            if (defendingTargetable.GetType() == typeof(BoardCreature))
            {
                BoardCreature defendingCreature = defendingTargetable as BoardCreature;

                IncrementIsWaiting();
                this.fXManager.ThrowEffectWithCallback(
                    "ExplosivePropVFX",
                    attackingTargetable.GetTargetableTransform(),
                    defendingTargetable.GetTargetableTransform(),
                    () =>
                    {
                        int damageDone = defendingCreature.TakeDamage(10);
                        List<Effect> effects = GetEffectsOnCreatureDamageTaken(
                            defendingCreature,
                            damageDone
                        );
                        AddToQueues(effects);

                        DecrementIsWaiting();
                    }
                );
            }
            else if (defendingTargetable.GetType() == typeof(PlayerAvatar))
            {
                PlayerAvatar defendingAvatar = defendingTargetable as PlayerAvatar;

                IncrementIsWaiting();
                this.fXManager.ThrowEffectWithCallback(
                    "ExplosivePropVFX",
                    attackingTargetable.GetTargetableTransform(),
                    defendingTargetable.GetTargetableTransform(),
                    () =>
                    {
                        int damageDone = defendingAvatar.TakeDamage(10);
                        List<Effect> effects = GetEffectsOnFaceDamageTaken(
                            defendingAvatar.GetPlayer(),
                            damageDone
                        );
                        AddToQueues(effects);

                        DecrementIsWaiting();
                    }
                );
            }
            else
            {
                Debug.LogError("Unsupported.");
            }
        }
        else if (challengeCard.Name == Card.CARD_NAME_BOMBS_AWAY)
        {
            PlayerAvatar attackingTargetable = BattleState.Instance().GetPlayerById(playerId).Avatar;
            Targetable defendingTargetable = Board.Instance().GetTargetableByPlayerIdAndCardId(fieldId, targetId);

            if (defendingTargetable.GetType() == typeof(BoardCreature))
            {
                BoardCreature defendingCreature = defendingTargetable as BoardCreature;

                IncrementIsWaiting();
                this.fXManager.ThrowEffectWithCallback(
                    "ExplosivePropVFX",
                    attackingTargetable.GetTargetableTransform(),
                    defendingTargetable.GetTargetableTransform(),
                    () =>
                    {
                        int damageDone = defendingCreature.TakeDamage(10);
                        List<Effect> effects = GetEffectsOnCreatureDamageTaken(
                            defendingCreature,
                            damageDone
                        );
                        AddToQueues(effects);

                        DecrementIsWaiting();
                    }
                );
            }
            else if (defendingTargetable.GetType() == typeof(PlayerAvatar))
            {
                PlayerAvatar defendingAvatar = defendingTargetable as PlayerAvatar;

                IncrementIsWaiting();
                this.fXManager.ThrowEffectWithCallback(
                    "ExplosivePropVFX",
                    attackingTargetable.GetTargetableTransform(),
                    defendingTargetable.GetTargetableTransform(),
                    () =>
                    {
                        int damageDone = defendingAvatar.TakeDamage(10);
                        List<Effect> effects = GetEffectsOnFaceDamageTaken(
                            defendingAvatar.GetPlayer(),
                            damageDone
                        );
                        AddToQueues(effects);

                        DecrementIsWaiting();
                    }
                );
            }
            else
            {
                Debug.LogError("Unsupported.");
            }
        }
        else if (challengeCard.Name == Card.CARD_NAME_BATTLE_ROYALE)
        {
            List<BoardCreature> playerCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);
            List<BoardCreature> opponentCreatures = Board.Instance().GetOpponentAliveCreaturesByPlayerId(playerId);

            List<BoardCreature> allCreatures = new List<BoardCreature>();
            allCreatures.AddRange(playerCreatures);
            allCreatures.AddRange(opponentCreatures);

            List<Effect> effects = new List<Effect>();

            foreach (BoardCreature boardCreature in allCreatures)
            {
                if (boardCreature.GetPlayerId() == fieldId && boardCreature.GetCardId() == targetId)
                {
                    continue;
                }
                else
                {
                    boardCreature.DeathNote();
                    effects.AddRange(GetEffectsOnCreatureDeath(boardCreature));
                }
            }

            AddToQueues(effects);
        }
        else if (challengeCard.Name == Card.CARD_NAME_BLUE_GIPSY_V3)
        {
            BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(playerId, cardId);
            BoardCreature targetedCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(fieldId, targetId);

            IncrementIsWaiting();
            this.fXManager.ThrowEffectWithCallback(
                "ExplosivePropVFX",
                boardCreature.GetTargetableTransform(),
                targetedCreature.GetTargetableTransform(),
                () =>
                {
                    int damageTaken = targetedCreature.TakeDamage(20);
                    AddToQueues(GetEffectsOnCreatureDamageTaken(targetedCreature, damageTaken));
                    DecrementIsWaiting();
                }
            );
        }
        else if (challengeCard.Name == Card.CARD_NAME_FROSTLAND_THRASHER_8)
        {
            BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(playerId, cardId);
            BoardCreature targetedCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(fieldId, targetId);

            IncrementIsWaiting();
            this.fXManager.ThrowEffectWithCallback(
                "ExplosivePropVFX",
                boardCreature.GetTargetableTransform(),
                targetedCreature.GetTargetableTransform(),
                () =>
                {
                    targetedCreature.DeathNote();
                    AddToQueues(GetEffectsOnCreatureDeath(targetedCreature));
                    DecrementIsWaiting();
                }
            );
        }
        else if (challengeCard.Name == Card.CARD_NAME_PAL_V1)
        {
            BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(playerId, cardId);
            BoardCreature targetedCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(fieldId, targetId);

            IncrementIsWaiting();
            this.fXManager.ThrowEffectWithCallback(
                "LightbulbPropVFX",
                boardCreature.GetTargetableTransform(),
                targetedCreature.GetTargetableTransform(),
                () =>
                {
                    targetedCreature.AddBuff(Card.BUFF_CATEGORY_ZERO_TWENTY);
                    DecrementIsWaiting();
                }
            );
        }
        else if (challengeCard.Name == Card.CARD_NAME_FIRESMITH_APPRENTICE)
        {
            BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(playerId, cardId);
            BoardCreature targetedCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(fieldId, targetId);

            IncrementIsWaiting();
            this.fXManager.ThrowEffectWithCallback(
                "BlacksmithPropVFX",
                boardCreature.GetTargetableTransform(),
                targetedCreature.GetTargetableTransform(),
                () =>
                {
                    targetedCreature.AddBuff(Card.BUFF_CATEGORY_TEN_TEN);
                    DecrementIsWaiting();
                }
            );
        }
        else
        {
            Debug.LogError("Invalid card for move random target.");
        }

        DecrementIsWaiting();
    }

    public void OnRandomTargets(
        string playerId,
        ChallengeCard challengeCard,
        List<string> fieldIds,
        List<string> targetIds
    )
    {
        string cardId = challengeCard.Id;

        if (fieldIds == null || targetIds == null)
        {
            Debug.LogError("Invalid parameters given to function.");
            return;
        }

        if (challengeCard.Name == Card.CARD_NAME_GOLDENVALLEY_MINE)
        {
            BoardStructure boardStructure = Board.Instance().GetStructureByPlayerIdAndCardId(
                challengeCard.PlayerId,
                challengeCard.Id
            );
            List<BoardCreature> boardCreatures = Board.Instance().GetCreaturesByPlayerIdsAndCardIds(
                fieldIds,
                targetIds
            );

            for (int i = 0; i < boardCreatures.Count; i += 1)
            {
                BoardCreature boardCreature = boardCreatures[i];
                IncrementIsWaiting();
                this.fXManager.ThrowEffectWithCallback(
                    "ExplosivePropVFX",
                    boardStructure.GetTargetableTransform(),
                    boardCreature.GetTargetableTransform(),
                    () =>
                    {
                        boardCreature.AddBuff(Card.BUFF_CATEGORY_TEN_TEN);
                        DecrementIsWaiting();
                    },
                    0.75f * i
                );
            }
        }
        else
        {
            Debug.LogError("Invalid card for move random targets.");
        }

        DecrementIsWaiting();
    }

    /*
     * Player is the one converting target to their side.
     */
    public void OnCreatureConvert(string playerId, string fieldId, string targetId)
    {
        BoardCreature boardCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(
            fieldId,
            targetId
        );
        Board.Instance().RemoveCreature(boardCreature);
        boardCreature.Die();

        List<BoardCreature> playerAliveCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(playerId);

        if (playerAliveCreatures.Count < 6)
        {
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE);
        }
        else
        {
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL);
        }

        WaitForServerMove(BattleState.Instance().AddDeviceMove(challengeMove));

        if (!FlagHelper.IsServerEnabled())
        {
            ChallengeCard spawnCard = CleanCardForConvert(playerId, boardCreature.GetChallengeCard());

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(playerId);

            int spawnIndex = Board.Instance().GetAvailableFieldIndexByPlayerId(playerId);

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

            BattleState.Instance().AddServerMove(challengeMove);
        }

        DecrementIsWaiting();
    }

    public void OnSpellTargetedPlay(
        ChallengeCard challengeCard,
        Targetable targetedCreature
    )
    {
        string playerId = challengeCard.PlayerId;
        switch (challengeCard.Name)
        {
            case Card.CARD_NAME_TOUCH_OF_ZEUS:
                SpellTargetedTouchOfZeus(playerId, targetedCreature);
                break;
            case Card.CARD_NAME_UNSTABLE_POWER:
                SpellTargetedUnstablePower(playerId, targetedCreature as BoardCreature);
                break;
            case Card.CARD_NAME_DEEP_FREEZE:
                SpellTargetedDeepFreeze(playerId, targetedCreature as BoardCreature);
                break;
            case Card.CARD_NAME_WIDESPREAD_FROSTBITE:
                SpellTargetedWidespreadFrostbite(playerId, targetedCreature as BoardCreature);
                break;
            case Card.CARD_NAME_DEATH_NOTE:
                SpellTargetedDeathNote(playerId, targetedCreature as BoardCreature);
                break;
            case Card.CARD_NAME_BESTOWED_VIGOR:
                SpellTargetedBestowedVigor(playerId, targetedCreature as BoardCreature);
                break;
            default:
                Debug.LogError(string.Format("Invalid targeted spell with name: {0}.", challengeCard.Name));
                break;
        }
        BattleState.Instance().SetIsLocked(false);
    }

    private void SpellTargetedTouchOfZeus(string playerId, Targetable targetable)
    {
        IncrementIsWaiting();
        this.fXManager.PlayEffectWithCallback(
            "LightningShotVFX",
            "ShockSFX",
            targetable.GetTargetableTransform(),
            () =>
            {
                if (targetable.GetType() == typeof(BoardCreature))
                {
                    BoardCreature targetedCreature = targetable as BoardCreature;
                    int damageTaken = targetedCreature.TakeDamage(30);
                    AddToQueues(GetEffectsOnCreatureDamageTaken(targetedCreature, damageTaken));
                    DecrementIsWaiting();
                }
                else if (targetable.GetType() == typeof(BoardStructure))
                {
                    BoardStructure targetedStructure = targetable as BoardStructure;
                    int damageTaken = targetedStructure.TakeDamage(30);
                    AddToQueues(GetEffectsOnStructureDamageTaken(targetedStructure, damageTaken));
                    DecrementIsWaiting();
                }
                else
                {
                    Debug.LogError("Unsupported.");
                    DecrementIsWaiting();
                }
            }
        );
    }

    private void SpellTargetedUnstablePower(string playerId, BoardCreature targetedCreature)
    {
        targetedCreature.AddBuff(Card.BUFF_CATEGORY_UNSTABLE_POWER);
        // No triggered effects on unstable power for now.
    }

    private void SpellTargetedDeepFreeze(string playerId, BoardCreature targetedCreature)
    {
        IncrementIsWaiting();
        this.fXManager.PlayEffectWithCallback(
            "DeepFreezeVFX",
            "IceCastSFX",
            targetedCreature.GetTargetableTransform(),
            () =>
            {
                int damageTaken = targetedCreature.TakeDamage(10);
                if (targetedCreature.Health > 0)
                {
                    targetedCreature.Freeze(1);
                }
                AddToQueues(GetEffectsOnCreatureDamageTaken(targetedCreature, damageTaken));
                DecrementIsWaiting();
            }
        );
    }

    private void SpellTargetedWidespreadFrostbite(string playerId, BoardCreature targetedCreature)
    {
        IncrementIsWaiting();
        this.fXManager.PlayEffectWithCallback(
            "DeepFreezeVFX",
            "IceCastSFX",
            targetedCreature.GetTargetableTransform(),
            () =>
            {
                targetedCreature.Freeze(2);
                DecrementIsWaiting();
            }
        );

        List<BoardCreature> adjacentCreatures =
            Board.Instance().GetAdjacentCreaturesByPlayerIdAndCardId(
                targetedCreature.GetPlayerId(),
                targetedCreature.GetCardId()
            );

        foreach (BoardCreature adjacentCreature in adjacentCreatures)
        {
            IncrementIsWaiting();
            this.fXManager.PlayEffectWithCallback(
                "DeepFreezeVFX",
                "IceCastSFX",
                adjacentCreature.GetTargetableTransform(),
                () =>
                {
                    adjacentCreature.Freeze(1);
                    DecrementIsWaiting();
                }
            );
        }
        // No triggered effects on freeze for now.
    }

    private void SpellTargetedDeathNote(string playerId, BoardCreature targetedCreature)
    {
        IncrementIsWaiting();
        this.fXManager.PlayEffectsWithCallback(
            new List<string> { "DeathNoteVFX" },
            new List<string> { "StabSFX", "SlashSFX" },
            targetedCreature.GetTargetableTransform(),
            () =>
            {
                targetedCreature.DeathNote();
                AddToQueues(GetEffectsOnCreatureDeath(targetedCreature));
                DecrementIsWaiting();
            }
        );
    }

    private void SpellTargetedBestowedVigor(string playerId, BoardCreature targetedCreature)
    {
        targetedCreature.AddBuff(Card.BUFF_CATEGORY_BESTOWED_VIGOR);
        targetedCreature.Heal(10);
    }

    public void OnSpellUntargetedPlay(ChallengeCard challengeCard)
    {
        string playerId = challengeCard.PlayerId;
        List<Effect> effects = new List<Effect>();

        switch (challengeCard.Name)
        {
            case Card.CARD_NAME_SHIELDS_UP:
                effects = SpellUntargetedRiotUp(playerId);
                break;
            case Card.CARD_NAME_BRR_BRR_BLIZZARD:
                effects = SpellUntargetedBrrBrrBlizzard(playerId);
                break;
            case Card.CARD_NAME_RAZE_TO_ASHES:
                effects = SpellUntargetedRazeToAshes(playerId);
                break;
            case Card.CARD_NAME_GREEDY_FINGERS:
                effects = SpellUntargetedGreedyFingers(playerId);
                break;
            case Card.CARD_NAME_SILENCE_OF_THE_LAMBS:
                effects = SpellUntargetedSilenceOfTheLambs(playerId);
                break;
            case Card.CARD_NAME_RALLY_TO_THE_QUEEN:
                effects = SpellUntargetedMudslinging(playerId);
                break;
            case Card.CARD_NAME_BOMBS_AWAY:
                effects = SpellUntargetedBombsAway(playerId, challengeCard);
                break;
            case Card.CARD_NAME_GRAVE_DIGGING:
                effects = SpellUntargetedGraveDigging(playerId);
                break;
            case Card.CARD_NAME_THE_SEVEN:
                effects = SpellUntargetedTheSeven(playerId);
                break;
            case Card.CARD_NAME_BATTLE_ROYALE:
                effects = SpellUntargetedBattleRoyale(playerId, challengeCard);
                break;
            default:
                Debug.LogError(string.Format("Invalid untargeted spell with name: {0}.", challengeCard.Name));
                break;
        }

        AddToQueues(effects);
        BattleState.Instance().SetIsLocked(false);
    }

    private List<Effect> SpellUntargetedRiotUp(string playerId)
    {
        List<BoardCreature> boardCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);
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
            Board.Instance().GetOpponentAliveCreaturesByPlayerId(playerId);

        SoundManager.Instance.PlaySound("BlizzardSFX", this.transform.position);

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
            Board.Instance().GetOpponentAliveCreaturesByPlayerId(playerId);

        List<Effect> effects = new List<Effect>();

        string opponentId = BattleState.Instance().GetOpponentIdByPlayerId(playerId);
        Vector3 position = Board.Instance().GetFieldCenterByPlayerId(opponentId);
        GameObject vfx = FXPoolManager.Instance.PlayEffect("RazeToAshesVFX", position);
        SoundManager.Instance.PlaySound("FireSpellSFX", position);

        foreach (BoardCreature targetedCreature in opponentAliveCreatures)
        {
            int damageTaken = targetedCreature.TakeDamage(50);
            effects.AddRange(GetEffectsOnCreatureDamageTaken(targetedCreature, damageTaken));
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

        List<BoardCreature> playerCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);
        List<BoardCreature> opponentCreatures = Board.Instance().GetOpponentAliveCreaturesByPlayerId(playerId);

        FXPoolManager.Instance.PlayEffect("SilenceOfLambsVFX");

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

        List<BoardCreature> playerCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);

        foreach (BoardCreature boardCreature in playerCreatures)
        {
            boardCreature.GrantTaunt();
        }

        return effects;
    }

    private List<Effect> SpellUntargetedBombsAway(string playerId, ChallengeCard card)
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
        List<BoardCreature> aliveCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);
        List<ChallengeCard> sortedDeadCards = BattleState.Instance().GetDeadCardsByPlayerId(playerId);

        List<Effect> effects = new List<Effect>();

        if (sortedDeadCards.Count <= 0)
        {
            return effects;
        }

        ChallengeCard reviveCard = sortedDeadCards.Last();

        Effect summonEffect = new Effect(
            playerId,
            EFFECT_SUMMON_CREATURE,
            null,
            0
        );
        summonEffect.SetCard(sortedDeadCards.Last());
        summonEffect.SetFieldIndex(
            aliveCreatures.Count < 6 ?
            Board.Instance().GetAvailableFieldIndexByPlayerId(playerId) :
            -1
        );
        effects.Add(summonEffect);

        return effects;
    }

    private List<Effect> SpellUntargetedTheSeven(string playerId)
    {
        List<BoardCreature> opponentAliveCreatures = Board.Instance().GetOpponentAliveCreaturesByPlayerId(playerId);

        if (opponentAliveCreatures.Count <= 0)
        {
            return new List<Effect>();
        }

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(playerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CONVERT_CREATURE);
        WaitForServerMove(BattleState.Instance().AddDeviceMove(challengeMove));

        if (!FlagHelper.IsServerEnabled())
        {
            BoardCreature opponentRandomCreature = Board.Instance().GetOpponentRandomCreature(playerId);

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(playerId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CONVERT_CREATURE);

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetFieldId(opponentRandomCreature.GetPlayerId());
            moveAttributes.SetTargetId(opponentRandomCreature.GetCardId());

            challengeMove.SetMoveAttributes(moveAttributes);
            BattleState.Instance().AddServerMove(challengeMove);
        }

        return new List<Effect>();
    }

    private List<Effect> SpellUntargetedBattleRoyale(string playerId, ChallengeCard challengeCard)
    {
        List<BoardCreature> playerCreatures = Board.Instance().GetAliveCreaturesByPlayerId(playerId);
        List<BoardCreature> opponentCreatures = Board.Instance().GetOpponentAliveCreaturesByPlayerId(playerId);

        List<BoardCreature> allCreatures = new List<BoardCreature>();
        allCreatures.AddRange(playerCreatures);
        allCreatures.AddRange(opponentCreatures);

        if (allCreatures.Count <= 0)
        {
            return new List<Effect>();
        }

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(playerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET);
        WaitForServerMove(BattleState.Instance().AddDeviceMove(challengeMove));

        if (!FlagHelper.IsServerEnabled())
        {
            BoardCreature randomCreature;

            if (BattleSingleton.Instance.IsEnvironmentTest())
            {
                randomCreature = allCreatures[0];
            }
            else
            {
                int randomIndex = UnityEngine.Random.Range(0, opponentCreatures.Count);
                randomCreature = allCreatures[randomIndex];
            }

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(playerId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET);

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetCard(challengeCard);
            moveAttributes.SetFieldId(randomCreature.GetPlayerId());
            moveAttributes.SetTargetId(randomCreature.GetCardId());

            challengeMove.SetMoveAttributes(moveAttributes);
            BattleState.Instance().AddServerMove(challengeMove);
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
                boardCreature.GetPlayerId(),
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

        if (amount > 0)
        {
            foreach (string effectName in EFFECTS_DAMAGE_TAKEN)
            {
                if (boardCreature.HasAbility(effectName) || boardCreature.HasBuff(effectName))
                {
                    effects.Add(
                        new Effect(
                            boardCreature.GetPlayerId(),
                            effectName,
                            boardCreature.GetCardId(),
                            boardCreature.SpawnRank
                        )
                    );
                }
            }

            if (boardCreature.Health <= 0)
            {
                effects.AddRange(GetEffectsOnCreatureDeath(boardCreature));
            }
        }

        return effects;
    }

    private List<Effect> GetEffectsOnCreatureDeath(BoardCreature boardCreature)
    {
        List<Effect> effects = new List<Effect>();

        foreach (string effectName in EFFECTS_DEATH_RATTLE)
        {
            if (boardCreature.HasAbility(effectName) || boardCreature.HasBuff(effectName))
            {
                effects.Add(
                    new Effect(
                        boardCreature.GetPlayerId(),
                        effectName,
                        boardCreature.GetCardId(),
                        boardCreature.SpawnRank
                    )
                );
            }
        }

        // If no death rattle effects, add in card die effect.
        // If there are death rattle effects, they will handle card die.
        if (effects.Count <= 0)
        {
            effects.Add(
                new Effect(
                    boardCreature.GetPlayerId(),
                    EFFECT_CARD_DIE,
                    boardCreature.GetCardId(),
                    boardCreature.SpawnRank
                )
            );
        }

        return effects;
    }

    private List<Effect> GetEffectsOnFaceDamageTaken(
        Player player,
        int amount
    )
    {
        string playerId = player.Id;

        List<Effect> effects = new List<Effect>();

        if (player.GetHealth() <= 0)
        {
            effects.Add(
                new Effect(
                    playerId,
                    EFFECT_PLAYER_AVATAR_DIE,
                    player.GetCardId(),
                    0
                )
            );
        }

        return effects;
    }

    private List<Effect> GetEffectsOnStructureDamageTaken(
        BoardStructure boardStructure,
        int amount
    )
    {
        List<Effect> effects = new List<Effect>();

        if (boardStructure.Health > 0)
        {
            return effects;
        }

        List<BoardCreature> boardCreatures = Board.Instance().GetCreaturesByStructure(boardStructure);

        if (boardStructure.GetCardName() == Card.CARD_NAME_WARDENS_OUTPOST)
        {
            foreach (BoardCreature boardCreature in boardCreatures)
            {
                boardCreature.RemoveTauntIfGranted();
            }
        }

        effects.Add(
            new Effect(
                boardStructure.GetPlayerId(),
                EFFECT_STRUCTURE_DIE,
                boardStructure.GetCardId(),
                0
            )
        );

        return effects;
    }

    private void WaitForServerMove(int moveRank)
    {
        IncrementIsWaiting();
        StartCoroutine("WaitForServerMoveCoroutine", new object[1] { moveRank });
    }

    private IEnumerator WaitForServerMoveCoroutine(object[] args)
    {
        double timeCounter = 0.0;

        int moveRank = (int)args[0];
        bool hasSentRequest = false;

        while (!hasSentRequest && BattleState.Instance().ProcessMoveQueue() != moveRank)
        {
            yield return new WaitForSeconds(0.1f);

            timeCounter += 0.1;
            if (timeCounter >= 5.0)
            {
                Debug.LogError("Stuck in waiting mode for five seconds.");
                timeCounter = 0.0;
                DecrementIsWaiting();

                if (FlagHelper.IsServerEnabled())
                {
                    // Reload state from server and reload scene.
                    BattleSingleton.Instance.SendGetActiveChallengeRequest();
                }
                else
                {
                    Debug.LogError("Stuck in waiting mode for five seconds.");
                }

                hasSentRequest = true;
            }
        }
    }

    public void WaitAndInvokeCallback(float duration, UnityAction callback)
    {
        StartCoroutine(
            "WaitAndInvokeCallbackCoroutine",
            new object[2] { duration, callback }
        );
    }

    private IEnumerator WaitAndInvokeCallbackCoroutine(object[] args)
    {
        float duration = (float)args[0];
        UnityAction callback = (UnityAction)args[1];
        yield return new WaitForSeconds(duration);
        callback.Invoke();
    }

    private void IncrementIsWaiting()
    {
        this.isWaiting += 1;
        if (this.isWaiting > 2)
        {
            Debug.LogError("Increment is waiting to > 2.");
        }
    }

    private void DecrementIsWaiting()
    {
        this.isWaiting -= 1;
        if (this.isWaiting < 0)
        {
            Debug.LogError("Decrement is waiting to < 0.");
        }
    }
}
