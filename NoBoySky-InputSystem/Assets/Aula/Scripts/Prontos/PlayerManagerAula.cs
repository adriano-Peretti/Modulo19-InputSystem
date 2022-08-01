using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManagerAula : MonoBehaviour
{
    public Transform spawnPoint_1, spawnPoint_2;

    PlayerInputManager manager;

    private void Awake()
    {
        manager = GetComponent<PlayerInputManager>();
    }

    void OnPlayerJoined(PlayerInput player)
    {
        if(manager.playerCount == 1)
        {
            player.transform.position = spawnPoint_1.position;
            player.transform.rotation = spawnPoint_1.rotation;
        }
        else
        {
            player.transform.position = spawnPoint_2.position;
            player.transform.rotation = spawnPoint_2.rotation;
        }
    }
}
