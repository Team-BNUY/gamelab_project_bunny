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
        private const byte maxPlayersPerRoom = 9;

        [SerializeField] private TMP_InputField _createInput;
        [SerializeField] private TMP_InputField _joinInput;
        [SerializeField] private TMP_InputField _nameInput;

        [SerializeField] private Button _createButton;
        [SerializeField] private Button _joinButton;
        private string _defaultName;

        void Start()
        {
            _createButton.onClick.AddListener(CreateRoom);
            _joinButton.onClick.AddListener(JoinRoom);
            _nameInput.onValueChanged.AddListener(SetName);
            SetUpNameInput();
        }

        public void CreateRoom()
        {
            PhotonNetwork.NickName = _defaultName;

            RoomOptions options = new RoomOptions();
            options.MaxPlayers = maxPlayersPerRoom;
            options.PublishUserId = true;
            options.BroadcastPropsChangeToAll = true;

            PhotonNetwork.CreateRoom(_createInput.text, options, TypedLobby.Default);
        }

        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(_joinInput.text);
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel(ROOM_SCENE_NAME);
        }

        private void SetUpNameInput()
        {
            if (PlayerPrefs.HasKey(PLAYER_PREF_NAME_KEY))
            {
                _defaultName = PlayerPrefs.GetString(PLAYER_PREF_NAME_KEY);
            }
            else
            {
                _defaultName = DEFAULT_PLAYER_NAME;
            }
            _nameInput.text = _defaultName;
            PhotonNetwork.NickName = _defaultName;
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
