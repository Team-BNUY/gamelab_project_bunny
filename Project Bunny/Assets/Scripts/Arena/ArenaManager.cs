using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using AI.Agents;
using Cinemachine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Arena
{
    public class ArenaManager : MonoBehaviourPunCallbacks
    {
        private static ArenaManager _instance;

        public static ArenaManager Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ArenaManager>();
                }

                return _instance;
            }
        }

        [Header("Gameplay References")]
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _playerCamera;
        [SerializeField] private GameObject _snowballPrefab;
        [SerializeField] private GameObject _iceballPrefab;
        [SerializeField] private GameObject _snowballBurst;
        [SerializeField] private GameObject _iceballBurst;
        [SerializeField] private GameObject _giantRollballPrefab;
        [SerializeField] private GameObject _giantRollballBurst;
        [SerializeField] private GameObject _snowmanPrefab;
        [SerializeField] private GameObject _cannonBall;
        [SerializeField] private NetworkCannon _cannon;
        private NetworkStudentController _localStudentController;
        
        [Header("Team References")]        
        [SerializeField] private Sprite[] _teamShirts = new Sprite[3];
        [SerializeField] private TeamWall[] _teamWalls;
        [SerializeField] private Transform[] _redSpawns;
        [SerializeField] private Transform[] _blueSpawns;
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private Image _leadingTeamShirt;
        
        [Header("AI")]
        [SerializeField] private LayerMask _studentLayer;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private int _gangMaximumSize;
        [SerializeField] private List<ActionSpot> _actionSpots;
        [SerializeField] private Teacher _teacherPrefab;
        [SerializeField] private Transform _teacherSpawn;
        [SerializeField] private Waypoint[] _teacherWaypoints;
        [SerializeField] private CinemachineVirtualCamera _teacherVirtualCamera;
        [SerializeField] private ExclamationMark _exclamationMark;
        private NetworkStudentController[] _allPlayers;
        private List<Gang> _gangs = new List<Gang>();

        [Header("Timers")]
        [SerializeField] private int _gameDuration = 7 * 60;
        [SerializeField] private float _teacherSpawnTime;
        [SerializeField] private float _teacherPreparationTime = 4f;
        [SerializeField] private TMP_Text timerDisplay;
        [SerializeField] private bool hasTimerStarted;
        [SerializeField] private float snowmanTimer;
        
        private int _timeElapsed;
        private int _oldTimeElapsed;
        private double _startTime;
        private bool _returnToLobbyHasRun;
        private int _readyPlayers;
        private bool _gameOver;
        private bool _teacherSpawned;
        private const string StartTimeKey = "StartTime";
        private const string ReadyKey = "isready";
        
        // Scene names
        private const string LobbySceneName = "2-Lobby";
        private const string RoomSceneName = "3-Room";

        [Header("Audio")]
        [SerializeField] private AudioClip _music;
        [SerializeField] private AudioClip _teacherSpawnSound;
        [SerializeField] private AudioClip _schoolBellSound;

        #region Properties

        public GameObject SnowballPrefab => _snowballPrefab;
        public GameObject IceballPrefab => _iceballPrefab;
        public GameObject SnowballBurst => _snowballBurst;
        public GameObject IceballBurst => _iceballBurst;
        public GameObject GiantRollballBurst => _giantRollballBurst;
        public GameObject GiantRollballPrefab => _giantRollballPrefab;
        public GameObject CannonBall => _cannonBall;
        public GameObject SnowmanPrefab => _snowmanPrefab;
        public NetworkCannon Cannon => _cannon;
        public NetworkStudentController[] AllPlayers => _allPlayers;
        public List<Gang> Gangs => _gangs;
        public LayerMask StudentLayer => _studentLayer;
        public LayerMask GroundLayer => _groundLayer;
        public LayerMask ObstacleLayer => _obstacleLayer;
        public int GangMaximumSize => _gangMaximumSize;
        public List<ActionSpot> ActionSpots => _actionSpots;
        public float SnowmanTimer => snowmanTimer;
        public Waypoint[] TeacherWaypoints => _teacherWaypoints;
        public float TeacherPreparationTime => _teacherPreparationTime;
        public CinemachineVirtualCamera TeacherVirtualCamera => _teacherVirtualCamera;
        public ExclamationMark ExclamationMark => _exclamationMark;

        #endregion

        #region Callbacks

        private void Awake()
        {
            _loadingScreen.SetActive(true);
            _leadingTeamShirt.sprite = _teamShirts[0];
        
            if (!PhotonNetwork.IsMasterClient) return;
        
            _leadingTeamShirt.sprite = _teamShirts[0];
            _gangs.Clear();
        }

        private void Start()
        {
            SetIsReady(false);
            _loadingScreen.SetActive(true);
            SpawnPlayer();

            ScoreManager.Instance.ClearPropertyCounters();

            SetIsReady(true);
        }

        private void FixedUpdate()
        {
            UpdateTimer();
        }
        
        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            //Kick everyone from the room if the master client changed (too many bugs to deal with otherwise)
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            var scoreManager = FindObjectOfType<ScoreManager>();
            var teamsManager = FindObjectOfType<PhotonTeamsManager>();

            PhotonNetwork.LocalPlayer.LeaveCurrentTeam();

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

            UnityEngine.SceneManagement.SceneManager.LoadScene(LobbySceneName);
        }
        
        public override void OnRoomPropertiesUpdate(Hashtable changedProps)
        {
            base.OnRoomPropertiesUpdate(changedProps);
            if (changedProps.ContainsKey(StartTimeKey) && !PhotonNetwork.IsMasterClient)
            {
                _startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties[StartTimeKey].ToString());
                hasTimerStarted = true;
            }

            if (!changedProps.ContainsKey("deaths1") && !changedProps.ContainsKey("deaths2")) return;
        
            var deaths1 = (int)changedProps["deaths1"];
            var deaths2 = (int)changedProps["deaths2"];
            if (deaths1 == deaths2)
            {
                SetLeadingShirt(0);
            }
            else
            {
                var winningTeam = (int)changedProps["deaths1"] < (int)changedProps["deaths2"] ? 1 : 2;
                SetLeadingShirt(winningTeam);
            }
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(ReadyKey))
            {
                if ((bool)changedProps[ReadyKey])
                    _readyPlayers++;
            }

            if (_readyPlayers != PhotonNetwork.CurrentRoom.PlayerCount) return;
        
            _allPlayers = Array.Empty<NetworkStudentController>();
            Invoke(nameof(GetAllPlayers), 0.1f);
            StartMatch();
        }

        #endregion
        
        private void StartMatch()
        {
            _gameOver = false;
            InjectInitialStudentStates();
            _loadingScreen.SetActive(false);
            _localStudentController.IsFrozen = false;
            StartTimer();

            AudioManager.Instance.Play(_music, 0.15f, true);
        }

        private void UpdateTimer()
        {
            if (!hasTimerStarted) return;

            _oldTimeElapsed = _timeElapsed;
            _timeElapsed = (int)(PhotonNetwork.Time - _startTime);

            if (_timeElapsed <= 0)
            {
                _timeElapsed = 0;
            }

            if (_timeElapsed > _oldTimeElapsed)
            {
                var timeLeft = _gameDuration - _timeElapsed;

                //Integer increment, update UI
                if (timeLeft > 60)
                {
                    timerDisplay.text = $"{timeLeft / 60}:{timeLeft % 60:00}";
                }
                else
                {
                    timerDisplay.text = timeLeft >= 0f ? timeLeft.ToString() : "0";
                }
            }

            if (_timeElapsed > _teacherSpawnTime && !_teacherSpawned)
            {
                _teacherSpawned = true;
                SpawnTeacher();
            }

            if (_timeElapsed < _gameDuration || _gameOver) return;
            _gameOver = true;
        
            AudioManager.Instance.PlayOneShot(_schoolBellSound, 0.5f);
            Invoke(nameof(ReturnToLobby), 2f);
        }

        private void InjectInitialStudentStates()
        {
            foreach (var gang in _gangs)
            {
                if (gang.Occupied) continue;

                foreach (var member in gang.Members)
                {
                    member.BeliefStates.AddState("curiousAboutOthers", 1);
                }
            }
        }

        private void SetIsReady(bool isReady)
        {
            var props = PhotonNetwork.LocalPlayer.CustomProperties;
            if (props.ContainsKey(ReadyKey))
            {
                props[ReadyKey] = isReady;
            }
            else
            {
                props.Add(ReadyKey, isReady);
            }
            
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    
        private void ReturnToLobby()
        {
            if (_returnToLobbyHasRun) return;
            _returnToLobbyHasRun = true;
        
            PhotonNetwork.LocalPlayer.LeaveCurrentTeam();

            if (!PhotonNetwork.IsMasterClient) return;
        
            ScoreManager.Instance.CalculateScore();
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.LoadLevel(RoomSceneName);
        }

        private void StartTimer()
        {
            timerDisplay.text = $"{(_gameDuration - _timeElapsed) / 60}:{(_gameDuration - _timeElapsed) % 60:00}";

            if (!PhotonNetwork.IsMasterClient) return;
        
            var myHashTable = new Hashtable();
            _startTime = PhotonNetwork.Time;
            hasTimerStarted = true;
            myHashTable.Add(StartTimeKey, _startTime);
            myHashTable.Add("deaths1", 0);
            myHashTable.Add("deaths2", 0);

            PhotonNetwork.CurrentRoom.SetCustomProperties(myHashTable);
        }

        private void SpawnPlayer()
        {
            var player = PhotonNetwork.Instantiate(_playerPrefab.name, GetPlayerSpawnPoint(PhotonNetwork.LocalPlayer.GetPhotonTeam().Code), Quaternion.identity).GetComponent<NetworkStudentController>();
            
            if (!player.photonView.IsMine) return;
            
            player.PlayerID = PhotonNetwork.LocalPlayer.UserId;
            player.TeamID = PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
            PhotonNetwork.LocalPlayer.TagObject = player;
            player.IsFrozen = true;
            _localStudentController = player;
            player.SetCamera(Instantiate(_playerCamera), 60f, 25f, true, 0.7f, 5f);
            player.photonView.RPC("SyncPlayerInfo", RpcTarget.AllBuffered, player.PlayerID, player.TeamID);

            foreach (var wall in _teamWalls)
            {
                if (_localStudentController.TeamID == wall._allowedTeam)
                {
                    wall._collider.enabled = false;
                }
            }
        
            var playerCustomization = _localStudentController.PlayerCustomization;
            playerCustomization.SetVisualCustomProperties();
        
            player.transform.position = GetPlayerSpawnPoint(player.TeamID);
        }

        private void SpawnTeacher()
        {
            AudioManager.Instance.PlayOneShot(_teacherSpawnSound, 2.5f);
        
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(_teacherPrefab.name, _teacherSpawn.position, _teacherSpawn.rotation);
                StartCoroutine(MoveCameraToTeacher());
            }
            else
            {
                StartCoroutine(MoveCameraToTeacher());
            }
        }

        private IEnumerator MoveCameraToTeacher()
        {
            _localStudentController.LookAtTeacher(true);
            _localStudentController.SetStudentFreezeState(true);
            yield return new WaitForSeconds(_teacherPreparationTime);
            _localStudentController.LookAtTeacher(false);
            _localStudentController.SetStudentFreezeState(false);
        }

        private void GetAllPlayers()
        {
            _allPlayers = FindObjectsOfType<NetworkStudentController>();

            if (PhotonNetwork.IsMasterClient)
            {
                SetSpawnPoints();
            }
        }

        private void SetSpawnPoints()
        {
            var blueSpawns = 0;
            var redSpawns = 0;
            foreach (var student in _allPlayers)
            {
                switch (student.TeamID)
                {
                    case 1:
                        student.transform.position = _blueSpawns[blueSpawns].position;
                        blueSpawns++;
                        break;
                    case 2:
                        student.transform.position = _redSpawns[redSpawns].position;
                        redSpawns++;
                        break;
                    default:
                        Debug.LogError("PROBLEM");
                        break;
                }
            }
        }

        #region PublicCalls

        public Vector3 GetPlayerSpawnPoint(byte teamID)
        {
            const float spawnRadius = 1f;
            var randX = Random.Range(-spawnRadius, spawnRadius);
            var randZ = Random.Range(-spawnRadius, spawnRadius);

            return teamID switch
            {
                1 => _blueSpawns[0].position + new Vector3(randX, 0, randZ),
                2 => _redSpawns[0].position + new Vector3(randX, 0, randZ),
                _ => throw new NullReferenceException("INVALID TEAM ID: " + teamID)
            };
        }
        
        public void IncrementTeamDeathCount(int teamCode)
        {
            var ht = PhotonNetwork.CurrentRoom.CustomProperties;
            var key = "deaths" + teamCode;

            if (ht.ContainsKey(key))
            {
                var deaths = (int)ht[key];
                ht[key] = ++deaths;
            }
            else
            {
                ht.Add(key, 1);
            }

            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        }

        public void SetLeadingShirt(int teamCode)
        {
            _leadingTeamShirt.sprite = _teamShirts[teamCode];
        }

        #endregion
    }
}
