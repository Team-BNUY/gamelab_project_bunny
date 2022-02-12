using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Button startGameButton;

    private const string GAME_SCENE_NAME = "Game";

    void Start()
    {
        startGameButton.gameObject.SetActive(NetworkServer.active);
        startGameButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        Debug.Log("POG");
        NetworkManager.singleton.ServerChangeScene(GAME_SCENE_NAME);
    }
}
