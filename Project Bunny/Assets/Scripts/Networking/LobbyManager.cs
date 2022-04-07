using System.Collections;
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
        private const string DEFAULT_PLAYER_NAME = "My Name";
        private const string PLAYER_PREF_NAME_KEY = "PlayerName";
        private const byte maxPlayersPerRoom = 9;

        [Header("Login UI")]
        [SerializeField] private TMP_InputField _createInput;
        [SerializeField] private TMP_InputField _joinInput;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _leaveButton;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _loginAudioSource;
        [SerializeField] private AudioClip _writingSoundClip;
        private string _defaultName;

        [Header("Singletons")]
        [SerializeField] private GameObject _scoreManager;
        [SerializeField] private GameObject _photonTeamsManager;

        void Start()
        {
            _createButton.onClick.AddListener(CreateRoom);
            _joinButton.onClick.AddListener(JoinRoom);
            _nameInput.onValueChanged.AddListener(SetName);
            _leaveButton.onClick.AddListener(LeaveGame);
            SetUpNameInput();
        }

        public void CreateRoom()
        {
            StartCoroutine(StartCreatingRoom());
        }

        private IEnumerator StartCreatingRoom()
        {
            SetWrittingSound();
            yield return new WaitForSeconds(0.3f);
            
            PhotonNetwork.NickName = _defaultName;

            var options = new RoomOptions
            {
                MaxPlayers = maxPlayersPerRoom,
                PublishUserId = true,
                BroadcastPropsChangeToAll = true
            };

            PhotonNetwork.CreateRoom(_createInput.text, options, TypedLobby.Default);
        }

        public void JoinRoom()
        {
            StartCoroutine(StartJoiningRoom());
        }

        private IEnumerator StartJoiningRoom()
        {
            SetWrittingSound();
            yield return new WaitForSeconds(0.3f);
            PhotonNetwork.JoinRoom(_joinInput.text);
        }

        public override void OnJoinedRoom()
        {
            Instantiate(_photonTeamsManager);

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Instantiate(_scoreManager.name, Vector3.zero, Quaternion.identity);


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

        private void SetWrittingSound()
        {
            _loginAudioSource.playOnAwake = false;
            _loginAudioSource.loop = false;
            _loginAudioSource.clip = _writingSoundClip;
            _loginAudioSource.Play();
        }

        private void LeaveGame()
        {
            PhotonNetwork.LeaveLobby();
            Application.Quit();
        }
    }
}
