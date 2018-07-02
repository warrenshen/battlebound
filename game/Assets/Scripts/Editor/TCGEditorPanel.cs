using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TCGEditorPanel : EditorWindow
{
    private Texture mainTexture;
    private Texture backgroundTexture;
    private GameObject summonPrefab;
    private GameObject effectPrefab;
    private int boxSize = 80;
    private int lineHeight = 20;
    private int lineMargin = 5;

    private enum CardType { Creature, Weapon, Structure, Spell }
    private CardType selectedCardType;

    private struct CardData
    {
        public string name;
        public int cost;
        public Card.RarityType rarity;
        public string mainImagePath;            //doesn't need field
        public string backgroundImagePath;      //doesn't need field
        public string prefabPath;               //doesn't need field

        //creature
        public int attack;
        public int health;
        public string summonPrefabPath;
        public string attackEffectPath;

        //weapon
        public int durability;
        //to-do: special attributes?

        //spell
        public bool targeted;
        public string spellEffectPath;
        //to-do: effect id

    }
    private CardData data;

    private static Material spriteMat;

    [MenuItem("Custom/TCG Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TCGEditorPanel window = EditorWindow.GetWindow(typeof(TCGEditorPanel)) as TCGEditorPanel;
        window.minSize = new Vector2(300, 700);
        window.backgroundTexture = EditorGUIUtility.whiteTexture;
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Label("Card Creator", EditorStyles.boldLabel);

        CardArtworkSection();
        CardDefaultSection();
        CardByTypeSection();

        //to-do: monster cards need attributes list

        //to-do: validation that all fields are filled, saving using create new class, serialize to text file
        //then move on to deletion and editing from editor...

        if (GUI.Button(new Rect(5, position.height - 35, position.width - 10, 30), "Save/Export to Card List"))
        {
            this.mainTexture = null;
            this.backgroundTexture = null;
        }
        GUILayout.EndVertical();
    }

    private void CardArtworkSection()
    {
        this.mainTexture = EditorGUI.ObjectField(new Rect(5, 20, boxSize, boxSize), this.mainTexture, typeof(Texture), false) as Texture;
        this.backgroundTexture = EditorGUI.ObjectField(new Rect(5 + boxSize, 20, boxSize, boxSize), this.backgroundTexture, typeof(Texture), false) as Texture;
        spriteMat = new Material(Shader.Find("Sprites/Default"));

        if (this.mainTexture != null || this.backgroundTexture != null)
        {
            //background needs to be rendered first to be on bottom
            if (this.backgroundTexture != null)
            {
                EditorGUI.DrawPreviewTexture(new Rect(5, position.height - 340, 240, 300), this.backgroundTexture, spriteMat);
            }
            else
            {
                EditorGUI.DrawPreviewTexture(new Rect(5, position.height - 340, 240, 300), EditorGUIUtility.whiteTexture, spriteMat);
            }
            if (this.mainTexture != null)
            {
                EditorGUI.DrawPreviewTexture(new Rect(5, position.height - 340, 240, 300), this.mainTexture, spriteMat);
            }

            //clear button
            if (GUI.Button(new Rect(boxSize * 2 + 10, lineHeight, 120, 20), "Clear textures"))
            {
                this.mainTexture = null;
                this.backgroundTexture = EditorGUIUtility.whiteTexture;
            }
        }
        else
        {
            EditorGUI.PrefixLabel(new Rect(3, position.height - 25, position.width - 6, 20), 0, new GUIContent("No background texture found"));
        }
    }

    private void CardDefaultSection()
    {
        //start card attributes
        int enumSelectorYPos = 120;
        this.selectedCardType = (CardType)EditorGUI.EnumPopup(new Rect(5, enumSelectorYPos, position.width - 10, 20), selectedCardType);

        int standardDetailsYPos = 140;
        EditorGUI.LabelField(new Rect(5, standardDetailsYPos, 40, lineHeight), "Name");
        this.data.name = EditorGUI.TextField(new Rect(50, standardDetailsYPos, 120, lineHeight), this.data.name);

        EditorGUI.LabelField(new Rect(5, standardDetailsYPos + lineHeight + lineMargin, 40, lineHeight), "Cost");
        this.data.name = EditorGUI.TextField(new Rect(50, standardDetailsYPos + lineHeight + lineMargin, 120, lineHeight), this.data.name);

        this.data.rarity = (Card.RarityType)EditorGUI.EnumPopup(new Rect(5, enumSelectorYPos + lineHeight * 3 + lineMargin * 2, position.width - 10, 20), this.data.rarity);
    }

    private void CardByTypeSection()
    {
        // === begin card specific fields ===
        int cardSpecificXPos = 185;
        int cardSpecificYPos = 140;
        int belowRarity = 220;
        switch (selectedCardType)
        {
            case CardType.Creature:
                EditorGUI.LabelField(new Rect(cardSpecificXPos, cardSpecificYPos, 40, lineHeight), "Attack");
                this.data.attack = EditorGUI.IntField(new Rect(cardSpecificXPos + 50, cardSpecificYPos, 50, lineHeight), this.data.attack);

                EditorGUI.LabelField(new Rect(cardSpecificXPos, cardSpecificYPos + lineHeight + lineMargin, 40, lineHeight), "Health");
                this.data.health = EditorGUI.IntField(new Rect(cardSpecificXPos + 50, cardSpecificYPos + lineHeight + lineMargin, 50, lineHeight), this.data.health);

                //prefab preview logic
                EditorGUI.LabelField(new Rect(5, belowRarity, position.width / 2 - 10, lineHeight), "Creature/Summon Prefab");
                this.summonPrefab = EditorGUI.ObjectField(new Rect(5, belowRarity + lineHeight, position.width / 2 - 10, 16), this.summonPrefab, typeof(GameObject), false) as GameObject;

                EditorGUI.LabelField(new Rect(position.width / 2, belowRarity, position.width / 2 - 10, lineHeight), "Attack Effect Prefab");
                this.effectPrefab = EditorGUI.ObjectField(new Rect(position.width / 2, belowRarity + lineHeight, position.width / 2 - 10, 16), this.effectPrefab, typeof(GameObject), false) as GameObject;
                EditorGUI.LabelField(new Rect(5, belowRarity + lineHeight * 2 + lineMargin, position.width - 10, lineHeight * 2), "Reminder: make sure the each prefab has appropriate\nsound effect attached with Play On Enable.");
                break;
            case CardType.Weapon:
                EditorGUI.LabelField(new Rect(cardSpecificXPos, cardSpecificYPos, 50, lineHeight), "Durability");
                this.data.attack = EditorGUI.IntField(new Rect(cardSpecificXPos + 50, cardSpecificYPos, 50, lineHeight), this.data.attack);
                break;
            case CardType.Structure:
                //to-do: lots
                break;
            case CardType.Spell:
                this.data.targeted = EditorGUI.ToggleLeft(new Rect(cardSpecificXPos, cardSpecificYPos, 80, 20), "Targeted?", this.data.targeted);

                EditorGUI.LabelField(new Rect(5, belowRarity, position.width / 2 - 10, lineHeight), "Spell Effect Prefab");
                this.effectPrefab = EditorGUI.ObjectField(new Rect(5, belowRarity + lineHeight, position.width - 10, 16), this.effectPrefab, typeof(GameObject), false) as GameObject;
                EditorGUI.LabelField(new Rect(5, belowRarity + lineHeight * 2 + lineMargin, position.width - 10, lineHeight * 2), "Reminder: make sure the each prefab has appropriate\nsound effect attached with Play On Enable.");
                break;
        }
    }
}
