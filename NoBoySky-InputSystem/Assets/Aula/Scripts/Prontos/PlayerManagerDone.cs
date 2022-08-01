using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManagerDone : MonoBehaviour
{
    public bool horizontalSplit = false;
    public Transform spawnPlayerOne, spawnPlayerTwo;
    public Texture playerTwoTexture;

    PlayerInputManager inputManager;
    List<PlayerInput> players = new List<PlayerInput>();

    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
    }

    void OnPlayerJoined(PlayerInput player)
    {
        players.Add(player);

        CarController carPlayer = player.GetComponent<CarController>();

        if(inputManager.playerCount > 1)
        {
            carPlayer.ChangeTexture(playerTwoTexture);
            carPlayer.ChangeSpawnPoint(spawnPlayerOne.position, spawnPlayerOne.rotation);

            if(horizontalSplit)
            {
                UpdateHorizontalSplit();
            }
        }
        else
        {
            carPlayer.ChangeSpawnPoint(spawnPlayerTwo.position, spawnPlayerTwo.rotation);
        }

    }

    void OnPlayerLeft(PlayerInput player)
    {
        players.Remove(player);
        player.DeactivateInput();

        if(inputManager.playerCount == 1 && horizontalSplit)
        {
            ReadjustCamera();
        }

        Debug.Log("Player Left!");
    }

    void ReadjustCamera()
    {
        players[0].camera.rect = new Rect(0f, 0f, 1f, 1f);
    }

    void UpdateHorizontalSplit()
    {
        inputManager.splitScreen = false;
        for (int i = 0; i < players.Count; i++)
        {
            AdjustPlayerCameraRect(players[i], i + 1);
        }
    }

    void AdjustPlayerCameraRect(PlayerInput player, int index)
    {
        Camera camera = player.camera;
        
        if (index == 1)
        {
            camera.rect = new Rect(0f, .5f, 1f, .5f);
        }
        else
        {
            camera.rect = new Rect(0f, 0f, 1f, .5f);
        }
    }

}
