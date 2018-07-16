using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Hand
{
    [SerializeField]
    private List<BattleCardObject> battleCardObjects;

    private string playerId;
    private string name;

    public Hand(Player player)
    {
        this.battleCardObjects = new List<BattleCardObject>();
        this.playerId = player.Id;
        this.name = player.Name;
    }

    public int Size()
    {
        return battleCardObjects.Count;
    }

    public BattleCardObject GetCardObjectByCardId(string cardId)
    {
        foreach (BattleCardObject battleCardObject in this.battleCardObjects)
        {
            if (battleCardObject.Card.Id == cardId)
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
    }

    public void Discard(int count)
    {

    }

    public void RemoveByIndex(int index)
    {
        this.battleCardObjects.RemoveAt(index);
    }

    public void RemoveByCardId(string cardId)
    {
        int removeIndex = this.battleCardObjects.FindIndex(element => element.Card.Id == cardId);
        this.battleCardObjects.RemoveAt(removeIndex);
    }

    public void RecedeCards()
    {
        RepositionCards(-1);
    }

    public void RepositionCards(float verticalShift = 0)
    {
        int size = this.battleCardObjects.Count;
        //if no cards, return
        if (size <= 0)
        {
            return;
        }

        HashSet<string> cardIdSet = new HashSet<string>();
        foreach (BattleCardObject battleCardObject in this.battleCardObjects)
        {
            if (battleCardObject.Card.Id != "HIDDEN" && cardIdSet.Contains(battleCardObject.Card.Id))
            {
                Debug.LogError("Duplicate card IDs in hand!");
            }
            else
            {
                cardIdSet.Add(battleCardObject.Card.Id);
            }
        }

        float cardWidth = this.battleCardObjects[0].transform.GetComponent<BoxCollider>().size.x + 0.24f - 0.15f * size;
        float rotation_x = 70f;

        for (int k = 0; k < size; k++)
        {
            BattleCardObject battleCardObject = this.battleCardObjects[k];
            if (ActionManager.Instance.GetDragTarget() == battleCardObject)
                continue;

            float pos = -((size - 1) / 2.0f) + k;
            float vertical = -0.15f * Mathf.Abs(pos) + Random.Range(-0.1f, 0.1f);

            RedrawOutline(battleCardObject);
            //to-do: do something with hypercard stencil here?

            Vector3 adjustedPos = new Vector3(pos * cardWidth * 1.2f, 0.2f * pos, vertical) + verticalShift * battleCardObject.transform.forward;
            CardTween.moveLocal(battleCardObject, adjustedPos, CardTween.TWEEN_DURATION);
            LeanTween.rotateLocal(battleCardObject.gameObject, new Vector3(rotation_x, pos * 5, 0), CardTween.TWEEN_DURATION);
        }
    }

    private void RedrawOutline(BattleCardObject battleCardObject)
    {
        Player player = BattleManager.Instance.GetPlayerById(this.playerId);
        battleCardObject.visual.SetOutline(
            player.HasTurn && player.Mana >= battleCardObject.Card.GetCost()
        );
        battleCardObject.visual.Redraw();
    }
}
