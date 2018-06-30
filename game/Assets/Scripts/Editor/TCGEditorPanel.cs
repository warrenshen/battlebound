using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TCGEditorPanel : EditorWindow
{
    private Texture mainTexture;
    private Texture backgroundTexture;
    private int boxSize = 80;

    private static Material spriteMat;

    [MenuItem("Custom/TCG Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TCGEditorPanel window = EditorWindow.GetWindow(typeof(TCGEditorPanel)) as TCGEditorPanel;
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Card Creator", EditorStyles.boldLabel);

        mainTexture = EditorGUI.ObjectField(new Rect(0, 20, boxSize, boxSize), mainTexture, typeof(Texture), false) as Texture;
        backgroundTexture = EditorGUI.ObjectField(new Rect(boxSize, 20, boxSize, boxSize), backgroundTexture, typeof(Texture), false) as Texture;
        spriteMat = new Material(Shader.Find("Sprites/Default"));


        if (backgroundTexture)
        {
            EditorGUI.DrawPreviewTexture(new Rect(0, 120, 200, 200), backgroundTexture, spriteMat);

            if (GUI.Button(new Rect(3, position.height - 25, position.width - 6, 20), "Clear texture"))
            {
                backgroundTexture = EditorGUIUtility.whiteTexture;
            }
        }
        else
        {
            EditorGUI.PrefixLabel(new Rect(3, position.height - 25, position.width - 6, 20), 0, new GUIContent("No background texture found"));
        }
        if (mainTexture)
        {
            EditorGUI.DrawPreviewTexture(new Rect(0, 120, 200, 200), mainTexture, spriteMat);

            if (GUI.Button(new Rect(3, position.height - 25, position.width - 6, 20), "Clear texture"))
            {
                mainTexture = EditorGUIUtility.whiteTexture;
            }
        }
        else
        {
            EditorGUI.PrefixLabel(new Rect(3, position.height - 25, position.width - 6, 20), 0, new GUIContent("No main creature/character texture found"));
        }


    }
}
