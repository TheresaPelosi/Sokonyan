using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Single_Level
{
    public List<string> m_Rows = new List<string>();
    public int Height {  get { return m_Rows.Count;  } }
    public int Width
    {
        get
        {
            int maxLength = 0;
            foreach (var r in m_Rows)
            {
                if (r.Length > maxLength) maxLength = r.Length;
            }
            return maxLength;
        }
    }
}

public class Level : MonoBehaviour
{
    public string file_name;
    public List<Single_Level> m_levels;

    void Awake()
    {
        TextAsset a = (TextAsset)Resources.Load(file_name);
        if (!a)
        {
            Debug.Log("Levels: " + file_name + " is not in this location.");
            return;
        }
        else
        {
            Debug.Log("Level successfully imported.");
        }

        string levelText = a.text;
        string[] lines;
        lines = levelText.Split(new string[] { "\n" }, System.StringSplitOptions.None);
        m_levels.Add(new Single_Level());
        for (int i = 0; i < lines.LongLength; i++)
        {
            string line = lines[i];
            if (line.StartsWith(";"))
            {
                Debug.Log("Generating next level");
                m_levels.Add(new Single_Level());
                continue;
            }
            m_levels[m_levels.Count - 1].m_Rows.Add(line);
        }
    }
}
