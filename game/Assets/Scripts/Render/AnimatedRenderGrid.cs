using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimatedRenderGrid : MonoBehaviour
{
    [SerializeField]
    private Transform fixedPoint;

    public List<Card> cards;

    private const int ROW_SIZE = 5;

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
        Card.CARD_NAME_EMBERKITTY,
        Card.CARD_NAME_EMILIA_AIRHEART,
        Card.CARD_NAME_LIL_RUSTY,
        Card.CARD_NAME_RHYNOKARP,
        Card.CARD_NAME_THE_SEVEN,
        Card.CARD_NAME_LIGHTHUNTER,
        Card.CARD_NAME_HOOFED_LUSH,
        Card.CARD_NAME_TRAINING_YARD,
        Card.CARD_NAME_IMBUE_FLIGHT,
        Card.CARD_NAME_ABYSSAL_EEL,
        Card.CARD_NAME_MANA_STORM,
        Card.CARD_NAME_CHAR_BOT_451,
        Card.CARD_NAME_CEREBOAROUS,
        Card.CARD_NAME_MEGAPUNK,
        Card.CARD_NAME_TOUCH_OF_ZEUS,
        Card.CARD_NAME_REFRESHMENTS,
        Card.CARD_NAME_TALUSREAVER,
        Card.CARD_NAME_BUBBLE_SQUIRTER,
        Card.CARD_NAME_ARMORED_WARDEN,
        Card.CARD_NAME_CLIFFSIDE_VISTA,
        Card.CARD_NAME_MARSHWATER_SQUEALER,
        Card.CARD_NAME_THIEF_OF_NIGHT,
        Card.CARD_NAME_UNSTABLE_POWER,
        Card.CARD_NAME_SEAHORSE_SQUIRE,
        Card.CARD_NAME_TEMPLE_GUARDIAN,
        Card.CARD_NAME_RESTORATION_WELL,
        Card.CARD_NAME_RITUAL_HATCHLING,
        Card.CARD_NAME_CELESTIAL_PALETTE,
        Card.CARD_NAME_FORESTRY_MOUND,
        Card.CARD_NAME_CRYSTAL_SNAPPER,
        Card.CARD_NAME_WARDENS_OUTPOST,
        Card.CARD_NAME_ARC_KNIGHT,
        Card.CARD_NAME_UNKINDLED_JUNIOR,
        Card.CARD_NAME_DUSK_DWELLER,
        Card.CARD_NAME_TRIDENT_BATTLEMAGE,
        Card.CARD_NAME_SENTIENT_SEAKING,
        Card.CARD_NAME_GUPPEA,
        Card.CARD_NAME_SABRE_CRYSTALLINE_DRAGON,
        Card.CARD_NAME_RALLY_TO_THE_QUEEN,
        Card.CARD_NAME_BOMBSHELL_BOMBADIER,
        Card.CARD_NAME_BLUE_GIPSY_V3,
        Card.CARD_NAME_ADDERSPINE_WEEVIL,
        Card.CARD_NAME_GRAVEYARD_GUARDIAN,
        Card.CARD_NAME_MUTANT_REPLICATION,
        Card.CARD_NAME_FROSTLAND_TOMBSTONE,
        Card.CARD_NAME_DIONYSIAN_TOSSPOT,
        Card.CARD_NAME_BATTLECLAD_GASDON,
        Card.CARD_NAME_ANGELIC_EGG,
        Card.CARD_NAME_GOLDENVALLEY_MINE,
        Card.CARD_NAME_SHIELDS_UP,
        Card.CARD_NAME_REDHAIRED_PALADIN,
        Card.CARD_NAME_IMP_STOMPING_GROUND,
        Card.CARD_NAME_POSEIDONS_HANDMAIDEN,
        Card.CARD_NAME_PIERCING_LIGHTSPEAR,
        Card.CARD_NAME_KRONOS_TIMEWARP_KINGPIN,
        Card.CARD_NAME_TARA_SWAN_PRINCESS,
        Card.CARD_NAME_PAL_V1,
        Card.CARD_NAME_PEARL_NYMPH,
        Card.CARD_NAME_FLAMEBELCHER,
        Card.CARD_NAME_WAVE_CHARMER
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

        yield return new WaitForSeconds(2);

        LeanTween.moveY(this.gameObject, this.gameObject.transform.position.y - 68.8f, 20).setEaseInQuad();
        //#if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false;
        //#endif
    }
}
