using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using AI.Agents;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Player;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    [Header("Player Instantiation")]
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
    [SerializeField] private Sprite[] _teamShirts = new Sprite[3];
    private NetworkStudentController _localStudentController;

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
    [SerializeField] private float _teacherSpawnTime;
    [SerializeField] private float _teacherPreparationTime;
    [SerializeField] private TMP_Text timerDisplay;
    [SerializeField] private bool hasTimerStarted;
    [SerializeField] private float snowmanTimer;
    private int _timeElapsed;
    private int _oldTimeElapsed;
    private double _startTime;
    private bool _returnToLobbyHasRun;
    private const float TEACHER_CAMERA_PAN_TIME = 4f;
    private const int TIMER_DURATION = 7 * 60;
    private const string START_TIME_KEY = "StartTime";
    private const string LOBBY_SCENE_NAME = "2-Lobby";
    private const string ROOM_SCENE_NAME = "3-Room";
    private const string READY_KEY = "isready";
    private int readyPlayers;
    private bool _over;
    [SerializeField] TeamWall[] teamWalls;

    [Header("Audio")]
    [SerializeField] private AudioClip _music;
    [SerializeField] private AudioClip _teacherSpawnSound;
    [SerializeField] private AudioClip _schoolBellSound;

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

    [Header("Other Spawns")]
    [SerializeField] private Transform[] _redSpawns;
    [SerializeField] private Transform[] _blueSpawns;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private Image _leadingTeamShirt;

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

    private void StartMatch()
    {
        _over = false;
        InjectInitialStudentStates();
        Invoke(nameof(SpawnTeacher), _teacherSpawnTime);
        _loadingScreen.SetActive(false);
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
            var timeLeft = TIMER_DURATION - _timeElapsed;

            //Integer increment, update UI
            if (timeLeft > 60)
            {
                timerDisplay.text = $"{timeLeft / 60}:{(timeLeft % 60).ToString("00")}";
            }
            else
            {
                if (timeLeft >= 0f)
                {
                    timerDisplay.text = timeLeft.ToString();
                }
                else
                {
                    timerDisplay.text = 0.ToString();
                }
            }
        }

        if (_timeElapsed < TIMER_DURATION || _over) return;
        _over = true;
        
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
        Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
        if (props.ContainsKey(READY_KEY))
        {
            props[READY_KEY] = isReady;
        }
        else
        {
            props.Add(READY_KEY, isReady);
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
        PhotonNetwork.LoadLevel(ROOM_SCENE_NAME);
    }

    private void StartTimer()
    {
        timerDisplay.text = $"{(TIMER_DURATION - _timeElapsed) / 60}:{((TIMER_DURATION - _timeElapsed) % 60).ToString("00")}";

        if (PhotonNetwork.IsMasterClient)
        {
            var myHashTable = new ExitGames.Client.Photon.Hashtable();
            _startTime = PhotonNetwork.Time;
            hasTimerStarted = true;
            myHashTable.Add(START_TIME_KEY, _startTime);
            myHashTable.Add("deaths1", 0);
            myHashTable.Add("deaths2", 0);

            PhotonNetwork.CurrentRoom.SetCustomProperties(myHashTable);
        }
    }

    private void SpawnPlayer()
    {
        NetworkStudentController player = PhotonNetwork.Instantiate(_playerPrefab.name, GetPlayerSpawnPoint(PhotonNetwork.LocalPlayer.GetPhotonTeam().Code), Quaternion.identity).GetComponent<NetworkStudentController>();
        if (player.photonView.IsMine)
        {
            player.PlayerID = PhotonNetwork.LocalPlayer.UserId;
            player.TeamID = PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
            PhotonNetwork.LocalPlayer.TagObject = player;
            _localStudentController = player;
            player.SetCamera(Instantiate(_playerCamera), 60f, 25f, true, 0.7f, 5f);
            player.photonView.RPC("SyncPlayerInfo", RpcTarget.AllBuffered, player.PlayerID, player.TeamID);
        }

        if (!player.photonView.IsMine) return;

        foreach (TeamWall wall in teamWalls) {
            if (_localStudentController.TeamID == wall._allowedTeam) {
                wall._collider.enabled = false;
            }
        }

        Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

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
        yield return new WaitForSeconds(TEACHER_CAMERA_PAN_TIME);
        _localStudentController.LookAtTeacher(false);
        _localStudentController.SetStudentFreezeState(false);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        //Kick everyone from the room if the master client changed (too many bugs to deal with otherwise)
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        PhotonTeamsManager teamsManager = FindObjectOfType<PhotonTeamsManager>();

        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();

        /*Hashtable emptyTable = new Hashtable();
        PhotonNetwork.LocalPlayer.CustomProperties = emptyTable;*/

        if (scoreManager != null)
            Destroy(scoreManager.gameObject);

        if (teamsManager != null)
            Destroy(teamsManager.gameObject);

        UnityEngine.SceneManagement.SceneManager.LoadScene(LOBBY_SCENE_NAME);
        
    }

    public Vector3 GetPlayerSpawnPoint(byte TeamID)
    {
        var spawnRadius = 1f;
        float randx, randz;
        randx = Random.Range(-spawnRadius, spawnRadius);
        randz = Random.Range(-spawnRadius, spawnRadius);

        return TeamID switch
        {
            1 => _blueSpawns[0].position + new Vector3(randx, 0, randz),
            2 => _redSpawns[0].position + new Vector3(randx, 0, randz),
            _ => throw new NullReferenceException("INVALID TEAM ID: " + TeamID)
        };
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
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        int blueSpawns = 0;
        int redSpawns = 0;
        foreach (NetworkStudentController student in _allPlayers)
        {
            if (student.TeamID == 1)
            {
                student.transform.position = _blueSpawns[blueSpawns].position;
                blueSpawns++;
            }
            else if (student.TeamID == 2)
            {
                student.transform.position = _redSpawns[redSpawns].position;
                redSpawns++;
            }
            else
            {
                Debug.LogError("PROBLEM");
            }
        }
    }

    public void IncrementTeamDeathCount(int teamCode)
    {
        Hashtable ht = PhotonNetwork.CurrentRoom.CustomProperties;
        string key = "deaths" + teamCode.ToString();

        if (ht.ContainsKey(key))
        {
            int deaths = (int)ht[key];
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

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnRoomPropertiesUpdate(changedProps);
        if (changedProps.ContainsKey(START_TIME_KEY) && !PhotonNetwork.IsMasterClient)
        {
            _startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties[START_TIME_KEY].ToString());
            hasTimerStarted = true;
        }

        if (changedProps.ContainsKey("deaths1") || changedProps.ContainsKey("deaths2"))
        {
            int deaths1 = (int)changedProps["deaths1"];
            int deaths2 = (int)changedProps["deaths2"];
            if (deaths1 == deaths2)
            {
                SetLeadingShirt(0);
            }
            else
            {
                int winningTeam = (int)changedProps["deaths1"] < (int)changedProps["deaths2"] ? 1 : 2;
                SetLeadingShirt(winningTeam);
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(READY_KEY))
        {
            if ((bool)changedProps[READY_KEY])
                readyPlayers++;
        }

        if (readyPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            _allPlayers = Array.Empty<NetworkStudentController>();
            Invoke(nameof(GetAllPlayers), 0.1f);
            StartMatch();
        }
    }
}
