using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AnimatedRenderCreatures : MonoBehaviour
{
    [SerializeField]
    private Transform fixedPoint;

    public List<Card> cards;

    private const int ROW_SIZE = 3;
    private int index = 0;

    private static System.Random rng = new System.Random();

    public void Start()
    {
        this.cards = new List<Card>();
        List<string> cardNames = new List<string>(Card.CARD_NAMES_CREATURE);
        cardNames.Remove("Floatwood");
        cardNames.Remove("Rhynokarp");

        foreach (string creatureName in cardNames)
        {
            CreatureCard creatureCard = new CreatureCard("", creatureName, 0);
            this.cards.Add(creatureCard);
        }
        cards = new List<Card>(this.cards.OrderBy(a => rng.Next()));

        StartCoroutine(RenderCardGrid());
    }

    private IEnumerator RenderCardGrid()
    {
        foreach (Card card in this.cards)
        {
            GameObject created = SummonPoolManager.Instance.GetSummonFromPool(card.Name);
            created.transform.parent = null;
            created.transform.Rotate(Vector3.right, 45);
            created.transform.position = fixedPoint.transform.position +
                                                (index % ROW_SIZE) * 4.33f * Vector3.right +
                                                (index / ROW_SIZE) * 2.5f * Vector3.forward;
            created.SetActive(true);
            index += 1;
        }

        yield return new WaitForSeconds(2);

        LeanTween.moveZ(this.gameObject, this.gameObject.transform.position.z + 60, 10).setEaseInQuad();
        //#if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false;
        //#endif
    }
}
