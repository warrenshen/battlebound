using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class DeveloperPanel : EditorWindow
{
    private bool flagsFoldout;
    private bool useServer;
    private bool logVerbose;

    public const string FLAG_USE_SERVER = "FLAG_USE_SERVER";
    public const string FLAG_LOG_VERBOSE = "FLAG_LOG_VERBOSE";

    [MenuItem("Custom/Developer Panel")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DeveloperPanel window = EditorWindow.GetWindow(typeof(DeveloperPanel)) as DeveloperPanel;
        window.minSize = new Vector2(100, 100);
        window.flagsFoldout = true;
        window.useServer = DeveloperPanel.GetFlag(DeveloperPanel.FLAG_USE_SERVER);
        window.logVerbose = DeveloperPanel.GetFlag(DeveloperPanel.FLAG_LOG_VERBOSE);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        this.flagsFoldout = EditorGUILayout.Foldout(this.flagsFoldout, "Development Flags");
        if (flagsFoldout)
        {
            this.useServer = EditorGUILayout.Toggle("Use server", this.useServer);
            this.logVerbose = EditorGUILayout.Toggle("Log verbose", this.logVerbose);
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
        int logVerboseValue = this.logVerbose ? 1 : 0;

        PlayerPrefs.SetInt(FLAG_USE_SERVER, useServerValue);
        PlayerPrefs.SetInt(FLAG_LOG_VERBOSE, logVerboseValue);

        //done getting values, now save
        PlayerPrefs.Save();
    }

    public static bool GetFlag(string flag)
    {
        int value = PlayerPrefs.GetInt(flag);
        return value == 1;
    }

    public static bool IsServerEnabled()
    {
        return GetFlag(FLAG_USE_SERVER);
    }

    public static bool IsLogVerbose()
    {
        return GetFlag(FLAG_LOG_VERBOSE);
    }
}
