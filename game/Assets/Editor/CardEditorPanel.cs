using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.IO;
using System;

using UnityEngine;
using UnityEditor;

public class CardEditorPanel : EditorWindow
{
    private Texture mainTexture;
    private Texture backgroundTexture;
    private GameObject summonPrefab;
    private GameObject effectPrefab;
    private int boxSize = 80;
    private int lineHeight = 20;
    private int lineMargin = 5;

    private static int imageHeight = 350;
    private static int imageWidth = 240;

    private Material frameMat;
    private Material frontMat;
    private Material backMat;
    private Texture cardFrame;

    private int shownTab;
    private string codexPath;

    private CardTemplate createTemplate;
    private CardTemplate editTemplate;
    private Dictionary<string, CardTemplate> templates;

    private int firstAbility;
    private int secondAbility;
    private int thirdAbility;
    private int fourthAbility;

    private bool flipHorizontal;

    [MenuItem("Custom/Card Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CardEditorPanel window = EditorWindow.GetWindow(typeof(CardEditorPanel)) as CardEditorPanel;
        window.minSize = new Vector2(400, 900);
        window.backgroundTexture = EditorGUIUtility.whiteTexture;
        window.cardFrame = Resources.Load("FrameForEditor") as Texture;

        window.frameMat = new Material(Shader.Find("Sprites/Default"));
        window.frontMat = new Material(Shader.Find("Sprites/Default"));
        window.backMat = new Material(Shader.Find("Sprites/Default"));

        window.codexPath = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "codex.txt";

        window.Show();
    }

    void OnGUI()
    {
        if (this.createTemplate == null)
        {
            this.createTemplate = new CardTemplate();
        }

        GUILayout.BeginVertical();

        this.shownTab = GUILayout.Toolbar(this.shownTab, new string[2] { "Create", "Edit" });

        switch (this.shownTab)
        {
            case 0:
                RenderCreateMode();
                break;
            case 1:
                RenderEditMode();
                break;
            default:
                break;
        }
        GUILayout.EndVertical();
    }

    private void RenderCreateMode()
    {
        RenderTemplateEditor(this.createTemplate, 0, false);
    }

    private void RenderEditMode()
    {
        if (GUILayout.Button("Choose Card Template to Edit"))
        {
            // create the menu and add items to it
            GenericMenu menu = new GenericMenu();
            this.templates = CodexHelper.ParseFile(this.codexPath);

            // forward slashes nest menu items under submenus
            foreach (string templateName in templates.Keys)
            {
                AddMenuItem(menu, templateName);
            }

            menu.ShowAsContext();
        }

        if (this.editTemplate != null)
        {
            RenderTemplateEditor(this.editTemplate, 25, true);
        }
    }

    void AddMenuItem(GenericMenu menu, string itemName)
    {
        // the menu item is marked as selected if it matches the current value of m_Color
        menu.AddItem(new GUIContent(itemName), this.editTemplate != null && this.editTemplate.name.Equals(itemName), ItemSelected, itemName);
    }

    private void ItemSelected(object chosenName)
    {
        string chosen = chosenName as string;
        CardTemplate chosenTemplate = templates[chosen];

        this.editTemplate = chosenTemplate;

        //do loading stuff here
        LoadTemplateData(chosenTemplate);
    }

    private void LoadTemplateData(CardTemplate chosen)
    {
        this.mainTexture = Resources.Load(chosen.frontImage) as Texture;
        this.backgroundTexture = Resources.Load(chosen.backImage) as Texture;

        this.firstAbility = Array.IndexOf(Card.VALID_ABILITIES, chosen.abilities[0]);
        this.secondAbility = Array.IndexOf(Card.VALID_ABILITIES, chosen.abilities[1]);
        this.thirdAbility = Array.IndexOf(Card.VALID_ABILITIES, chosen.abilities[2]);
        this.fourthAbility = Array.IndexOf(Card.VALID_ABILITIES, chosen.abilities[3]);

        if (this.firstAbility < 0)
            this.firstAbility = 0;
        if (this.secondAbility < 0)
            this.secondAbility = 0;
        if (this.thirdAbility < 0)
            this.thirdAbility = 0;
        if (this.fourthAbility < 0)
            this.fourthAbility = 0;
    }

    private void RenderTemplateEditor(CardTemplate template, int verticalOffset, bool editMode)
    {
        ApplyCorrections(template);  //do fixes as needed

        CardArtworkSelect(template, verticalOffset, editMode);
        CardDefaultSection(template, verticalOffset);
        CardByTypeSection(template, verticalOffset, editMode);

        bool valid = ValidateEntries(template);
        string buttonMessage = "Save/export to NEW card to Codex";
        if (editMode)
        {
            buttonMessage = "Save/export EDITS to existing card in Codex";
        }

        GUIStyle style = new GUIStyle(GUI.skin.button);

        if (valid && GUI.Button(new Rect(5, position.height - 35, position.width - 10, 30), buttonMessage, style))
        {
            GenerateCard(template, editMode);
        }
    }

    private void ApplyCorrections(CardTemplate template)
    {
        if (template.frontScale.x < 0)
        {
            template.frontScale *= -1;
            this.flipHorizontal = true;
        }
        else
        {
            this.flipHorizontal = false;
        }
    }

    private string LocalToResources(string path)
    {
        if (!path.Contains("Resources"))
        {
            Debug.LogError("Bad path recieved by CardEditorPanel; asset not in 'Resources' directory: " + path);
            return null;
        }

        string[] projectPath = path.Split(Path.DirectorySeparatorChar);

        int startIndex = 0;
        for (int i = 0; i < projectPath.Length; i++)
        {
            if (projectPath[i] == "Resources")
            {
                startIndex = i + 1;
            }
        }

        string normalized = "";
        for (int i = startIndex; i < projectPath.Length - 1; i++)
        {
            normalized = normalized + projectPath[i] + Path.DirectorySeparatorChar;
        }
        path = normalized + Path.GetFileNameWithoutExtension(path);
        return path;
    }

    private void GenerateCard(CardTemplate template, bool editMode)
    {
        //to-do: paths must be only one and exactly one depth under Resources for now, too lazy for regex
        template.frontImage = LocalToResources(AssetDatabase.GetAssetPath(this.mainTexture));
        template.backImage = LocalToResources(AssetDatabase.GetAssetPath(this.backgroundTexture));

        if (this.summonPrefab != null)
            template.summonPrefab = LocalToResources(AssetDatabase.GetAssetPath(this.summonPrefab));
        template.effectPrefab = LocalToResources(AssetDatabase.GetAssetPath(this.effectPrefab));

        template.abilities[0] = Card.VALID_ABILITIES[this.firstAbility];
        template.abilities[1] = Card.VALID_ABILITIES[this.secondAbility];
        template.abilities[2] = Card.VALID_ABILITIES[this.thirdAbility];
        template.abilities[3] = Card.VALID_ABILITIES[this.fourthAbility];

        ////to-do: special attributes?
        if (!editMode)
        {
            AppendToCodex(template);
        }
        else
        {
            EditTemplateInCodex(template);
        }
        //this.createTemplate = new CardTemplate();
    }

    private void AppendToCodex(CardTemplate template)
    {
        // This text is added only once to the file.
        CodexHelper.AppendElement(this.codexPath, template);
    }

    private void EditTemplateInCodex(CardTemplate template)
    {
        CodexHelper.EditElement(this.codexPath, template);
    }


    private void StatusBox(Color color, string message)
    {
        EditorGUI.DrawRect(new Rect(5, position.height - 60, position.width - 10, 30), color);
        EditorGUI.LabelField(new Rect(10, position.height - 60 + lineMargin, position.width - 20, 30), message);
    }

    private void ValidationErrorBox(string message)
    {
        StatusBox(new Color(1, 0.33f, 0.33f), message);
    }

    private bool ValidateEntries(CardTemplate template)
    {
        if (this.mainTexture == null)
        {
            ValidationErrorBox("Missing main artwork.");
            return false;
        }
        else if (this.backgroundTexture == null)
        {
            ValidationErrorBox("Missing background artwork.");
            return false;
        }
        else if (template.name == null || template.name.Length <= 0)
        {
            ValidationErrorBox("Missing name.");
            return false;
        }
        else if (template.cardType == Card.CardType.Creature && template.health == 0)
        {
            ValidationErrorBox("Health cannot be zero.");
            return false;
        }
        else if (template.cardType == Card.CardType.Creature && this.summonPrefab == null)
        {
            ValidationErrorBox("Creature cards must have a summon prefab.");
            return false;
        }
        else if (this.effectPrefab == null)
        {
            ValidationErrorBox("Missing effect prefab.");
            return false;
        }
        return true;
    }

    private void CardArtworkSelect(CardTemplate template, int verticalOffset, bool editMode)
    {
        int top = 25 + verticalOffset;
        this.mainTexture = EditorGUI.ObjectField(new Rect(5, top, boxSize, boxSize), this.mainTexture, typeof(Texture), false) as Texture;
        this.backgroundTexture = EditorGUI.ObjectField(new Rect(5, top + boxSize, boxSize, boxSize), this.backgroundTexture, typeof(Texture), false) as Texture;
        this.flipHorizontal = EditorGUI.Toggle(new Rect(boxSize + 15 + 120, top, 200, 20), "Flip horizontal", this.flipHorizontal);

        // front scale/tiling
        template.frontScale.x = EditorGUI.Slider(new Rect(boxSize + 10, top + 25, 160, 18), Mathf.Abs(template.frontScale.x), 0.33F, 3);
        template.frontScale.y = EditorGUI.Slider(new Rect(boxSize + 160 + 15, top + 25, 160, 18), template.frontScale.y, 0.33F, 3);
        float maybeFlip = this.flipHorizontal ? -1 : 1;
        template.frontScale.x *= maybeFlip;

        // front offset
        template.frontOffset.x = EditorGUI.Slider(new Rect(boxSize + 10, top + 47, 160, 18), template.frontOffset.x, -3, 3);
        template.frontOffset.y = EditorGUI.Slider(new Rect(boxSize + 160 + 15, top + 47, 160, 18), template.frontOffset.y, -3, 3);


        EditorGUI.PrefixLabel(new Rect(boxSize + 10, top + lineMargin + boxSize, 400, 20),
                              new GUIContent("Back: scale, offset"));
        // back scale/tiling
        template.backScale.x = EditorGUI.Slider(new Rect(boxSize + 10, top + 25 + boxSize, 160, 18), template.backScale.x, 0.33F, 3);
        template.backScale.y = EditorGUI.Slider(new Rect(boxSize + 160 + 15, top + 25 + boxSize, 160, 18), template.backScale.y, 0.33F, 3);
        // back offset
        template.backOffset.x = EditorGUI.Slider(new Rect(boxSize + 10, top + 47 + boxSize, 160, 18), template.backOffset.x, -3, 3);
        template.backOffset.y = EditorGUI.Slider(new Rect(boxSize + 160 + 15, top + 47 + boxSize, 160, 18), template.backOffset.y, -3, 3);

        //
        if (this.mainTexture != null || this.backgroundTexture != null)
        {
            //background needs to be rendered first to be on bottom
            if (this.backgroundTexture != null)
            {
                float verticalSize = CardEditorPanel.imageHeight * 1 / template.backScale.y;
                EditorGUI.DrawPreviewTexture(new Rect(25 - template.backOffset.x / template.backScale.x * imageWidth, position.height - 370 - 25 + template.backOffset.y * imageHeight + (imageHeight - verticalSize) * template.backScale.y,
                                                      CardEditorPanel.imageWidth * 1 / template.backScale.x, verticalSize),
                                                      this.backgroundTexture, backMat);
            }
            else
            {
                EditorGUI.DrawPreviewTexture(new Rect(25, position.height - 370, 240, 350), EditorGUIUtility.whiteTexture, backMat);
            }
            if (this.mainTexture != null)
            {
                float verticalSize = CardEditorPanel.imageHeight * 1 / template.frontScale.y;
                EditorGUI.DrawPreviewTexture(new Rect(25 - template.frontOffset.x / template.frontScale.x * imageWidth, position.height - 370 - 25 + template.frontOffset.y * imageHeight + (imageHeight - verticalSize) * template.frontScale.y,
                                                      CardEditorPanel.imageWidth * 1 / template.frontScale.x, verticalSize),
                                                      this.mainTexture, frontMat);
            }

            //clear button
            if (GUI.Button(new Rect(boxSize + 10, top, 120, 20), "Clear textures"))
            {
                this.mainTexture = null;
                this.backgroundTexture = EditorGUIUtility.whiteTexture;
            }
        }
        else
        {
            EditorGUI.PrefixLabel(new Rect(23, position.height - 25, position.width - 6, 20), 0, new GUIContent("No background texture found"));
        }
        EditorGUI.DrawPreviewTexture(new Rect(5, position.height - 400, 280, 390), this.cardFrame, frameMat);  //hardcoded for card frame
    }

    private void CardDefaultSection(CardTemplate template, int verticalOffset)
    {
        //start card attributes
        int enumSelectorYPos = 200 + verticalOffset;
        template.cardType = (Card.CardType)EditorGUI.EnumPopup(new Rect(5, enumSelectorYPos, position.width - 10, 20), template.cardType);

        int standardDetailsYPos = 220 + verticalOffset;
        EditorGUI.LabelField(new Rect(5, standardDetailsYPos, 40, lineHeight), "Name");
        template.name = EditorGUI.TextField(new Rect(50, standardDetailsYPos, 120, lineHeight), template.name);

        EditorGUI.LabelField(new Rect(5, standardDetailsYPos + lineHeight + lineMargin, 40, lineHeight), "Cost");
        template.cost = EditorGUI.IntField(new Rect(50, standardDetailsYPos + lineHeight + lineMargin, 120, lineHeight), template.cost);

        template.rarity = (Card.RarityType)EditorGUI.EnumPopup(new Rect(5, enumSelectorYPos + lineHeight * 3 + lineMargin * 2, position.width - 10, 20), template.rarity);

        EditorGUI.LabelField(new Rect(5, standardDetailsYPos + 65, position.width - 10, lineHeight), "Description (use html tags)");
        template.description = EditorGUI.TextArea(new Rect(5, standardDetailsYPos + 80, position.width - 10, lineHeight * 3), template.description);
    }

    private void CardByTypeSection(CardTemplate template, int verticalOffset, bool editMode)
    {
        if (editMode)
        {
            this.summonPrefab = Resources.Load(template.summonPrefab) as GameObject;
            this.effectPrefab = Resources.Load(template.effectPrefab) as GameObject;
        }

        // === begin card specific fields ===
        int cardSpecificXPos = 185;
        int cardSpecificYPos = 220 + +verticalOffset;
        int belowRarity = 365 + +verticalOffset;
        switch (template.cardType)
        {
            case Card.CardType.Creature:

                EditorGUI.LabelField(new Rect(cardSpecificXPos, cardSpecificYPos, 40, lineHeight), "Attack");
                template.attack = EditorGUI.IntField(new Rect(cardSpecificXPos + 50, cardSpecificYPos, 50, lineHeight), template.attack);

                EditorGUI.LabelField(new Rect(cardSpecificXPos, cardSpecificYPos + lineHeight + lineMargin, 40, lineHeight), "Health");
                template.health = EditorGUI.IntField(new Rect(cardSpecificXPos + 50, cardSpecificYPos + lineHeight + lineMargin, 50, lineHeight), template.health);

                //prefab preview logic
                EditorGUI.LabelField(new Rect(5, belowRarity, position.width / 2 - 10, lineHeight), "Creature/Summon Prefab");
                this.summonPrefab = EditorGUI.ObjectField(new Rect(5, belowRarity + lineHeight, position.width / 2 - 10, 16), this.summonPrefab, typeof(GameObject), false) as GameObject;

                EditorGUI.LabelField(new Rect(position.width / 2, belowRarity, position.width / 2 - 10, lineHeight), "Attack Effect Prefab");
                this.effectPrefab = EditorGUI.ObjectField(new Rect(position.width / 2, belowRarity + lineHeight, position.width / 2 - 10, 16), this.effectPrefab, typeof(GameObject), false) as GameObject;
                EditorGUI.LabelField(new Rect(5, belowRarity + lineHeight * 5 + lineMargin * 2, position.width - 10, lineHeight * 2), "Reminder: make sure the each prefab has appropriate\nsound effect attached with Play On Enable.");


                float qtr = position.width - 10;
                this.firstAbility = EditorGUI.Popup(new Rect(5, belowRarity + 40, qtr, 20), this.firstAbility, Card.VALID_ABILITIES);
                this.secondAbility = EditorGUI.Popup(new Rect(5, belowRarity + 40 + 16, qtr, 20), this.secondAbility, Card.VALID_ABILITIES);
                this.thirdAbility = EditorGUI.Popup(new Rect(5, belowRarity + 40 + 16 * 2, qtr, 20), this.thirdAbility, Card.VALID_ABILITIES);
                this.fourthAbility = EditorGUI.Popup(new Rect(5, belowRarity + 40 + 16 * 3, qtr, 20), this.fourthAbility, Card.VALID_ABILITIES);
                break;
            case Card.CardType.Weapon:
                EditorGUI.LabelField(new Rect(cardSpecificXPos, cardSpecificYPos, 50, lineHeight), "Durability");
                template.durability = EditorGUI.IntField(new Rect(cardSpecificXPos + 50, cardSpecificYPos, 50, lineHeight), template.durability);
                break;
            case Card.CardType.Structure:
                //to-do: lots
                break;
            case Card.CardType.Spell:
                template.targeted = EditorGUI.ToggleLeft(new Rect(cardSpecificXPos, cardSpecificYPos, 80, 20), "Targeted?", template.targeted);

                EditorGUI.LabelField(new Rect(5, belowRarity, position.width / 2 - 10, lineHeight), "Spell Effect Prefab");
                this.effectPrefab = EditorGUI.ObjectField(new Rect(5, belowRarity + lineHeight, position.width - 10, 16), this.effectPrefab, typeof(GameObject), false) as GameObject;
                EditorGUI.LabelField(new Rect(5, belowRarity + lineHeight * 2 + lineMargin, position.width - 10, lineHeight * 2), "Reminder: make sure the each prefab has appropriate\nsound effect attached with Play On Enable.");
                break;
        }
    }
}
