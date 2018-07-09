using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class TCGEditorPanel : EditorWindow
{
    private Texture mainTexture;
    private Texture backgroundTexture;
    private GameObject summonPrefab;
    private GameObject effectPrefab;
    private int boxSize = 80;
    private int lineHeight = 20;
    private int lineMargin = 5;

    private int imageHeight = 350;
    private int imageWidth = 240;

    private CardTemplate data;

    private Material frameMat;
    private Material frontMat;
    private Material backMat;
    private Texture cardFrame;

    private float frontHorizontalScale = 1.0F;
    private float frontVerticalScale = 1.0F;
    private float frontHorizontalOffset = 0.0F;
    private float frontVerticalOffset = 0.0F;

    private float backHorizontalScale = 1.0F;
    private float backVerticalScale = 1.0F;
    private float backHorizontalOffset = 0.0F;
    private float backVerticalOffset = 0.0F;


    [MenuItem("Custom/TCG Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TCGEditorPanel window = EditorWindow.GetWindow(typeof(TCGEditorPanel)) as TCGEditorPanel;
        window.minSize = new Vector2(400, 900);
        window.backgroundTexture = EditorGUIUtility.whiteTexture;
        window.cardFrame = Resources.Load("FrameForEditor") as Texture;

        window.frameMat = new Material(Shader.Find("Sprites/Default"));
        window.frontMat = new Material(Shader.Find("Sprites/Default"));
        window.backMat = new Material(Shader.Find("Sprites/Default"));
        window.data = new CardTemplate();

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

        bool valid = ValidateEntries();
        if (GUI.Button(new Rect(5, position.height - 35, position.width - 10, 30), "Save/Export to Card List") && valid)
        {
            GenerateCard();
        }
        GUILayout.EndVertical();
    }

    private string LocalToResources(string path)
    {
        if (!path.Contains("Resources"))
        {
            Debug.LogError(string.Format("Bad path recieved by TCGEditor: {}, asset not in 'Resources' directory.", path));
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

        path = string.Format("{0}{1}", normalized,
                             Path.GetFileNameWithoutExtension(path));
        Debug.Log(path);
        return path;
    }

    private void GenerateCard()
    {
        //to-do: paths must be only one and exactly one depth under Resources for now, too lazy for regex
        this.data.frontImage = LocalToResources(AssetDatabase.GetAssetPath(this.mainTexture));
        this.data.backImage = LocalToResources(AssetDatabase.GetAssetPath(this.backgroundTexture));

        this.data.summonPrefab = LocalToResources(AssetDatabase.GetAssetPath(this.summonPrefab));
        this.data.effectPrefab = LocalToResources(AssetDatabase.GetAssetPath(this.effectPrefab));

        this.data.frontScale = new Vector2(this.frontHorizontalScale, this.frontVerticalScale);
        this.data.frontOffset = new Vector2(this.frontHorizontalOffset, this.frontVerticalOffset);

        this.data.backScale = new Vector2(this.backHorizontalScale, this.backVerticalScale);
        this.data.backOffset = new Vector2(this.backHorizontalOffset, this.backVerticalOffset);

        string json = JsonUtility.ToJson(this.data);
        ////to-do: special attributes?
        AppendToCodexFile(json);
    }

    public static void AppendToCodexFile(string json)
    {
        string path = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "codex.txt";
        // This text is added only once to the file.
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("//Start of codex");
            }
        }

        // This text is always added, making the file longer over time
        // if it is not deleted.
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine(json);
        }
        Debug.Log(json);
    }

    private void ValidationErrorBox(string message)
    {
        EditorGUI.DrawRect(new Rect(5, position.height - 60, position.width - 10, 30), new Color(1, 0.5f, 0.33f));
        EditorGUI.LabelField(new Rect(10, position.height - 60 + lineMargin, position.width - 20, 30), message);
    }

    private bool ValidateEntries()
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
        else if (this.data.name == null || this.data.name.Length <= 0)
        {
            ValidationErrorBox("Missing name.");
            return false;
        }
        else if (this.data.health == 0)
        {
            ValidationErrorBox("Health cannot be zero.");
            return false;
        }
        else if (this.data.cardType == CardRaw.CardType.Creature && this.summonPrefab == null)
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

    private void CardArtworkSection()
    {
        this.mainTexture = EditorGUI.ObjectField(new Rect(5, 20, boxSize, boxSize), this.mainTexture, typeof(Texture), false) as Texture;
        this.backgroundTexture = EditorGUI.ObjectField(new Rect(5, 20 + boxSize, boxSize, boxSize), this.backgroundTexture, typeof(Texture), false) as Texture;

        // front scale/tiling
        this.frontHorizontalScale = EditorGUI.Slider(new Rect(boxSize + 10, 45, 160, 18), this.frontHorizontalScale, 0.33F, 3);
        this.frontVerticalScale = EditorGUI.Slider(new Rect(boxSize + 160 + 15, 45, 160, 18), this.frontVerticalScale, 0.33F, 3);
        // front offset
        this.frontHorizontalOffset = EditorGUI.Slider(new Rect(boxSize + 10, 67, 160, 18), this.frontHorizontalOffset, -3, 3);
        this.frontVerticalOffset = EditorGUI.Slider(new Rect(boxSize + 160 + 15, 67, 160, 18), this.frontVerticalOffset, -3, 3);


        EditorGUI.PrefixLabel(new Rect(boxSize + 10, 25 + boxSize, 400, 20),
                              new GUIContent("Back: scale, offset"));
        // back scale/tiling
        this.backHorizontalScale = EditorGUI.Slider(new Rect(boxSize + 10, 45 + boxSize, 160, 18), this.backHorizontalScale, 0.33F, 3);
        this.backVerticalScale = EditorGUI.Slider(new Rect(boxSize + 160 + 15, 45 + boxSize, 160, 18), this.backVerticalScale, 0.33F, 3);
        // back offset
        this.backHorizontalOffset = EditorGUI.Slider(new Rect(boxSize + 10, 67 + boxSize, 160, 18), this.backHorizontalOffset, -3, 3);
        this.backVerticalOffset = EditorGUI.Slider(new Rect(boxSize + 160 + 15, 67 + boxSize, 160, 18), this.backVerticalOffset, -3, 3);

        //
        if (this.mainTexture != null || this.backgroundTexture != null)
        {
            //background needs to be rendered first to be on bottom
            if (this.backgroundTexture != null)
            {
                float verticalSize = this.imageHeight * 1 / backVerticalScale;
                EditorGUI.DrawPreviewTexture(new Rect(25 - this.backHorizontalOffset / this.backHorizontalScale * imageWidth, position.height - 370 + this.backVerticalOffset * imageHeight + (imageHeight - verticalSize) * backVerticalScale,
                                                      this.imageWidth * 1 / this.backHorizontalScale, verticalSize),
                                                      this.backgroundTexture, backMat);
            }
            else
            {
                EditorGUI.DrawPreviewTexture(new Rect(25, position.height - 370, 240, 350), EditorGUIUtility.whiteTexture, backMat);
            }
            if (this.mainTexture != null)
            {
                float verticalSize = this.imageHeight * 1 / frontVerticalScale;
                EditorGUI.DrawPreviewTexture(new Rect(25 - this.frontHorizontalOffset / this.frontHorizontalScale * imageWidth, position.height - 370 + this.frontVerticalOffset * imageHeight + (imageHeight - verticalSize) * frontVerticalScale,
                                                      this.imageWidth * 1 / this.frontHorizontalScale, verticalSize),
                                                      this.mainTexture, frontMat);
            }

            //clear button
            if (GUI.Button(new Rect(boxSize + 10, lineHeight, 120, 20), "Clear textures"))
            {
                this.mainTexture = null;
                this.backgroundTexture = EditorGUIUtility.whiteTexture;
            }
        }
        else
        {
            EditorGUI.PrefixLabel(new Rect(23, position.height - 25, position.width - 6, 20), 0, new GUIContent("No background texture found"));
        }
        EditorGUI.DrawPreviewTexture(new Rect(5, position.height - 400, 280, 390), this.cardFrame, frameMat);
    }

    private void CardDefaultSection()
    {
        //start card attributes
        int enumSelectorYPos = 200;
        this.data.cardType = (CardRaw.CardType)EditorGUI.EnumPopup(new Rect(5, enumSelectorYPos, position.width - 10, 20), this.data.cardType);

        int standardDetailsYPos = 220;
        EditorGUI.LabelField(new Rect(5, standardDetailsYPos, 40, lineHeight), "Name");
        this.data.name = EditorGUI.TextField(new Rect(50, standardDetailsYPos, 120, lineHeight), this.data.name);

        EditorGUI.LabelField(new Rect(5, standardDetailsYPos + lineHeight + lineMargin, 40, lineHeight), "Cost");
        this.data.cost = EditorGUI.IntField(new Rect(50, standardDetailsYPos + lineHeight + lineMargin, 120, lineHeight), this.data.cost);

        this.data.rarity = (Card.RarityType)EditorGUI.EnumPopup(new Rect(5, enumSelectorYPos + lineHeight * 3 + lineMargin * 2, position.width - 10, 20), this.data.rarity);

        EditorGUI.LabelField(new Rect(5, 285, position.width - 10, lineHeight), "Description (use html tags)");
        this.data.description = EditorGUI.TextArea(new Rect(5, 300, position.width - 10, lineHeight * 3), this.data.description);
    }

    private void CardByTypeSection()
    {
        // === begin card specific fields ===
        int cardSpecificXPos = 185;
        int cardSpecificYPos = 220;
        int belowRarity = 365;
        switch (this.data.cardType)
        {
            case CardRaw.CardType.Creature:
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
            case CardRaw.CardType.Weapon:
                EditorGUI.LabelField(new Rect(cardSpecificXPos, cardSpecificYPos, 50, lineHeight), "Durability");
                this.data.durability = EditorGUI.IntField(new Rect(cardSpecificXPos + 50, cardSpecificYPos, 50, lineHeight), this.data.durability);
                break;
            case CardRaw.CardType.Structure:
                //to-do: lots
                break;
            case CardRaw.CardType.Spell:
                this.data.targeted = EditorGUI.ToggleLeft(new Rect(cardSpecificXPos, cardSpecificYPos, 80, 20), "Targeted?", this.data.targeted);

                EditorGUI.LabelField(new Rect(5, belowRarity, position.width / 2 - 10, lineHeight), "Spell Effect Prefab");
                this.effectPrefab = EditorGUI.ObjectField(new Rect(5, belowRarity + lineHeight, position.width - 10, 16), this.effectPrefab, typeof(GameObject), false) as GameObject;
                EditorGUI.LabelField(new Rect(5, belowRarity + lineHeight * 2 + lineMargin, position.width - 10, lineHeight * 2), "Reminder: make sure the each prefab has appropriate\nsound effect attached with Play On Enable.");
                break;
        }
    }
}
