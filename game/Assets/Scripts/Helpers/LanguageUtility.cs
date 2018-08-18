using System.IO;
using System.Collections.Generic;

public class LanguageUtility
{
    private static LanguageUtility instance;
    private Dictionary<string, List<string>> cardNames;
    private Dictionary<string, List<string>> cardAbilities;

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
        cardNames = LanguageUtility.LoadTranslations("Translations/names.csv");
        cardAbilities = LanguageUtility.LoadTranslations("Translations/abilities.csv");
    }

    private static Dictionary<string, List<string>> LoadTranslations(string path)
    {
        Dictionary<string, List<string>> translations = new Dictionary<string, List<string>>();

        char[] delimiters = new char[] { ',' };
        string line;

        using (StreamReader reader = new StreamReader(path))
        {
            while ((line = reader.ReadLine()) != null)
            {
                List<string> elements = new List<string>(line.Split(delimiters));
                string key = elements[0];
                elements.RemoveAt(0);

                translations.Add(key, elements);
            }
        }
        return translations;
    }

    public string GetLocalizedNames(string original, Language id)
    {
        return cardNames[original][(int)id];
    }

    public string GetLocalizedAbilities(string original, Language id)
    {
        return cardAbilities[original][(int)id];
    }
}
