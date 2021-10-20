using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool Move(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) < 0.5)
        {
            direction.x = 0;
        } else
        {
            direction.y = 0;
        }

        direction.Normalize();
        direction = new Vector2(direction.x * 4, direction.y * 4);
        if (Blocked(transform.position, direction))
        {
            return false;
        } else
        {
            transform.Translate(direction);
            return true;
        }
    }

    bool Blocked(Vector3 position, Vector2 direction)
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
                Yarn y = yarn.GetComponent<Yarn>();
                if (y && y.Move(direction))
                {
                    return false;
                } else
                {
                    return true;
                }
            }
        }

        return false;
    }
}
