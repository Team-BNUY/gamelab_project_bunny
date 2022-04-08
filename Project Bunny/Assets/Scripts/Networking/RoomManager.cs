using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using Player;
using System;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        private static RoomManager _instance;

        public static RoomManager Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<RoomManager>();
                }

                return _instance;
            }
        }

        private const string LOBBY_SCENE_NAME = "2-Lobby";
        private const string ARENA_SCENE_NAME = "4-Arena";

        [Header("Prefabs")]
        [SerializeField] private GameObject _snowballPrefab;
        [SerializeField] private GameObject _snowballBurst;
        

        [Header("Player Instantiation")]
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _playerCamera;
        [SerializeField] private Transform[] _playerSpawnPosition;
        private Hashtable _customProperties;

        [Space(10)]
        [Header("UI")]
        [SerializeField] private Button _leaveRoomBtn;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Image[] displayedScores;
        [SerializeField] private Image teamWhoWon;
        [SerializeField] private TMPro.TMP_Text[] displayedNames;
        [SerializeField] private TMPro.TMP_Text[] displayedWinCons;
        [SerializeField] private Sprite[] scoreSprites;
        [SerializeField] private Sprite blueWinSprite;
        [SerializeField] private Sprite redWinSprite;
        [SerializeField] private Sprite noContestSprite;
        [SerializeField] private GameObject firstRunWhiteboard;
        [SerializeField] private GameObject scoresWhiteboard;

        public GameObject SnowballPrefab => _snowballPrefab;
        public GameObject SnowballBurst => _snowballBurst;

        private NetworkStudentController _localStudentController;
        public NetworkStudentController LocalStudentController => _localStudentController;

        public bool isFirstRun = true;

        private void Start()
        {
            _customProperties = new Hashtable();

            /*if (PhotonNetwork.LocalPlayer.CustomProperties != null)
            { 
                _customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            }
            else _customProperties = new Hashtable();*/

            InitialiseUI();
            SpawnPlayer();

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
            if (_customProperties.ContainsKey(propertyName))
            {
                _customProperties[propertyName] = propertyValue;
            }
            else
            {
                _customProperties.Add(propertyName, propertyValue);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperties);
        }

        private void InitialiseUI()
        {
            loadingScreen.SetActive(false);

            if (ScoreManager.Instance)
            {
                isFirstRun = ScoreManager.Instance.isFirstMatch;

                foreach (Image img in displayedScores)
                {
                    img.gameObject.SetActive(false);
                }

                if (!isFirstRun)
                {
                    firstRunWhiteboard.SetActive(false);
                    scoresWhiteboard.SetActive(true);

                    teamWhoWon.sprite = ScoreManager.Instance.winningTeamCode switch
                    {
                        1 => blueWinSprite,
                        2 => redWinSprite,
                        _ => noContestSprite
                    };

                    var randomScoresIndex = new List<int>();
                    var maxLoops = 100;

                    while (randomScoresIndex.Count < displayedScores.Length)
                    {
                        maxLoops--;
                        if (maxLoops <= 0) break;
                        var rand = UnityEngine.Random.Range(0, ScoreManager.Instance.scores.Length);
                        if (!string.IsNullOrEmpty(ScoreManager.Instance.scores[rand]) && !randomScoresIndex.Contains(rand) && ScoreManager.Instance.scoreValues[rand] != 0)
                        {
                            randomScoresIndex.Add(rand);
                        }
                    }

                    for (int i = 0; i < randomScoresIndex.Count; i++)
                    {
                        displayedScores[i].gameObject.SetActive(true);
                        displayedScores[i].sprite = scoreSprites[randomScoresIndex[i]];
                        displayedNames[i].text = ScoreManager.Instance.scores[randomScoresIndex[i]];
                        displayedWinCons[i].text = GetWinConText(ScoreManager.Instance.scoreValues[randomScoresIndex[i]], randomScoresIndex[i]);
                    }
                }
                else
                {
                    firstRunWhiteboard.SetActive(true);
                    scoresWhiteboard.SetActive(false);
                    foreach (var img in displayedScores)
                    {
                        img.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                firstRunWhiteboard.SetActive(true);
                scoresWhiteboard.SetActive(false);
                foreach (var img in displayedScores)
                {
                    img.gameObject.SetActive(false);
                }
            }
        }

        private string GetWinConText(int score, int index)
        {
            // rebel - 0, bully - 1, hardWorker - 2, teachersPet - 3,
            // office - 4, glaceFolie - 5, shoveler - 6, avalance - 7
            switch (index)
            {
                case 0:
                    return $"Hit the teacher {score} times";
                case 1:
                    return $"Bullied others {score} times";
                case 2:
                    return $"Landed {score} hits";
                case 3:
                    return $"Only threw {score} snowballs";
                case 4:
                    return $"Got caught {score} times";
                case 5:
                    return $"Threw {score} ice balls";
                case 6:
                    return $"Dug out {score} snowballs";
                case 7:
                    return $"Got hit by {score} giant balls";
                default:
                    return string.Empty;
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
            var scoreManager = FindObjectOfType<ScoreManager>();
            var teamsManager = FindObjectOfType<PhotonTeamsManager>();

            /*Hashtable emptyTable = new Hashtable();
            PhotonNetwork.LocalPlayer.CustomProperties = emptyTable;*/

            if (scoreManager != null)
            {
                Destroy(scoreManager.gameObject);
            }

            if (teamsManager != null)
            {
                Destroy(teamsManager.gameObject);
            }

            SceneManager.LoadScene(LOBBY_SCENE_NAME);
        }

        private void SwapTeams(byte teamCode)
        {
            PhotonTeamExtensions.SwitchTeam(PhotonNetwork.LocalPlayer, teamCode);
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

        /*public void CorrectNumberOfJerseys()
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
        }*/

        private void SetAllPlayerSpawns()
        {
            var allStudents = FindObjectsOfType<NetworkStudentController>();

            for (var i = 0; i < allStudents.Length; i++)
            {
                if (allStudents[i])
                {
                    allStudents[i].transform.position = _playerSpawnPosition[i].position;
                }
            }
        }

        private void SpawnPlayer()
        {
            var player = PhotonNetwork.Instantiate(_playerPrefab.name, _playerSpawnPosition[0].position, Quaternion.identity).GetComponent<NetworkStudentController>();
            _localStudentController = player;
            player.PlayerID = PhotonNetwork.LocalPlayer.UserId;
            PhotonNetwork.LocalPlayer.TagObject = player;
            player.SetCamera(Instantiate(_playerCamera), 40f, 15f, false, 0.374f, 4f);


            if (isFirstRun)
            {
                player.transform.position = _playerSpawnPosition[PhotonNetwork.CurrentRoom.PlayerCount].position;
            }
            else
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Invoke(nameof(SetAllPlayerSpawns), 0.1f);
                }
                else {
                    if (_customProperties.ContainsKey("isReady"))
                        _customProperties["isReady"] = false;
                    else {
                        _customProperties.Add("isReady", false);
                    }
                    PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperties);
                }
            }


            var playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

            if (playerProperties.ContainsKey("hatIndex"))
            {
                player.photonView.RPC("SetHat", RpcTarget.AllBuffered, (int)playerProperties["hatIndex"]);
            }

            if (playerProperties.ContainsKey("hairIndex"))
            {
                if (playerProperties.ContainsKey("hairColorIndex"))
                {
                    player.photonView.RPC("SetHair", RpcTarget.AllBuffered, (int)playerProperties["hairIndex"], (int)playerProperties["hairColorIndex"]);
                }
                else
                {
                    player.photonView.RPC("SetHair", RpcTarget.AllBuffered, (int)playerProperties["hairIndex"], -1);
                }
            }

            if (playerProperties.ContainsKey("pantIndex"))
            {
                if (playerProperties.ContainsKey("pantColorIndex"))
                {
                    player.photonView.RPC("SetPants", RpcTarget.AllBuffered, (int)playerProperties["pantIndex"], (int)playerProperties["pantColorIndex"]);
                }
                else
                {
                    player.photonView.RPC("SetPants", RpcTarget.AllBuffered, (int)playerProperties["pantIndex"], -1);
                }
            }

            if (playerProperties.ContainsKey("coatIndex"))
            {
                if (playerProperties.ContainsKey("coatColorIndex"))
                {
                    player.photonView.RPC("SetCoat", RpcTarget.AllBuffered, (int)playerProperties["coatIndex"], (int)playerProperties["coatColorIndex"]);
                }
                else
                {
                    player.photonView.RPC("SetCoat", RpcTarget.AllBuffered, (int)playerProperties["coatIndex"], -1);
                }
            }

            if (playerProperties.ContainsKey("skinColorIndex"))
            {
                player.photonView.RPC("SetSkinColor", RpcTarget.AllBuffered, (int)playerProperties["skinColorIndex"]);
            }
            
            if (playerProperties.ContainsKey("shoesColorIndex"))
            {
                player.photonView.RPC("SetShoesColor", RpcTarget.AllBuffered, (int)playerProperties["shoesColorIndex"]);
            }
            
            player.RestoreTeamlessColors_RPC();    
            //CorrectNumberOfJerseys();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            if (!PhotonNetwork.IsMasterClient) return;

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
            if (newPlayer.TagObject == null) return;
            
            Destroy(((NetworkStudentController)newPlayer.TagObject).gameObject);
            _customProperties.Clear();
            newPlayer.SetCustomProperties(_customProperties);
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);


            if (changedProps.ContainsKey("isReady") && !targetPlayer.IsMasterClient)
            {
                _localStudentController.photonView.RPC("SyncIsReady", RpcTarget.AllBuffered, (bool)changedProps["isReady"], targetPlayer.UserId);
            }
        }
    }
}
