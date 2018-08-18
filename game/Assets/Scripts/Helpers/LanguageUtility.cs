using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class LanguageUtility
{
    private static LanguageUtility instance;
    private Dictionary<string, List<string>> cardNames;
    private Dictionary<string, List<string>> cardAbilities;
    private LanguageUtility.Language selectedLanguage;

    public enum Language { EN, CN, KR, JP }

    public static LanguageUtility Instance()
    {
        if (instance == null)
        {
            instance = new LanguageUtility();
        }
        return instance;
    }

    public LanguageUtility()
    {
        this.cardNames = LanguageUtility.LoadTranslations("Translations/names");
        this.cardAbilities = LanguageUtility.LoadTranslations("Translations/abilities");
        this.selectedLanguage = (LanguageUtility.Language)PlayerPrefs.GetInt("LANGUAGE", 0);
    }

    private static Dictionary<string, List<string>> LoadTranslations(string path)
    {
        Dictionary<string, List<string>> translations = new Dictionary<string, List<string>>();

        char[] delimiters = new char[] { ',' };

        TextAsset translationsText = (TextAsset)Resources.Load(path, typeof(TextAsset));
        string translationsString = translationsText.text;
        List<string> translationStrings = new List<string>(translationsString.Split('\n'));

        foreach (string translationString in translationStrings)
        {
            List<string> elements = new List<string>(translationString.Split(delimiters));
            string key = elements[0];

            if (!translations.ContainsKey(key))
            {
                translations.Add(key, elements);
            }
        }

        return translations;
    }

    public bool HasLocalizedName(string name)
    {
        bool value = this.cardNames.ContainsKey(name);
        if (value)
        {
            Debug.LogWarning(string.Format("Missing translation for: {0}", name));
        }
        return value;
    }

    public string GetLocalizedName(string name)
    {
        return cardNames[name][(int)this.selectedLanguage];
    }

    public bool HasLocalizedAbility(string ability)
    {
        bool value = this.cardNames.ContainsKey(ability);
        if (value)
        {
            Debug.LogWarning(string.Format("Missing translation for: {0}", ability));
        }
        return value;
    }

    public string GetLocalizedAbility(string ability)
    {
        return cardNames[ability][(int)this.selectedLanguage];
    }

    public LanguageUtility.Language GetLanguage()
    {
        return selectedLanguage;
    }
}
