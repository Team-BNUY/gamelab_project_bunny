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
using Newtonsoft.Json;

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
        [SerializeField] private Image[] displayedScores;
        [SerializeField] private Image teamWhoWon;
        [SerializeField] private TMPro.TMP_Text[] displayedNames;
        [SerializeField] private Sprite[] scoreSprites;
        private List<PlayerTile> _tiles;
        private NetworkStudentController _localStudentController;
        public NetworkStudentController LocalStudentController => _localStudentController;

        void Start()
        {
            _tiles = new List<PlayerTile>();

            if (PhotonNetwork.LocalPlayer.CustomProperties != null)
            { 
                _customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            }
            else _customProperties = new Hashtable();

            SpawnPlayer();
            InitialiseUI();


            //Code that automatically removes the appropriate number of jerseys from the table when
            //we come back to the Classroom after a match.
            //Will Uncomment if needed. Right now everyone loses their teams and team colors when they come back from a match.

           
            

            PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperties);
        }


        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            PhotonNetwork.DestroyPlayerObjects(newMasterClient);
            //Kick everyone from the room if the master client changed (too many bugs to deal with otherwise)
            PhotonNetwork.LeaveRoom();

        }

        public void SetCustomProperty(string propertyName, int propertyValue)
        {
            if (_customProperties.ContainsKey(PhotonNetwork.LocalPlayer.UserId+propertyName))
            {
                _customProperties[PhotonNetwork.LocalPlayer.UserId+propertyName] = propertyValue;
            }
            else
            {
                _customProperties.Add(PhotonNetwork.LocalPlayer.UserId+propertyName, propertyValue);
            }
            
            PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperties);
        }

        private void InitialiseUI()
        {
            loadingScreen.SetActive(false);

            if (ScoreManager.Instance)
            {
                teamWhoWon.gameObject.SetActive(false);
                foreach (Image img in displayedScores)
                {
                    img.gameObject.SetActive(false);
                }

                if (!ScoreManager.Instance.isFirstMatch)
                {
                    teamWhoWon.gameObject.SetActive(true);
                    if (ScoreManager.Instance.winningTeamCode == 1)
                        teamWhoWon.color = Color.blue;
                    else if (ScoreManager.Instance.winningTeamCode == 2)
                        teamWhoWon.color = Color.red;
                    else
                        teamWhoWon.gameObject.SetActive(false);

                    int countedScores = 0;
                    for (int i = 0; i < ScoreManager.Instance.scores.Length; i++) {
                        if (!String.IsNullOrEmpty(ScoreManager.Instance.scores[i]) && countedScores <= 4) {
                            displayedScores[countedScores].gameObject.SetActive(true);
                            displayedScores[countedScores].sprite = scoreSprites[i];
                            displayedNames[countedScores].text = ScoreManager.Instance.scores[i];
                            countedScores++;
                        }
                    }
                }
                else
                {
                    foreach (Image img in displayedScores)
                    {
                        img.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                foreach (Image img in displayedScores)
                {
                    img.gameObject.SetActive(false);
                }
            }


            if (ScoreManager.Instance)
            {
                string scores = "";
                scores += "Rebel: " + ScoreManager.Instance.rebel;
                scores += "\nBully: " + ScoreManager.Instance.bully;
                scores += "\nHard Worker: " + ScoreManager.Instance.hardWorker;
                scores += "\nTeacher's Pet: " + ScoreManager.Instance.teachersPet;
                scores += "\nMeet In My Office: " + ScoreManager.Instance.meetMeInMyOffice;
                scores += "\nGlace Folie: " + ScoreManager.Instance.glaceFolie;
                scores += "\nShoveler: " + ScoreManager.Instance.shoveler;
                scores += "\nAvalanche: " + ScoreManager.Instance.avalanche;
            }
        }

        public void PlayerLeaveRoom()
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
        }

        public override void OnLeftRoom()
        {
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            PhotonTeamsManager teamsManager = FindObjectOfType<PhotonTeamsManager>();

            /*Hashtable emptyTable = new Hashtable();
            PhotonNetwork.LocalPlayer.CustomProperties = emptyTable;*/

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

        public void CorrectNumberOfJerseys()
        {
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam() == null) return;
            
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == "Blue")
            {
                BlueTeamTable.instance.AddTeamCount_RPC();
            }
            else if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == "Red")
            {
                RedTeamTable.instance.AddTeamCount_RPC();
            }
        }

        private void SpawnPlayer()
        {
            NetworkStudentController player = PhotonNetwork.Instantiate(_playerPrefab.name, _playerSpawnPosition.transform.position, Quaternion.identity).GetComponent<NetworkStudentController>();
            _localStudentController = player;
            player.PlayerID = PhotonNetwork.LocalPlayer.UserId;
            PhotonNetwork.LocalPlayer.TagObject = player;
            player.SetCamera(Instantiate(_playerCamera), 40f, 15f, false, 0.374f);

            Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

        if (playerProperties.ContainsKey(PhotonNetwork.LocalPlayer.UserId+"hatIndex"))
        {
            player.photonView.RPC("SetHat", RpcTarget.AllBuffered, (int)playerProperties[PhotonNetwork.LocalPlayer.UserId+"hatIndex"]);
        }

        if (playerProperties.ContainsKey(PhotonNetwork.LocalPlayer.UserId+"hairIndex"))
        {
            if (playerProperties.ContainsKey(PhotonNetwork.LocalPlayer.UserId+"hairColorIndex"))
            {
                player.photonView.RPC("SetHair", RpcTarget.AllBuffered, (int)playerProperties[PhotonNetwork.LocalPlayer.UserId+"hairIndex"], (int)playerProperties[PhotonNetwork.LocalPlayer.UserId+"hairColorIndex"]);
            }
            else
            {
                player.photonView.RPC("SetHair", RpcTarget.AllBuffered, (int)playerProperties[PhotonNetwork.LocalPlayer.UserId+"hairIndex"], 0);
            }
        }

        if (playerProperties.ContainsKey(PhotonNetwork.LocalPlayer.UserId+"pantIndex"))
        {
            if (playerProperties.ContainsKey(PhotonNetwork.LocalPlayer.UserId+"pantColorIndex"))
            {
                player.photonView.RPC("SetPants", RpcTarget.AllBuffered, (int)playerProperties[PhotonNetwork.LocalPlayer.UserId+"pantIndex"], (int)playerProperties[PhotonNetwork.LocalPlayer.UserId+"pantColorIndex"]);
            }
            else
            {
                player.photonView.RPC("SetPants", RpcTarget.AllBuffered, (int)playerProperties[PhotonNetwork.LocalPlayer.UserId+"pantIndex"], 0);
            }
        }

        if (playerProperties.ContainsKey(PhotonNetwork.LocalPlayer.UserId+"coatIndex"))
        {
            if (playerProperties.ContainsKey(PhotonNetwork.LocalPlayer.UserId+"coatColorIndex"))
            {
                player.photonView.RPC("SetCoat", RpcTarget.AllBuffered, (int)playerProperties[PhotonNetwork.LocalPlayer.UserId+"coatIndex"], (int)playerProperties[PhotonNetwork.LocalPlayer.UserId+"coatColorIndex"]);
            }
            else
            {
                player.photonView.RPC("SetCoat", RpcTarget.AllBuffered, (int)playerProperties[PhotonNetwork.LocalPlayer.UserId+"coatIndex"], 0);
            }
        }

        if (playerProperties.ContainsKey(PhotonNetwork.LocalPlayer.UserId+"skinColorIndex"))
        {
            player.photonView.RPC("SetSkinColor", RpcTarget.AllBuffered, (int)playerProperties[PhotonNetwork.LocalPlayer.UserId+"skinColorIndex"]);
        }
        player.UpdateTeamColorVisuals();    
        CorrectNumberOfJerseys();
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
