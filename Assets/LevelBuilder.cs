using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelElement
{
    public string m_Character;
    public GameObject m_Prefab;
}

public class LevelBuilder : MonoBehaviour
{
    public int m_CurrentLevel;
    public List<LevelElement> m_LevelElements;
    private Single_Level m_Level;

    GameObject GetPrefab(char c)
    {
        LevelElement element = m_LevelElements.Find(le => le.m_Character == c.ToString());
        if (element != null)
            return element.m_Prefab;
        else
            return null;
    }

    public void NextLevel()
    {
        m_CurrentLevel++;
        if (m_CurrentLevel >= GetComponent<Level>().m_levels.Count)
        {
            m_CurrentLevel = 0;
        }
    }

    public void Build()
    {
        m_Level = GetComponent<Level>().m_levels[m_CurrentLevel];
        int startx = -m_Level.Width / 2;
        int x = startx;
        int y = -m_Level.Height / 2;

        foreach (var row in m_Level.m_Rows)
        {
            foreach (var ch in row)
            {
                GameObject prefab = GetPrefab(ch);
                if (prefab)
                {
                    Instantiate(prefab, new Vector3(x*4, y*4, 0), Quaternion.identity);
                }
                x++;
            }
            y++;
            x = startx;
        }
    }
}
