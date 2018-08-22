using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class RenderCardManager : MonoBehaviour
{
    public List<Card> cards;
    [SerializeField]
    private Transform fixedPoint;

    public IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        //After Unity4,you have to do this function after WaitForEndOfFrame in Coroutine
        //Or you will get the error:"ReadPixels was called to read pixels from system frame buffer, while not inside drawing frame"
        string fileName = String.Format("Renders/{0}.png", name);
        zzTransparencyCapture.captureScreenshot(fileName);
        Debug.Log("Captured " + fileName);
    }

    // Use this for initialization
    void Start()
    {
        cards = new List<Card>();

        foreach (string creatureName in Card.CARD_NAMES_CREATURE)
        {
            CreatureCard creatureCard = new CreatureCard("", creatureName, 0);
            cards.Add(creatureCard);
        }

        foreach (string spellName in Card.CARD_NAMES_SPELL)
        {
            SpellCard spellCard = new SpellCard("", spellName, 0);
            cards.Add(spellCard);
        }

        foreach (string structureName in Card.CARD_NAMES_STRUCTURES)
        {
            StructureCard structureCard = new StructureCard("", structureName, 0);
            cards.Add(structureCard);
        }

        StartCoroutine(RenderCards());
    }

    private IEnumerator RenderCards()
    {
        foreach (Card card in this.cards)
        {
            GameObject cardGameObject = null;
            System.Type type = card.GetType();
            if (type == typeof(CreatureCard))
            {
                cardGameObject = Instantiate(
                    CardSingleton.Instance.CreatureCardObjectPrefab,
                    transform.position,
                    Quaternion.identity
                );
            }
            else if (type == typeof(SpellCard))
            {
                cardGameObject = Instantiate(
                    CardSingleton.Instance.SpellCardObjectPrefab,
                    transform.position,
                    Quaternion.identity
                );
            }
            else if (type == typeof(StructureCard))
            {
                cardGameObject = Instantiate(
                    CardSingleton.Instance.StructureCardObjectPrefab,
                    transform.position,
                    Quaternion.identity
                );
            }
            else //weapon
            {
                cardGameObject = Instantiate(
                    CardSingleton.Instance.WeaponCardObjectPrefab,
                    transform.position,
                    Quaternion.identity
                );
            }
            cardGameObject.name = card.Name;
            cardGameObject.transform.position = fixedPoint.position;
            BattleCardObject battleCardObject = cardGameObject.GetComponent<BattleCardObject>();
            battleCardObject.Initialize(null, card);
            battleCardObject.visual.Redraw();

            battleCardObject.visual.gameObject.SetActive(true);
            cardGameObject.SetActive(true);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();  //give it some time to redraw

            yield return StartCoroutine(Capture(card.Name));

            battleCardObject.visual.gameObject.SetActive(false);
            cardGameObject.SetActive(false);
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
