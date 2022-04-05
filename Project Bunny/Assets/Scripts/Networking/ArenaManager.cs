using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using AI.Agents;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Player;
using TMPro;
using UnityEngine;
using Cinemachine;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ArenaManager : MonoBehaviourPunCallbacks
{
    private static ArenaManager _instance;

    public static ArenaManager Instance {
        get {
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
    [SerializeField] private GameObject _giantRollballPrefab;
    [SerializeField] private GameObject _giantRollballBurst;
    [SerializeField] private GameObject _cannonBall;
    [SerializeField] private GameObject _snowmanPrefab;

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
    private NetworkStudentController[] _allPlayers;
    private List<Gang> _gangs = new List<Gang>();

    [Header("Timers")]
    [SerializeField] private float _teacherSpawnTime;
    [SerializeField] private float _teacherPreparationTime;
    [SerializeField] private TMP_Text timerDisplay;
    [SerializeField] private bool hasTimerStarted = false;
    [SerializeField] private float snowmanTimer;
    private int _timeElapsed = 0;
    private int _oldTimeElapsed = 0;
    private double _startTime;
    private bool _returnToLobbyHasRun = false;
    private const float TEACHER_CAMERA_PAN_TIME = 4f;
    private const int TIMER_DURATION = 5 * 60;
    private const string START_TIME_KEY = "StartTime";
    private const string LOBBY_SCENE_NAME = "2-Lobby";
    private const string ROOM_SCENE_NAME = "3-Room";
    private const string READY_KEY = "isready";
    private int readyPlayers = 0;

    private NetworkStudentController _localStudentController;
    private NetworkGiantRollball[] _currentRollballs = new NetworkGiantRollball[2];

    public GameObject SnowballPrefab => _snowballPrefab;
    public GameObject IceballPrefab => _iceballPrefab;
    public GameObject SnowballBurst => _snowballBurst;
    public GameObject GiantRollballBurst => _giantRollballBurst;
    public GameObject CannonBall => _cannonBall;
    public GameObject SnowmanPrefab => _snowmanPrefab;
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

    [Header("Other Spawns")]
    [SerializeField] private Transform[] _redSpawns;
    [SerializeField] private Transform[] _blueSpawns;
    [SerializeField] private Transform[] _giantRollballSpawns;
    [SerializeField] private GameObject loadingScreen;

    private void Awake()
    {
        loadingScreen.SetActive(true);
        if (!PhotonNetwork.IsMasterClient) return;

        _gangs.Clear();
    }

    void Start()
    {
        SetIsReady(false);
        loadingScreen.SetActive(true);
        SpawnPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            InitializeGiantRollballs();
        }

        ScoreManager.Instance.ClearPropertyCounters();

        InjectInitialStudentStates();

        //Invoke(nameof(StartMatch), 1f);
        SetIsReady(true);
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void StartMatch()
    {
        Invoke(nameof(SpawnTeacher), _teacherSpawnTime);
        loadingScreen.SetActive(false);
        StartTimer();
    }

    private void UpdateTimer()
    {
        if (!hasTimerStarted) return;
        _oldTimeElapsed = _timeElapsed;
        this._timeElapsed = (int)(PhotonNetwork.Time - _startTime);

        if (_timeElapsed > _oldTimeElapsed)
        {
            int timeLeft = TIMER_DURATION - _timeElapsed;

            //Integer increment, update UI
            if (timeLeft > 60)
            {
                this.timerDisplay.text = $"{timeLeft / 60}:{(timeLeft % 60).ToString("00")}";
            }
            else
            {
                this.timerDisplay.text = timeLeft.ToString();
            }
        }

        if (_timeElapsed >= TIMER_DURATION)
        {
            Debug.Log("Timer completed.");
            ReturnToLobby();
        }
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
        ExitGames.Client.Photon.Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
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

        if (PhotonNetwork.IsMasterClient)
        {
            ScoreManager.Instance.CalculateScore();
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.LoadLevel(ROOM_SCENE_NAME);
        }
    }

    private void StartTimer()
    {
        this.timerDisplay.text = $"{(TIMER_DURATION - _timeElapsed) / 60}:{((TIMER_DURATION - _timeElapsed) % 60).ToString("00")}";

        if (PhotonNetwork.IsMasterClient)
        {
            var myHashTable = new ExitGames.Client.Photon.Hashtable();
            _startTime = PhotonNetwork.Time;
            hasTimerStarted = true;
            myHashTable.Add(START_TIME_KEY, _startTime);
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

        player.transform.position = GetPlayerSpawnPoint(player.TeamID);
    }
    

    private void InitializeGiantRollballs()
    {
        var i = 0;
        foreach (var spawn in _giantRollballSpawns)
        {
            SpawnGiantRollball(spawn, i);
            i++;
        }
    }

    public void RemoveGiantRollball(int index)
    {
        StartCoroutine(RemoveGiantRollballWhenDestroyed(index));
    }

    private IEnumerator RemoveGiantRollballWhenDestroyed(int index)
    {
        _currentRollballs[index] = null;
        yield return new WaitForSeconds(5f);
        SpawnGiantRollball(_giantRollballSpawns[index], index);
    }

    private void SpawnGiantRollball(Transform spawn, int index)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var spawnTransform = spawn.transform;
            var go = PhotonNetwork.Instantiate(_giantRollballPrefab.name, spawnTransform.position, spawnTransform.rotation);
            var rollBall = go.GetComponent<NetworkGiantRollball>();
            rollBall.InitializeGiantRollball(index);
            _currentRollballs[index] = rollBall;
        }
    }

    private void SpawnTeacher()
    {
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
        
        /*Hashtable emptyTable = new Hashtable();
        PhotonNetwork.LocalPlayer.CustomProperties = emptyTable;*/

        if (scoreManager != null)
            GameObject.Destroy(scoreManager.gameObject);

        if (teamsManager != null)
            GameObject.Destroy(teamsManager.gameObject);

        UnityEngine.SceneManagement.SceneManager.LoadScene(LOBBY_SCENE_NAME);
    }

    public Vector3 GetPlayerSpawnPoint(byte TeamID)
    {
        float spawnRadius = 1f;
        float randx, randz;
        randx = Random.Range(-spawnRadius, spawnRadius);
        randz = Random.Range(-spawnRadius, spawnRadius);

        if (TeamID == 1)
            return _blueSpawns[0].position + new Vector3(randx, 0, randz);
        else if (TeamID == 2)
            return _redSpawns[0].position + new Vector3(randx, 0, randz);
        else
            throw new NullReferenceException("INVALID TEAM ID: " + TeamID);
    }

    private void GetAllPlayers()
    {
        _allPlayers = FindObjectsOfType<NetworkStudentController>();
        SetSpawnPoints();
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

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        if (propertiesThatChanged.ContainsKey(START_TIME_KEY) && !PhotonNetwork.IsMasterClient)
        {
            _startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties[START_TIME_KEY].ToString());
            hasTimerStarted = true;
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
            if (PhotonNetwork.IsMasterClient)
            {
                _allPlayers = Array.Empty<NetworkStudentController>();
                Invoke(nameof(GetAllPlayers), 0.1f);
            }
            StartMatch();
        }
    }
}
