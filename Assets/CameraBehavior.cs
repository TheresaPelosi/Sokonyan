using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    void FixedUpdate()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0) {
            Transform player = players[0].transform;
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
    }
}
