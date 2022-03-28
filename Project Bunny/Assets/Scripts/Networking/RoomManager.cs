using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using System.Linq;
using Player;

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
        [SerializeField] private GameObject _playerTile;
        [SerializeField] private Transform _playerTileParent;
        private List<PlayerTile> _tiles;

        void Start()
        {
            _tiles = new List<PlayerTile>();
            _customProperties = new Hashtable();

            SpawnPlayer();
            InitialiseUI();
            PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperties);

            /*PhotonTeamsManager.PlayerJoinedTeam += OnPlayerJoinedTeam;
            if (PhotonTeamsManager.Instance.GetTeamMembersCount(1) <= PhotonTeamsManager.Instance.GetTeamMembersCount(2))
            {
                PhotonNetwork.LocalPlayer.JoinTeam(1);
                BlueTeamTable.instance.AddTeamCount();
            }
            else
            {
                PhotonNetwork.LocalPlayer.JoinTeam(2);
            }*/

            // _startGameBtn.interactable = false;

            foreach (KeyValuePair<int, Photon.Realtime.Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                if (_tiles.Any(x => x.player == player.Value)) continue;
                AddPlayerTile(player.Value);
            }
        }

        private void AddPlayerTile(Photon.Realtime.Player player)
        {
            PlayerTile tile = Instantiate(_playerTile, _playerTileParent).GetComponent<PlayerTile>();
            tile.SetPlayer(player);
            _tiles.Add(tile);
            _playerTileParent.GetComponentsInChildren<PlayerTile>().OrderBy(x => x.player.IsMasterClient).ThenBy(x => x.player.NickName);
        }

        private void RemovePlayerTile(Photon.Realtime.Player player)
        {
            PlayerTile tile = _tiles.FirstOrDefault(x => x.player == player);
            _tiles.Remove(tile);
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
                
            _leaveRoomBtn.onClick.AddListener(() => PhotonNetwork.LeaveRoom());
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
            player.PlayerID = PhotonNetwork.LocalPlayer.UserId;
            PhotonNetwork.LocalPlayer.TagObject = player;
            player.SetCamera(Instantiate(_playerCamera), 40f, 15f);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            if (!_tiles.Any(x => x.player == newPlayer))
            {
                AddPlayerTile(newPlayer);
            }

            _startGameBtn.interactable = (PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.PlayerList.Length >= 2);
            
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            if (newPlayer.GetPhotonTeam() != null)
            {
                if(newPlayer.GetPhotonTeam().Name == "Blue")
                {
                    BlueTeamTable.instance._view.RPC("SubtractTeamCount", RpcTarget.AllBuffered);
                    newPlayer.LeaveCurrentTeam();
                }
                else if(newPlayer.GetPhotonTeam().Name == "Red")
                {
                    RedTeamTable.instance._view.RPC("SubtractTeamCount", RpcTarget.AllBuffered);
                    newPlayer.LeaveCurrentTeam();
                }
            }
            
            base.OnPlayerLeftRoom(newPlayer);
            if (_tiles.Any(x => x.player == newPlayer))
            {
                RemovePlayerTile(newPlayer);
            }
            if (newPlayer.TagObject != null)
            {
                GameObject.Destroy(((NetworkStudentController)newPlayer.TagObject).gameObject);
                _customProperties.Clear();
                newPlayer.SetCustomProperties(_customProperties);
            }

            _startGameBtn.interactable = (PhotonNetwork.LocalPlayer.IsMasterClient
                && PhotonNetwork.PlayerList.Length >= 2);
            
        }

        private void OnPlayerJoinedTeam(Photon.Realtime.Player player, PhotonTeam team)
        {
            PlayerTile tile = _tiles.FirstOrDefault(x => x.player == player);

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
                _tiles.FirstOrDefault(x => x.player == targetPlayer).UpdateView(changedProps);
            }
        }
    }
}
