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

    private static List<string> EFFECT_PRIORITY_ORDER = new List<string>()
    {
        EFFECT_CARD_DIE,
        Card.BUFF_CATEGORY_UNSTABLE_POWER,
        Card.CARD_ABILITY_END_TURN_HEAL_TEN,
        Card.CARD_ABILITY_END_TURN_HEAL_TWENTY,
        Card.CARD_ABILITY_END_TURN_DRAW_CARD,
        Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD,
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

        public Effect(string playerId, string name, string cardId, int spawnRank)
        {
            this.playerId = playerId;
            this.name = name;
            this.cardId = cardId;
            this.spawnRank = spawnRank;
        }
    }

    private void Awake()
    {
        Instance = this;

        this.queue = new List<Effect>();
        this.isWaiting = false;
    }

    // Use this for initialization
    void Start()
    {

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

    private void ProcessQueue()
    {
        if (this.queue.Count <= 0)
        {
            Debug.LogError("Process queue called when queue is empty.");
            return;
        }

        this.queue.Sort(delegate (Effect a, Effect b)
        {
            int aOrder = EFFECT_PRIORITY_ORDER.IndexOf(a.Name);
            int bOrder = EFFECT_PRIORITY_ORDER.IndexOf(b.Name);

            if (aOrder == bOrder)
            {
                return a.SpawnRank < b.SpawnRank ? -1 : 1;
            }
            else
            {
                return aOrder < bOrder ? -1 : 1;
            }
        });

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
            case EFFECT_CARD_DIE:
                boardCreature.Die();
                break;
            case Card.CARD_ABILITY_END_TURN_HEAL_TEN:
                boardCreature.Heal(10);
                break;
            case Card.CARD_ABILITY_END_TURN_HEAL_TWENTY:
                boardCreature.Heal(20);
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
            case Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD:
                AbilityDeathRattleDrawCard(effect);
                break;
            case Card.BUFF_CATEGORY_UNSTABLE_POWER:
                boardCreature.SetHealth(0);
                break;
            default:
                Debug.LogError(string.Format("Unhandled effect: {0}.", effect.Name));
                break;
        }
    }

    private void AbilityDeathRattleDrawCard(Effect effect)
    {
        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(effect.PlayerId);
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD);
        challengeMove.SetRank(BattleManager.Instance.GetDeviceMoveRank());
        BattleManager.Instance.AddDeviceMove(challengeMove);

        this.isWaiting = true;
        StartCoroutine("WaitForDrawCard", new object[1] { challengeMove.Rank });

        if (!InspectorControlPanel.Instance.DevelopmentMode)
        {
            BattleManager.Instance.GetPlayerById(effect.PlayerId).DrawCards(1);
        }

        Board.Instance.RemoveCreatureByPlayerIdAndCardId(effect.PlayerId, effect.CardId);
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

        for (int i = 0; i < 6; i += 1)
        {
            BoardCreature boardCreature = field.GetCreatureByIndex(i);
            if (boardCreature == null)
            {
                continue;
            }

            if (boardCreature.HasBuff(Card.BUFF_CATEGORY_UNSTABLE_POWER))
            {
                this.queue.Add(new Effect(playerId, Card.BUFF_CATEGORY_UNSTABLE_POWER, boardCreature.GetCardId(), boardCreature.SpawnRank));
            }
        }
    }

    public void OnEndTurn(string playerId, UnityAction callback)
    {
        this.callback = callback;

        Board.PlayingField field = Board.Instance.GetFieldByPlayerId(playerId);

        for (int i = 0; i < 6; i += 1)
        {
            BoardCreature boardCreature = field.GetCreatureByIndex(i);
            if (boardCreature == null)
            {
                continue;
            }

            if (boardCreature.HasAbility(Card.CARD_ABILITY_END_TURN_HEAL_TEN))
            {
                this.queue.Add(new Effect(playerId, Card.CARD_ABILITY_END_TURN_HEAL_TEN, boardCreature.GetCardId(), boardCreature.SpawnRank));
            }
            else if (boardCreature.HasAbility(Card.CARD_ABILITY_END_TURN_HEAL_TWENTY))
            {
                this.queue.Add(new Effect(playerId, Card.CARD_ABILITY_END_TURN_HEAL_TWENTY, boardCreature.GetCardId(), boardCreature.SpawnRank));
            }

            if (boardCreature.HasAbility(Card.CARD_ABILITY_END_TURN_DRAW_CARD))
            {
                this.queue.Add(new Effect(playerId, Card.CARD_ABILITY_END_TURN_DRAW_CARD, boardCreature.GetCardId(), boardCreature.SpawnRank));
            }
        }
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

        if (boardCreature.HasAbility(Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD))
        {
            this.queue.Add(new Effect(playerId, Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD, boardCreature.GetCardId(), boardCreature.SpawnRank));
        }
    }

    public void OnCreatureAttack(string playerId, string cardId, string fieldId, string targetId)
    {

    }

    public void OnCreatureAttack(Targetable attackingTargetable, Targetable defendingTargetable)
    {
        if (
            attackingTargetable.GetType() == typeof(BoardCreature) &&
            defendingTargetable.GetType() == typeof(BoardCreature)
        )
        {
            BoardCreature attackingCreature = attackingTargetable as BoardCreature;
            BoardCreature defendingCreature = defendingTargetable as BoardCreature;

            attackingCreature.Fight(defendingCreature);

            List<Effect> effects = new List<Effect>();

            if (attackingCreature.Health <= 0)
            {
                effects.Add(
                    new Effect(
                        attackingCreature.Owner.Id,
                        EFFECT_CARD_DIE,
                        attackingCreature.GetCardId(),
                        attackingCreature.SpawnRank
                    )
                );

                effects.AddRange(GetEffectsOnCreatureDeath(attackingCreature));
            }
            if (defendingCreature.Health <= 0)
            {
                effects.Add(
                    new Effect(
                        defendingCreature.Owner.Id,
                        EFFECT_CARD_DIE,
                        defendingCreature.GetCardId(),
                        defendingCreature.SpawnRank
                    )
                );

                effects.AddRange(GetEffectsOnCreatureDeath(defendingCreature));
            }

            this.queue.AddRange(effects);
        }
        else
        {
            Debug.LogError("Unsupported.");
        }
    }

    private List<Effect> GetEffectsOnCreatureDeath(BoardCreature boardCreature)
    {
        List<Effect> effects = new List<Effect>();

        if (boardCreature.HasAbility(Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD))
        {
            effects.Add(
                new Effect(
                    boardCreature.Owner.Id,
                    Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
                    boardCreature.GetCardId(),
                    boardCreature.SpawnRank
                )
            );
        }
        if (boardCreature.HasAbility(Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_TWENTY))
        {
            effects.Add(
                new Effect(
                    boardCreature.Owner.Id,
                    Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_TWENTY,
                    boardCreature.GetCardId(),
                    boardCreature.SpawnRank
                )
            );
        }

        return effects;
    }

    public void OnCreatureDeath(string playerId, string cardId)
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

        if (boardCreature.HasAbility(Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD))
        {
            this.queue.Add(new Effect(playerId, Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD, boardCreature.GetCardId(), boardCreature.SpawnRank));
        }
    }
}
