using UnityEngine;
using Photon.Pun;
using Player;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using System.Linq;

namespace Networking
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        private const string LOBBY_SCENE_NAME = "2-Lobby";
        private const string ARENA_SCENE_NAME = "4-Arena";

        [Header("Player Instantiation")]
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _playerCamera;
        private Hashtable _customProperties;

        [Space(10)]
        [Header("UI")]
        [SerializeField] private Canvas _hostCanvas;
        [SerializeField] private Canvas _nonHostCanvas;
        [SerializeField] private Button _startGameBtn;
        [SerializeField] private Button _readyUpBtn;
        [SerializeField] private Button _redJerseyBtn;
        [SerializeField] private Button _blueJerseyBtn;
        [SerializeField] private Button _leaveRoomBtn;
        [SerializeField] private GameObject playerTile;
        [SerializeField] private Transform playerTileParent;
        private List<PlayerTile> tiles;

        void Start()
        {
            PhotonTeamsManager.PlayerJoinedTeam += OnPlayerJoinedTeam;
            tiles = new List<PlayerTile>();
            _customProperties = new Hashtable();

            SpawnPlayer();
            InitialiseUI();

            if (PhotonTeamsManager.Instance.GetTeamMembersCount(1) <= PhotonTeamsManager.Instance.GetTeamMembersCount(2))
            {
                PhotonNetwork.LocalPlayer.JoinTeam(1);
            }
            else
            {
                PhotonNetwork.LocalPlayer.JoinTeam(2);
            }

            _startGameBtn.interactable = false;

            foreach (KeyValuePair<int, Photon.Realtime.Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                if (tiles.Any(x => x.player == player.Value)) continue;
                AddPlayerTile(player.Value);
            }
        }

        private void AddPlayerTile(Photon.Realtime.Player player)
        {
            PlayerTile tile = GameObject.Instantiate(playerTile, playerTileParent).GetComponent<PlayerTile>();
            tile.SetPlayer(player);
            tiles.Add(tile);
            playerTileParent.GetComponentsInChildren<PlayerTile>().OrderBy(x => x.player.IsMasterClient).ThenBy(x => x.player.NickName);
        }

        private void RemovePlayerTile(Photon.Realtime.Player player)
        {
            PlayerTile tile = tiles.FirstOrDefault(x => x.player == player);
            tiles.Remove(tile);
            if (player.TagObject != null)
            {
                GameObject.Destroy(((NetworkStudentController)player.TagObject).gameObject);
            }
            GameObject.Destroy(tile.gameObject);
        }

        private void InitialiseUI()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                _customProperties.Add("ready", true);
                _nonHostCanvas.gameObject.SetActive(false);
                _hostCanvas.gameObject.SetActive(true);
            }
            else
            {
                _customProperties.Add("ready", false);
                _nonHostCanvas.gameObject.SetActive(true);
                _hostCanvas.gameObject.SetActive(false);
            }

            _startGameBtn.onClick.AddListener(StartGame);
            _readyUpBtn.onClick.AddListener(ReadyUp);

            _blueJerseyBtn.onClick.AddListener(() => SwapTeams(1));
            _redJerseyBtn.onClick.AddListener(() => SwapTeams(2));
            _leaveRoomBtn.onClick.AddListener(() => PhotonNetwork.LeaveRoom());

            PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperties);
        }

        public override void OnLeftRoom()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(LOBBY_SCENE_NAME);
        }

        private void SwapTeams(byte teamCode)
        {
            PhotonTeamExtensions.SwitchTeam(PhotonNetwork.LocalPlayer, teamCode);
        }

        private void ReadyUp()
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("ready"))
            {
                _customProperties["ready"] = !((bool)PhotonNetwork.LocalPlayer.CustomProperties["ready"]);
            }
            else
            {
                _customProperties["ready"] = true;
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperties);
        }

        private void StartGame()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(ARENA_SCENE_NAME);
        }

        private void SpawnPlayer()
        {
            NetworkStudentController player = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<NetworkStudentController>();
            PhotonNetwork.LocalPlayer.TagObject = player;
            player.SetCamera(GameObject.Instantiate(_playerCamera));
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            if (!tiles.Any(x => x.player == newPlayer))
            {
                AddPlayerTile(newPlayer);
            }

            _startGameBtn.interactable = (PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.PlayerList.Length >= 2);
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
        {
            base.OnPlayerLeftRoom(newPlayer);
            if (tiles.Any(x => x.player == newPlayer))
            {
                RemovePlayerTile(newPlayer);
            }

            _startGameBtn.interactable = (PhotonNetwork.LocalPlayer.IsMasterClient
                && PhotonNetwork.PlayerList.Length >= 2);
        }

        private void OnPlayerJoinedTeam(Photon.Realtime.Player player, PhotonTeam team)
        {
            PlayerTile tile = tiles.FirstOrDefault(x => x.player == player);

            if (tile != null)
            {
                tile.SetTeamIndicator(team.Code);
            }
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            if (targetPlayer != null)
            {
                tiles.FirstOrDefault(x => x.player == targetPlayer).UpdateView(changedProps);
            }
        }
    }
}
