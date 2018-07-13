using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    private List<Effect> queue;
    private bool isWaiting;

    private UnityAction callback;

    public static EffectManager Instance { get; private set; }

    public const string EFFECT_CARD_DIE = "EFFECT_CARD_DIE";
    public const string EFFECT_CARD_DIE_AFTER_DEATH_RATTLE = "EFFECT_CARD_DIE_AFTER_DEATH_RATTLE";
    public const string EFFECT_PLAYER_AVATAR_DIE = "EFFECT_PLAYER_AVATAR_DIE";
    public const string EFFECT_DEATH_RATTLE_ATTACK_RANDOM = "EFFECT_DEATH_RATTLE_ATTACK_RANDOM";

    private static List<string> EFFECT_PRIORITY_ORDER = new List<string>
    {
        EFFECT_PLAYER_AVATAR_DIE,

        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,

        EFFECT_CARD_DIE,
        Card.CARD_ABILITY_LIFE_STEAL,

        Card.BUFF_CATEGORY_UNSTABLE_POWER,

        Card.CARD_ABILITY_END_TURN_HEAL_TEN,
        Card.CARD_ABILITY_END_TURN_HEAL_TWENTY,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_END_TURN_DRAW_CARD,

        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD,

        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
        EFFECT_DEATH_RATTLE_ATTACK_RANDOM,
        EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
    };

    private static List<string> EFFECTS_END_TURN = new List<string>
    {
        Card.CARD_ABILITY_END_TURN_HEAL_TEN,
        Card.CARD_ABILITY_END_TURN_HEAL_TWENTY,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_END_TURN_DRAW_CARD,
    };

    private static List<string> EFFECTS_START_TURN = new List<string>
    {
        Card.BUFF_CATEGORY_UNSTABLE_POWER,
    };

    private static List<string> EFFECTS_BATTLE_CRY = new List<string>
    {
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
        Card.CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
        Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD,
    };

    private static List<string> EFFECT_DEATH_RATTLE = new List<string>
    {
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
        Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
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

        this.queue = new List<Effect>();
        this.isWaiting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isWaiting)
        {
            return;
        }

        if (this.queue.Count <= 0)
        {
            if (this.callback != null)
            {
                UnityAction action = this.callback;
                this.callback = null;
                action();
            }
            BattleManager.Instance.ProcessMoveQueue();
        }
        else
        {
            ProcessQueue();
        }
    }

    private void AddToQueue(Effect effect)
    {
        this.queue.Add(effect);
    }

    private void AddToQueue(List<Effect> effects)
    {
        effects.Sort(delegate (Effect a, Effect b)
        {
            int aOrder = EFFECT_PRIORITY_ORDER.IndexOf(a.Name);
            int bOrder = EFFECT_PRIORITY_ORDER.IndexOf(b.Name);

            if (a.SpawnRank == b.SpawnRank)
            {
                return aOrder < bOrder ? -1 : 1;
            }
            else
            {
                return a.SpawnRank < b.SpawnRank ? -1 : 1;
            }
        });

        this.queue.AddRange(effects);
    }

    private void ProcessQueue()
    {
        if (this.queue.Count <= 0)
        {
            Debug.LogError("Process queue called when queue is empty.");
            return;
        }

        Effect effect = this.queue[0];
        this.queue.RemoveAt(0);

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
            case EFFECT_PLAYER_AVATAR_DIE:
                Debug.LogWarning("Game over");
                break;
            case EFFECT_CARD_DIE_AFTER_DEATH_RATTLE:
            case EFFECT_CARD_DIE:
                Board.Instance.RemoveCreatureByPlayerIdAndCardId(
                    effect.PlayerId,
                    effect.CardId
                );
                boardCreature.Die();
                break;
            case Card.CARD_ABILITY_LIFE_STEAL:
                AbilityLifeSteal(effect);
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

                if (!InspectorControlPanel.Instance.DevelopmentMode)
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

        if (!InspectorControlPanel.Instance.DevelopmentMode)
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
        // TODO: animate.

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

        AddToQueue(effects);
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

        AddToQueue(effects);
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

        AddToQueue(effects);
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

        AddToQueue(effects);

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(effect.PlayerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD);
        challengeMove.SetRank(BattleManager.Instance.GetDeviceMoveRank());
        BattleManager.Instance.AddDeviceMove(challengeMove);

        this.isWaiting = true;
        StartCoroutine("WaitForDrawCard", new object[1] { challengeMove.Rank });

        if (!InspectorControlPanel.Instance.DevelopmentMode)
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

        AddToQueue(effects);
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
        Board.PlayingField field = Board.Instance.GetFieldByPlayerId(playerId);

        List<Effect> effects = new List<Effect>();

        for (int i = 0; i < 6; i += 1)
        {
            BoardCreature boardCreature = field.GetCreatureByIndex(i);
            if (boardCreature == null)
            {
                continue;
            }

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

        AddToQueue(effects);
    }

    public void OnEndTurn(string playerId, UnityAction callback)
    {
        this.callback = callback;

        Board.PlayingField field = Board.Instance.GetFieldByPlayerId(playerId);

        List<Effect> effects = new List<Effect>();

        for (int i = 0; i < 6; i += 1)
        {
            BoardCreature boardCreature = field.GetCreatureByIndex(i);
            if (boardCreature == null)
            {
                continue;
            }

            /*
             * Redraw each creature, handles things like:
             * - Creature can attack outline
             */
            boardCreature.Redraw();

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

        AddToQueue(effects);
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

        AddToQueue(effects);
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
            else
            {
                attackingCreature.DecrementCanAttack();
            }

            List<Effect> effects = new List<Effect>();

            attackingCreature.Fight(defendingCreature);

            int damageDone = defendingCreature.TakeDamage(attackingCreature.Attack);
            effects.AddRange(GetEffectsOnCreatureDamageDealt(attackingCreature, damageDone));
            effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

            int damageReceived = attackingCreature.TakeDamage(defendingCreature.Attack);
            effects.AddRange(GetEffectsOnCreatureDamageDealt(defendingCreature, damageDone));
            effects.AddRange(GetEffectsOnCreatureDamageTaken(attackingCreature, damageDone));

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

            AddToQueue(effects);
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
            else
            {
                attackingCreature.DecrementCanAttack();
            }

            List<Effect> effects = new List<Effect>();

            attackingCreature.Fight(defendingAvatar);

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

            AddToQueue(effects);
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
        // TODO: animate as bomb or whatever.
        if (
            attackingTargetable.GetType() == typeof(BoardCreature) &&
            defendingTargetable.GetType() == typeof(BoardCreature)
        )
        {
            BoardCreature attackingCreature = attackingTargetable as BoardCreature;
            BoardCreature defendingCreature = defendingTargetable as BoardCreature;

            List<Effect> effects = new List<Effect>();

            attackingCreature.Fight(defendingCreature);

            int damageDone = defendingCreature.TakeDamage(attackingCreature.Attack);
            effects.AddRange(GetEffectsOnCreatureDamageTaken(defendingCreature, damageDone));

            defendingCreature.Redraw();
            attackingCreature.Redraw();

            if (defendingCreature.Health <= 0)
            {
                effects.AddRange(GetEffectsOnCreatureDeath(defendingCreature));
            }

            AddToQueue(effects);
        }
        else if (
            attackingTargetable.GetType() == typeof(BoardCreature) &&
            defendingTargetable.GetType() == typeof(PlayerAvatar)
        )
        {
            BoardCreature attackingCreature = attackingTargetable as BoardCreature;
            PlayerAvatar defendingAvatar = defendingTargetable as PlayerAvatar;

            List<Effect> effects = new List<Effect>();

            attackingCreature.Fight(defendingAvatar);

            int damageDone = defendingAvatar.TakeDamage(attackingCreature.Attack);
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

            AddToQueue(effects);
        }
        else
        {
            Debug.LogError("Unsupported.");
        }
    }

    public void OnSpellTargetedPlay(BattleCardObject battleCardObject, BoardCreature targetedCreature)
    {
        SpellCard spellCard = battleCardObject.Card as SpellCard;
        spellCard.Activate(targetedCreature);

        List<Effect> effects = new List<Effect>();

        switch (spellCard.Name)
        {
            case SpellCard.SPELL_NAME_LIGHTNING_BOLT:
                int damageTaken = targetedCreature.TakeDamage(30);
                effects.AddRange(GetEffectsOnCreatureDamageTaken(targetedCreature, damageTaken));

                if (targetedCreature.Health <= 0)
                {
                    effects.AddRange(GetEffectsOnCreatureDeath(targetedCreature));
                }
                break;
            default:
                Debug.LogError(string.Format("Invalid targeted spell with name: {0}.", spellCard.Name));
                break;
        }

        AddToQueue(effects);
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
