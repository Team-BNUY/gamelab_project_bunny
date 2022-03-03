using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

namespace Networking
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        private const string ROOM_SCENE_NAME = "3-Room";
        private const string DEFAULT_PLAYER_NAME = "Joe";
        private const string PLAYER_PREF_NAME_KEY = "PlayerName";

        [SerializeField] private TMP_InputField createInput;
        [SerializeField] private TMP_InputField joinInput;
        [SerializeField] private TMP_InputField nameInput;

        [SerializeField] private byte maxPlayersPerRoom = 4;

        private Button createButton;
        private Button joinButton;
        private string defaultName;

        void Start()
        {
            createButton = createInput.GetComponentInChildren<Button>();
            joinButton = joinInput.GetComponentInChildren<Button>();
            createButton.onClick.AddListener(CreateRoom);
            joinButton.onClick.AddListener(JoinRoom);
            nameInput.onValueChanged.AddListener(SetName);
            SetUpNameInput();
        }

        public void CreateRoom()
        {
            PhotonNetwork.NickName = defaultName;

            RoomOptions options = new RoomOptions();
            options.MaxPlayers = maxPlayersPerRoom;
            options.PublishUserId = true;
            options.BroadcastPropsChangeToAll = true;

            PhotonNetwork.CreateRoom(createInput.text, options, TypedLobby.Default);
        }

        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(joinInput.text);
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel(ROOM_SCENE_NAME);
        }

        private void SetUpNameInput()
        {
            if (PlayerPrefs.HasKey(PLAYER_PREF_NAME_KEY))
            {
                defaultName = PlayerPrefs.GetString(PLAYER_PREF_NAME_KEY);
            }
            else
            {
                defaultName = DEFAULT_PLAYER_NAME;
            }
            nameInput.text = defaultName;
            PhotonNetwork.NickName = defaultName;
        }

        /// <summary>
        /// Sets player name and saves to player prefs.
        /// Called by UI text input events.
        /// </summary>
        /// <param name="newName">new player name</param>
        public void SetName(string newName)
        {
            if (string.IsNullOrEmpty(newName)) return;

            PhotonNetwork.NickName = newName;
            PlayerPrefs.SetString(PLAYER_PREF_NAME_KEY, newName);
        }
    }
}
