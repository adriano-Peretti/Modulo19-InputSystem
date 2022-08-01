using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerType
{
    Character = 0,
    Nave
}

public class PlayerNaveManager : MonoBehaviour
{
    [HideInInspector]
    public static PlayerNaveManager instance = null;

    public CinemachineVirtualCameraBase characterCamera, naveCamera;
    public PlayerController playerController;
    public NaveController naveController;
    public Text textMessage;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        ChangePlayer(PlayerType.Nave);
    }

    public void ChangePlayer(PlayerType type)
    {
        switch(type)
        {
            case PlayerType.Nave:
                playerController.DeactivateCharacter();
                playerController.gameObject.SetActive(false);
                naveController.ActivateNave();
                characterCamera.enabled = false;
                naveCamera.enabled = true;
                break;

            case PlayerType.Character:
                naveController.DeactivateNave();
                playerController.gameObject.SetActive(true);
                playerController.ActivateCharacter(naveController.playerSpawnPoint.position);
                naveCamera.enabled = false;
                characterCamera.enabled = true;
                break;

            default:
                Debug.Log("Invalid Player Type");
                break;

        }
    }

    public void ShowMessage(string text)
    {
        textMessage.text = text;
    }

    public void HideMessage()
    {
        textMessage.text = "";
    }

}
