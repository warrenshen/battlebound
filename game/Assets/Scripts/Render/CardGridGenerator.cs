using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardGridGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform fixedPoint;

    public List<Card> cards;

    private const int ROW_SIZE = 8;
    private const int NUM_CARDS = 50;

    private int index = 0;
    private Vector3 CARD_BOUNDS;

    private List<string> cardNames = new List<string>()
    {
        Card.CARD_NAME_FORGEMECH,
        Card.CARD_NAME_FROSTLAND_THRASHER_8,
        Card.CARD_NAME_BREAKNECK_EVOLUTION,
        Card.CARD_NAME_GOBLIN_SENTRY_TOWER,
        Card.CARD_NAME_CULYSSA,
        Card.CARD_NAME_BRR_BRR_BLIZZARD,
        Card.CARD_NAME_HOBGOBLIN_HOSTEL,
        Card.CARD_NAME_SNEERBLADE,
        Card.CARD_NAME_TIMEBANK_TOWER,
        Card.CARD_NAME_YOUNG_KYO,
        Card.CARD_NAME_FROSTBEARDS_DIRK,
        Card.CARD_NAME_FOXY_APPRENTICE,
        Card.CARD_NAME_RAZE_TO_ASHES,
        Card.CARD_NAME_ACCURSED_FIRESTAR,
        Card.CARD_NAME_HELLBRINGER,
        Card.CARD_NAME_TAJI_THE_FEARLESS,
        Card.CARD_NAME_FIREBORN_MENACE,
        Card.CARD_NAME_BOMBS_AWAY,
        Card.CARD_NAME_NECROMANCERS_TECPATL,
        Card.CARD_NAME_TERRATANK,
        Card.CARD_NAME_DEATH_NOTE,
        Card.CARD_NAME_KRUL_PHANTOM_SKULLCRUSHER,
        Card.CARD_NAME_GREEDY_FINGERS,
        Card.CARD_NAME_DIVINE_CATACLYSM,
        Card.CARD_NAME_SHIPFAIRING_WISDOM,
        Card.CARD_NAME_PRICKLEPILLAR,
        Card.CARD_NAME_BATTLE_ROYALE,
        Card.CARD_NAME_DWARVEN_FORGE,
        Card.CARD_NAME_PAL_V1,
        Card.CARD_NAME_BLESSED_NEWBORN,
        Card.CARD_NAME_HIGHLAND_STABLES,
        Card.CARD_NAME_FIRESTRIDED_TIGRESS,
        Card.CARD_NAME_FIREBUG_CATELYN,
        Card.CARD_NAME_SHIELDS_UP,
        Card.CARD_NAME_LUX,
        Card.CARD_NAME_SENTIENT_SEAKING,
        Card.CARD_NAME_DARK_PACT,
        Card.CARD_NAME_POWER_SIPHONER,
        Card.CARD_NAME_ROYAL_BARRACKS,
        Card.CARD_NAME_FROSTSPORE,
    };

    public void Start()
    {
        this.cards = new List<Card>();

        foreach (string cardName in this.cardNames)
        {
            Card generated = Card.CreateByNameAndLevel("", cardName, 0);
            cards.Add(generated);
        }

        StartCoroutine(RenderCardGrid());
    }

    public IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        //After Unity4,you have to do this function after WaitForEndOfFrame in Coroutine
        //Or you will get the error:"ReadPixels was called to read pixels from system frame buffer, while not inside drawing frame"
        string fileName = String.Format("Renders/{0}.png", name);
        zzTransparencyCapture.captureScreenshot(fileName);
        Debug.Log("Captured " + fileName);
    }

    private IEnumerator RenderCardGrid()
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
            cardGameObject.transform.parent = fixedPoint;
            cardGameObject.transform.position = Vector3.zero +
                                                (index % ROW_SIZE) * CARD_BOUNDS.x * Vector3.right +
                                                (index / ROW_SIZE) * CARD_BOUNDS.y * Vector3.down;

            BattleCardObject battleCardObject = cardGameObject.GetComponent<BattleCardObject>();
            this.CARD_BOUNDS = battleCardObject.GetComponent<BoxCollider>().bounds.size * 1.1f;
            battleCardObject.Initialize(null, card);
            battleCardObject.visual.Redraw();

            battleCardObject.visual.gameObject.SetActive(true);
            cardGameObject.SetActive(true);

            index += 1;
        }

        yield return new WaitForSeconds(1);
        yield return StartCoroutine(Capture("Battlebound_Cards_Grid"));

        //#if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false;
        //#endif
    }
}
