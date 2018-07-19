using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class DeveloperPanel : EditorWindow
{
    private bool flagsFoldout;
    private bool useServer;
    public const string USE_SERVER_FLAG = "Use Server";

    [MenuItem("Custom/Developer Panel")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DeveloperPanel window = EditorWindow.GetWindow(typeof(DeveloperPanel)) as DeveloperPanel;
        window.minSize = new Vector2(100, 100);
        window.flagsFoldout = true;
        window.useServer = DeveloperPanel.GetFlag(DeveloperPanel.USE_SERVER_FLAG);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        this.flagsFoldout = EditorGUILayout.Foldout(this.flagsFoldout, "Development Flags");
        if (flagsFoldout)
        {
            this.useServer = EditorGUILayout.Toggle("Use Server", this.useServer);
        }


        if (GUI.Button(new Rect(5, position.height - 35, position.width - 10, 30), "Save Settings"))
        {
            SaveSettings();
        }

        EditorGUILayout.EndVertical();
    }

    private void SaveSettings()
    {
        int useServerValue = this.useServer ? 1 : 0;
        PlayerPrefs.SetInt(USE_SERVER_FLAG, useServerValue);

        //done getting values, now save
        PlayerPrefs.Save();
    }

    public static bool GetFlag(string flag)
    {
        int value = PlayerPrefs.GetInt(flag);
        return value == 1;
    }
}
