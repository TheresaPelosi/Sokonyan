using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yarn : MonoBehaviour {

    public bool m_OnCat = false;
    public bool Move(Vector2 direction)
    {
        if (YarnBlocked(transform.position, direction))
        {
            return false;
        } else
        {
            transform.Translate(direction);
            TestOnCat();
            return true;
        }
    }

    bool YarnBlocked(Vector3 position, Vector2 direction)
    {
        Vector2 newPosition = new Vector2(position.x, position.y) + direction;
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        foreach (var wall in walls)
        {
            if (wall.transform.position.x == newPosition.x && wall.transform.position.y == newPosition.y)
            {
                return true;
            }
        }

        GameObject[] yarns = GameObject.FindGameObjectsWithTag("Yarn");
        foreach (var yarn in yarns)
        {
            if (yarn.transform.position.x == newPosition.x && yarn.transform.position.y == newPosition.y)
            {
                return true;
            }
        }

        return false;
    }

    void TestOnCat()
    {
        GameObject[] cats = GameObject.FindGameObjectsWithTag("Cat");
        foreach (var cat in cats)
        {
            if (transform.position.x == cat.transform.position.x && transform.position.y == cat.transform.position.y)
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
                m_OnCat = true;
                return;
            }
        }
        GetComponent<SpriteRenderer>().color = Color.white;
        m_OnCat = false;
    }
}
