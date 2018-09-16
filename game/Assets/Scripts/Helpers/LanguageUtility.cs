using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LanguageUtility
{
    private static bool LOG_STRICT = false;

    private static LanguageUtility instance;
    private Dictionary<string, List<string>> cardNames;
    private Dictionary<string, List<string>> cardDescriptions;
    private LanguageUtility.Language selectedLanguage;

    private static List<Regex> GlobalPatterns = new List<Regex>() {
        new Regex(@" \+.*\/+.+? ", RegexOptions.Compiled | RegexOptions.IgnoreCase)  //stats
    };

    private static List<Regex> ItalicPatterns = new List<Regex>() {
        new Regex(@"“.*”", RegexOptions.Compiled | RegexOptions.IgnoreCase)  //flavor text
    };

    private static List<Regex> EnglishPatterns = new List<Regex>() {
        new Regex(@"((Warcry)|(Deathwish)|(Doublestrike)|(Piercing)|(Lethal)|(Haste)|(Shielded)|(Protector)|(Lifesap)|(Turnover)|(Ranged))", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new Regex(@"((Summon)|(Resurrect)|(Convert)|(Destroy)|(Condemn)|(Draw)|(Freeze)|(Restore)|(Recover))", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new Regex(@"((Creature)|(Spell)|(Weapon)|(Structure))", RegexOptions.Compiled)
    };

    private static List<Regex> ChinesePatterns = new List<Regex>() {
        new Regex(@"((战嚎)|(亡愿)|(双击)|(穿刺)|(致命)|(冲锋)|(护盾)|(保卫)|(吸血)|(回合之后)|(远击))", RegexOptions.Compiled),
        new Regex(@"((召唤)|(复活)|(转化)|(毁灭)|(诅咒)|(取)|(冰冻)|(恢复)|(回复))", RegexOptions.Compiled),
        new Regex(@"((单位牌)|(Spell)|(武器牌)|(Structure))", RegexOptions.Compiled)
    };

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
        this.cardDescriptions = LanguageUtility.LoadTranslations("Translations/descriptions");
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

    public string GetLocalizedName(string name)
    {
        name = name.Replace("～", "~");
        if (name == null)
        {
            if (LOG_STRICT)
            {
                Debug.LogError("GetLocalizedName called on null.");
            }
            else
            {
                Debug.LogWarning("GetLocalizedName called on null.");
            }
            return name;
        }

        if (!this.cardNames.ContainsKey(name))
        {
            if (LOG_STRICT)
            {
                Debug.LogError(string.Format("Missing translation for: {0}", name));
            }
            else
            {
                Debug.LogWarning(string.Format("Missing translation for: {0}", name));
            }
            return name;
        }
        string value = this.cardNames[name][(int)this.selectedLanguage];
        if (value.Equals(""))
        {
            if (LOG_STRICT)
            {
                Debug.LogError(string.Format("Empty translation for: {0}", name));
            }
            else
            {
                Debug.LogWarning(string.Format("Empty translation for: {0}", name));
            }
            return name;
        }

        value = value.Replace("~", ",").Replace("～", ",");
        return value;
    }

    public string GetLocalizedDescription(string description)
    {
        description = description.Replace("～", "~");
        if (description == null)
        {
            if (LOG_STRICT)
            {
                Debug.LogError("GetLocalizedDescription called on null.");
            }
            else
            {
                Debug.LogWarning("GetLocalizedDescription called on null.");
            }
            return description;
        }

        if (!this.cardDescriptions.ContainsKey(description))
        {
            if (LOG_STRICT)
            {
                Debug.LogError(string.Format("Missing translation for: {0}", description));
            }
            else
            {
                Debug.LogWarning(string.Format("Missing translation for: {0}", description));

            }
            return Prettify(description);
        }
        string value = this.cardDescriptions[description][(int)this.selectedLanguage];
        if (value.Equals(""))
        {
            if (LOG_STRICT)
            {
                Debug.LogError(string.Format("Empty translation for: {0}", description));
            }
            else
            {
                Debug.LogWarning(string.Format("Empty translation for: {0}", description));
            }
            return Prettify(description);
        }
        return Prettify(value);
    }

    //make string into rich text e.g. bolding, commas, new lines
    private string Prettify(string input)
    {
        string output = input.Replace(";", "\n").Replace("~", ",").Replace("～", ",");

        foreach (Regex token in GlobalPatterns)
        {
            foreach (Match match in token.Matches(output))
            {
                output = token.Replace(output, "<b>$0</b>");
            }
        }

        foreach (Regex token in ItalicPatterns)
        {
            foreach (Match match in token.Matches(output))
            {
                output = token.Replace(output, "<i>$0</i>");
            }
        }

        //to-do put a conditional on card type
        foreach (Regex token in EnglishPatterns)
        {
            foreach (Match match in token.Matches(output))
            {
                output = token.Replace(output, "<b>$0</b>");
            }
        }

        foreach (Regex token in ChinesePatterns)
        {
            foreach (Match match in token.Matches(output))
            {
                output = token.Replace(output, "<b>$0</b>");
            }
        }

        return output;
    }

    public LanguageUtility.Language GetLanguage()
    {
        return selectedLanguage;
    }
}
