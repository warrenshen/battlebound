using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Hand
{
    [SerializeField]
    private List<BattleCardObject> battleCardObjects;

    int cardLayer;

    public Hand(Player player)
    {
        this.battleCardObjects = new List<BattleCardObject>();
        this.cardLayer = LayerMask.NameToLayer("Card");
    }

    public int Size()
    {
        return this.battleCardObjects.Count;
    }

    public bool IsFull()
    {
        return Size() >= 10;
    }

    public BattleCardObject GetCardObjectByCardId(string cardId)
    {
        foreach (BattleCardObject battleCardObject in this.battleCardObjects)
        {
            if (battleCardObject.GetCardId() == cardId)
            {
                return battleCardObject;
            }
        }

        return null;
    }

    public BattleCardObject GetCardObjectByIndex(int index)
    {
        return this.battleCardObjects.ElementAt(index);
    }

    public void AddCardObject(BattleCardObject battleCardObject)
    {
        this.battleCardObjects.Add(battleCardObject);
        SoundManager.Instance.PlaySound("PlayCardSFX", battleCardObject.transform.position);

        UpdateCosts();
    }

    public void InsertCardObject(BattleCardObject battleCardObject, int index)
    {
        this.battleCardObjects.Insert(index, battleCardObject);
        SoundManager.Instance.PlaySound("PlayCardSFX", battleCardObject.transform.position);

        UpdateCosts();
    }

    public void RemoveByIndex(int index)
    {
        this.battleCardObjects.RemoveAt(index);

        UpdateCosts();
    }

    public void RemoveByCardId(string cardId)
    {
        int removeIndex = this.battleCardObjects.FindIndex(
            element => element.GetCardId() == cardId
        );

        if (removeIndex < 0)
        {
            Debug.LogError(string.Format("Failed to remove card from hand with card ID: {0}", cardId));
            return;
        }

        this.battleCardObjects.RemoveAt(removeIndex);

        UpdateCosts();
    }

    private void UpdateCosts()
    {
        Dictionary<CardTemplate.ClassColor, int> classColorToCount =
            new Dictionary<CardTemplate.ClassColor, int>();

        foreach (BattleCardObject battleCardObject in this.battleCardObjects)
        {
            CardTemplate.ClassColor classColor = battleCardObject.GetClassColor();

            if (classColor == CardTemplate.ClassColor.Neutral)
            {
                continue;
            }

            if (classColorToCount.ContainsKey(classColor))
            {
                classColorToCount[classColor] += 1;
            }
            else
            {
                classColorToCount[classColor] = 1;
            }
        }

        foreach (BattleCardObject battleCardObject in this.battleCardObjects)
        {
            CardTemplate.ClassColor classColor = battleCardObject.GetClassColor();

            if (classColor == CardTemplate.ClassColor.Neutral)
            {
                continue;
            }

            if (classColorToCount[classColor] >= 3)
            {
                battleCardObject.GrantDecreaseCostByColor();
            }
            else
            {
                battleCardObject.RemoveDecreaseCostByColor();
            }
        }
    }

    public void RepositionCards(UnityAction onRepositionFinish = null)
    {
        int size = this.Size();
        //if no cards, return
        if (size <= 0)
        {
            return;
        }

        HashSet<string> cardIdSet = new HashSet<string>();
        foreach (BattleCardObject battleCardObject in this.battleCardObjects)
        {
            //set to card layer if needed
            if (battleCardObject.gameObject.layer != cardLayer)
                battleCardObject.gameObject.SetLayer(cardLayer);

            if (battleCardObject.GetCardId() != "HIDDEN" && cardIdSet.Contains(battleCardObject.GetCardId()))
            {
                Debug.LogError("Duplicate card IDs in hand!");
            }
            else
            {
                cardIdSet.Add(battleCardObject.GetCardId());
            }
        }

        float cardWidth = this.battleCardObjects[0].transform.GetComponent<BoxCollider>().size.x + 0.24f - 0.15f * size;
        float rotation_x = 70f;

        for (int k = 0; k < size; k++)
        {
            BattleCardObject battleCardObject = this.battleCardObjects[k];
            RedrawOutline(battleCardObject);

            if (ActionManager.Instance.GetDragTarget() == battleCardObject)
            {
                continue;
            }
            if (LeanTween.isTweening(battleCardObject.gameObject))  //TODO: this is hiding and causing some subtle problems...
            {
                Debug.LogError("Called reposition hand animation while card is currently animating!!");
                continue;
            }

            float pos = -((size - 1) / 2.0f) + k;
            float vertical = -0.15f * Mathf.Abs(pos) + Random.Range(-0.1f, 0.1f);

            Vector3 adjustedPos = new Vector3(pos * cardWidth * 1.1f, 0.15f * pos, vertical);
            CardTween.moveLocal(battleCardObject, adjustedPos, CardTween.TWEEN_DURATION);

            if (k == size - 1)
            {
                // If this animation is for the last card in hand, add set on complete.
                LeanTween
                    .rotateLocal(battleCardObject.gameObject, new Vector3(rotation_x, pos * 3f, 0), CardTween.TWEEN_DURATION)
                    .setOnComplete(() =>
                    {
                        if (onRepositionFinish != null)
                        {
                            onRepositionFinish.Invoke();
                        }
                    });
            }
            else
            {
                LeanTween.rotateLocal(
                    battleCardObject.gameObject,
                    new Vector3(rotation_x, pos * 3f, 0),
                    CardTween.TWEEN_DURATION
                );
            }
        }
    }

    public void RenderCards()
    {
        foreach (BattleCardObject element in this.battleCardObjects)
        {
            RedrawOutline(element);
        }
    }

    private void RedrawOutline(BattleCardObject battleCardObject)
    {
        Player player = BattleState.Instance().GetPlayerById(battleCardObject.Owner.Id);

        bool shouldSetOutline = player.HasTurn && player.Mana >= battleCardObject.GetCost();
        if (FlagHelper.IsServerEnabled())
        {
            shouldSetOutline = shouldSetOutline && player.Id == BattleState.Instance().You.Id;
        }

        battleCardObject.visual.SetOutline(shouldSetOutline);
    }
}
