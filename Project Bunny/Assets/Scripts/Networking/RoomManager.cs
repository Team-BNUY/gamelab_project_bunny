using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using Player;
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

        private static bool _isFirstRun = true;
        private const string LobbySceneName = "2-Lobby";
        private const string ArenaSceneName = "4-Arena";

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
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private Image[] _displayedScores;
        [SerializeField] private Image _teamWhoWon;
        [SerializeField] private TMPro.TMP_Text[] _displayedNames;
        [SerializeField] private TMPro.TMP_Text[] _displayedWinCons;
        [SerializeField] private Sprite[] _scoreSprites;
        [SerializeField] private Sprite _blueWinSprite;
        [SerializeField] private Sprite _redWinSprite;
        [SerializeField] private Sprite _noContestSprite;
        [SerializeField] private GameObject _firstRunWhiteboard;
        [SerializeField] private GameObject _scoresWhiteboard;

        public GameObject SnowballPrefab => _snowballPrefab;
        public GameObject SnowballBurst => _snowballBurst;
        public NetworkStudentController LocalStudentController { get; private set; }

        private void Start()
        {
            _customProperties = new Hashtable();

            InitialiseUI();
            SpawnPlayer();

            PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperties);
        }


        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            PhotonNetwork.DestroyPlayerObjects(newMasterClient);
            
            // Kick everyone from the room if the master client changed (too many bugs to deal with otherwise)
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
            _loadingScreen.SetActive(false);

            if (ScoreManager.Instance)
            {
                _isFirstRun = ScoreManager.Instance.isFirstMatch;

                foreach (var img in _displayedScores)
                {
                    img.gameObject.SetActive(false);
                }

                if (!_isFirstRun)
                {
                    _firstRunWhiteboard.SetActive(false);
                    _scoresWhiteboard.SetActive(true);

                    _teamWhoWon.sprite = ScoreManager.Instance.winningTeamCode switch
                    {
                        1 => _blueWinSprite,
                        2 => _redWinSprite,
                        _ => _noContestSprite
                    };

                    var randomScoresIndex = new List<int>();
                    var maxLoops = 100;
                    var randomNumberIndex = 0;
                    while (randomScoresIndex.Count < _displayedScores.Length)
                    {
                        maxLoops--;
                        
                        if (maxLoops <= 0) break;
                        
                        var rand = ScoreManager.Instance.RandomNumbers[randomNumberIndex];
                        randomNumberIndex++;
                        
                        if (!string.IsNullOrEmpty(ScoreManager.Instance.scores[rand]) && !randomScoresIndex.Contains(rand) && ScoreManager.Instance.scoreValues[rand] != 0)
                        {
                            randomScoresIndex.Add(rand);
                        }
                    }

                    for (var i = 0; i < randomScoresIndex.Count; i++)
                    {
                        _displayedScores[i].gameObject.SetActive(true);
                        _displayedScores[i].sprite = _scoreSprites[randomScoresIndex[i]];
                        _displayedNames[i].text = ScoreManager.Instance.scores[randomScoresIndex[i]];
                        _displayedWinCons[i].text = GetWinConText(ScoreManager.Instance.scoreValues[randomScoresIndex[i]], randomScoresIndex[i]);
                    }
                }
                else
                {
                    _firstRunWhiteboard.SetActive(true);
                    _scoresWhiteboard.SetActive(false);
                    foreach (var img in _displayedScores)
                    {
                        img.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                _firstRunWhiteboard.SetActive(true);
                _scoresWhiteboard.SetActive(false);
                foreach (var img in _displayedScores)
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

            if (scoreManager != null)
            {
                Destroy(scoreManager.gameObject);
            }

            if (teamsManager != null)
            {
                Destroy(teamsManager.gameObject);
            }

            SceneManager.LoadScene(LobbySceneName);
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
                PhotonNetwork.LoadLevel(ArenaSceneName);
            }

            if (PhotonNetwork.LevelLoadingProgress > 0 && PhotonNetwork.LevelLoadingProgress < 1)
            {
                _loadingScreen.SetActive(true);
            }
        }

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
            LocalStudentController = player;
            player.PlayerID = PhotonNetwork.LocalPlayer.UserId;
            PhotonNetwork.LocalPlayer.TagObject = player;
            player.SetCamera(Instantiate(_playerCamera), 40f, 15f, false, 0.374f, 4f);

            if (_isFirstRun)
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
            
            var playerCustomization = LocalStudentController.PlayerCustomization;
            playerCustomization.SetVisualCustomProperties();
            playerCustomization.RestoreTeamlessColors();
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
            
            if (!changedProps.ContainsKey("isReady") || targetPlayer.IsMasterClient) return;
            
            LocalStudentController.SyncIsReady((bool)changedProps["isReady"], targetPlayer.UserId);
        }
    }
}
