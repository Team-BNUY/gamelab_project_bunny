using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public const string GAME_SCENE_NAME = "NetworkPlayground";

    [SerializeField] private TMP_InputField createInput;
    [SerializeField] private TMP_InputField joinInput;

    private Button createButton;
    private Button joinButton;

    void Start()
    {
        createButton = createInput.GetComponentInChildren<Button>();
        joinButton = joinInput.GetComponentInChildren<Button>();
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);

    }

    public void CreateRoom() {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom() {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom() {
        PhotonNetwork.LoadLevel(GAME_SCENE_NAME);
    }
}
