#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class DeveloperPanel : EditorWindow
{
    private bool isInitialized;
    private bool flagsFoldout;

    private bool useServer;
    private bool useStaging;
    private bool playCampaign;
    private bool logVerbose;
    private bool skipMulligan;
    private bool showChinese;

    [MenuItem("Custom/Developer Panel")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DeveloperPanel window = EditorWindow.GetWindow(typeof(DeveloperPanel)) as DeveloperPanel;
        window.minSize = new Vector2(100, 100);
        window.Show();
    }

    private void OnGUI()
    {
        if (!this.isInitialized)
        {
            this.flagsFoldout = true;
            this.useServer = FlagHelper.GetFlag(FlagHelper.FLAG_USE_SERVER);
            this.useStaging = FlagHelper.GetFlag(FlagHelper.FLAG_USE_STAGING);
            this.playCampaign = FlagHelper.GetFlag(FlagHelper.FLAG_PLAY_CAMPAIGN);
            this.logVerbose = FlagHelper.GetFlag(FlagHelper.FLAG_LOG_VERBOSE);
            this.skipMulligan = FlagHelper.GetFlag(FlagHelper.FLAG_SKIP_MULLIGAN);
            this.showChinese = FlagHelper.GetFlag("LANGUAGE");

            this.isInitialized = true;
        }

        EditorGUILayout.BeginVertical();
        this.flagsFoldout = EditorGUILayout.Foldout(this.flagsFoldout, "Development Flags", true);
        if (this.flagsFoldout)
        {
            this.useServer = EditorGUILayout.Toggle("Use server", this.useServer);
            this.useStaging = EditorGUILayout.Toggle("Use staging", this.useStaging);
            this.playCampaign = EditorGUILayout.Toggle("Play campaign", this.playCampaign);
            this.logVerbose = EditorGUILayout.Toggle("Log verbose", this.logVerbose);
            this.skipMulligan = EditorGUILayout.Toggle("Skip mulligan", this.skipMulligan);
            this.showChinese = EditorGUILayout.Toggle("Show chinese", this.showChinese);
        }

        if (GUI.Button(new Rect(5, position.height - 35, position.width - 10, 30), "Save Settings"))
        {
            SaveSettings();
        }

        EditorGUILayout.EndVertical();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(FlagHelper.FLAG_USE_SERVER, this.useServer ? 1 : 0);
        PlayerPrefs.SetInt(FlagHelper.FLAG_USE_STAGING, this.useStaging ? 1 : 0);
        PlayerPrefs.SetInt(FlagHelper.FLAG_PLAY_CAMPAIGN, this.playCampaign ? 1 : 0);
        PlayerPrefs.SetInt(FlagHelper.FLAG_LOG_VERBOSE, this.logVerbose ? 1 : 0);
        PlayerPrefs.SetInt(FlagHelper.FLAG_SKIP_MULLIGAN, this.skipMulligan ? 1 : 0);
        PlayerPrefs.SetInt("LANGUAGE", this.showChinese ? 1 : 0);

        //done getting values, now save
        PlayerPrefs.Save();
    }
}
#endif
