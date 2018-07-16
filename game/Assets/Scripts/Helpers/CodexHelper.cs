using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public static class CodexHelper
{

    public static Dictionary<string, CardTemplate> ParseFile(string path)
    {
        Dictionary<string, CardTemplate> cardTemplates = new Dictionary<string, CardTemplate>();

        using (StreamReader sr = File.OpenText(path))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                CardTemplate cardTemplate = JsonUtility.FromJson<CardTemplate>(line);
                cardTemplates.Add(cardTemplate.name, cardTemplate);
            }
        }
        return cardTemplates;
    }

    public static bool AppendElement(string path, CardTemplate inputted)
    {
        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                //codex file created, do nothing
            }
        }

        string json = JsonUtility.ToJson(inputted);
        Debug.Log(json);

        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine(json);
        }

        return true;
    }

    public static bool EditElement(string path, CardTemplate inputted)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Codex file not found!");
        }

        string[] fileLines = File.ReadAllLines(path);
        for (int i = 0; i < fileLines.Length; i++)
        {
            CardTemplate readTemplate = JsonUtility.FromJson<CardTemplate>(fileLines[i]);
            if (readTemplate.name == inputted.name)
            {
                fileLines[i] = JsonUtility.ToJson(inputted);
                break;
            }

        }
        File.WriteAllLines(path, fileLines);
        Debug.Log(string.Format("Edit card in codex: {0}", inputted.name));
        return true;
    }
}
