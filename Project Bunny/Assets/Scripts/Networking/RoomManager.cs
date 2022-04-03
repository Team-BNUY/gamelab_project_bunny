using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using System.Linq;
using Player;
using Photon.Realtime;
using System;

namespace Networking
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        private static RoomManager _instance;

        public static RoomManager Instance {
            get {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<RoomManager>();
                }

                return _instance;
            }
        }

        private const string LOBBY_SCENE_NAME = "2-Lobby";
        private const string ARENA_SCENE_NAME = "4-Arena";

        [Header("Player Instantiation")]
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _playerCamera;
        [SerializeField] private GameObject _playerSpawnPosition;
        private Hashtable _customProperties;

        [Space(10)]
        [Header("UI")]
        [SerializeField] private Button _leaveRoomBtn;
        [SerializeField] private GameObject loadingScreen;
        private List<PlayerTile> _tiles;

        void Start()
        {
            _tiles = new List<PlayerTile>();
            _customProperties = new Hashtable();

            SpawnPlayer();
            InitialiseUI();

            PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperties);
        }


        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            PhotonNetwork.DestroyPlayerObjects(newMasterClient);
            //Kick everyone from the room if the master client changed (too many bugs to deal with otherwise)
            PhotonNetwork.LeaveRoom();

        }

        private void InitialiseUI()
        {
            loadingScreen.SetActive(false);

            _leaveRoomBtn.onClick.AddListener(() =>
            {
                if (PhotonNetwork.LocalPlayer.GetPhotonTeam() != null)
                {
                    if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == "Blue")
                    {
                        BlueTeamTable.instance._view.RPC("SubtractTeamCount", RpcTarget.AllBuffered);
                        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();

                    }
                    else if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == "Red")
                    {
                        RedTeamTable.instance._view.RPC("SubtractTeamCount", RpcTarget.AllBuffered);
                        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
                    }
                }
                PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                PhotonNetwork.LeaveRoom();
            });
        }

        public override void OnLeftRoom()
        {
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            PhotonTeamsManager teamsManager = FindObjectOfType<PhotonTeamsManager>();

            if (scoreManager != null)
                GameObject.Destroy(scoreManager.gameObject);

            if (teamsManager != null)
                GameObject.Destroy(teamsManager.gameObject);

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

        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.LoadLevel(ARENA_SCENE_NAME);
            }

            if (PhotonNetwork.LevelLoadingProgress > 0 && PhotonNetwork.LevelLoadingProgress < 1)
            {
                loadingScreen.SetActive(true);
            }

        }

        private void SpawnPlayer()
        {
            NetworkStudentController player = PhotonNetwork.Instantiate(_playerPrefab.name, _playerSpawnPosition.transform.position, Quaternion.identity).GetComponent<NetworkStudentController>();
            player.PlayerID = PhotonNetwork.LocalPlayer.UserId;
            PhotonNetwork.LocalPlayer.TagObject = player;
            player.SetCamera(Instantiate(_playerCamera), 40f, 15f, false, 0.374f);
            
            player.RestoreTeamlessColors_RPC();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            base.OnPlayerEnteredRoom(newPlayer);

            base.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (newPlayer.GetPhotonTeam() != null)
            {
                if (newPlayer.GetPhotonTeam().Name == "Blue")
                {
                    BlueTeamTable.instance._view.RPC("SubtractTeamCount", RpcTarget.AllBuffered);
                    newPlayer.LeaveCurrentTeam();

                }
                else if (newPlayer.GetPhotonTeam().Name == "Red")
                {
                    RedTeamTable.instance._view.RPC("SubtractTeamCount", RpcTarget.AllBuffered);
                    newPlayer.LeaveCurrentTeam();
                }
            }

            base.OnPlayerLeftRoom(newPlayer);
            if (newPlayer.TagObject != null)
            {
                GameObject.Destroy(((NetworkStudentController)newPlayer.TagObject).gameObject);
                _customProperties.Clear();
                newPlayer.SetCustomProperties(_customProperties);
            }
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
        }
    }
}
