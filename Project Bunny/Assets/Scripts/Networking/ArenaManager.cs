using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Player;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] private GameObject _giantRollballBurst;
    [SerializeField] private GameObject _cannonBall;
    [SerializeField] private GameObject _snowmanPrefab;
    
    [Header("AI")]
    [SerializeField] private LayerMask _studentLayer;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private List<ActionSpot> _actionSpots;
    private NetworkStudentController[] _allPlayers;
    private List<Gang> _gangs = new List<Gang>();

    [Header("Timer")]
    [SerializeField] private TMP_Text timerDisplay;
    [SerializeField] private bool hasTimerStarted = false;
    private int timeElapsed = 0;
    private int oldTimeElapsed = 0;
    private double startTime;
    private bool returnToLobbyHasRun = false;
    private const int TIMER_DURATION = 1 * 60;
    private const string START_TIME_KEY = "StartTime";
    private const string LOBBY_SCENE_NAME = "2-Lobby";
    private const string ROOM_SCENE_NAME = "3-Room";

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
    public List<ActionSpot> ActionSpots => _actionSpots;

    [SerializeField] private Transform[] _redSpawns;
    [SerializeField] private Transform[] _blueSpawns;

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        _gangs.Clear();
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _allPlayers = Array.Empty<NetworkStudentController>();
            Invoke(nameof(GetAllPlayers), 0.1f);
        }
        ScoreManager.Instance.ResetTeamDeaths();

        SpawnPlayer();
        StartTimer();
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (!hasTimerStarted) return;
        oldTimeElapsed = timeElapsed;
        this.timeElapsed = (int)(PhotonNetwork.Time - startTime);

        if (timeElapsed > oldTimeElapsed)
        {
            int timeLeft = TIMER_DURATION - timeElapsed;

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

        if (timeElapsed >= TIMER_DURATION)
        {
            Debug.Log("Timer completed.");
            ReturnToLobby();
        }
    }

    private void ReturnToLobby()
    {
        if (returnToLobbyHasRun) return;
        returnToLobbyHasRun = true;

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
        this.timerDisplay.text = $"{(TIMER_DURATION - timeElapsed) / 60}:{((TIMER_DURATION - timeElapsed) % 60).ToString("00")}";

        if (PhotonNetwork.IsMasterClient)
        {
            var myHashTable = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            hasTimerStarted = true;
            myHashTable.Add(START_TIME_KEY, startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(myHashTable);
        }
    }

    private void SpawnPlayer()
    {
        NetworkStudentController player = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<NetworkStudentController>();
        player.transform.position = GetPlayerSpawnPoint(player);
        player.PlayerID = PhotonNetwork.LocalPlayer.UserId;
        player.TeamID = PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
        PhotonNetwork.LocalPlayer.TagObject = player;
        player.SetCamera(Instantiate(_playerCamera), 60f, 25f);
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

        if (scoreManager != null)
            GameObject.Destroy(scoreManager.gameObject);

        if (teamsManager != null)
            GameObject.Destroy(teamsManager.gameObject);

        UnityEngine.SceneManagement.SceneManager.LoadScene(LOBBY_SCENE_NAME);
    }

    public Vector3 GetPlayerSpawnPoint(NetworkStudentController player = null)
    {
        float spawnRadius = 1f;
        float randx, randz;
        randx = Random.Range(-spawnRadius, spawnRadius);
        randz = Random.Range(-spawnRadius, spawnRadius);

        object teamId;
        PhotonTeam team;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamId)
            && PhotonTeamsManager.Instance.TryGetTeamByCode((byte)teamId, out team))
        {
            if (team.Code == 1)
                return _blueSpawns[0].position + new Vector3(randx, 0, randz);
            else
                return _redSpawns[0].position + new Vector3(randx, 0, randz);
        }
        return Vector3.zero;
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
            object teamId;
            PhotonTeam team;
            bool tryGetPlayerTeam = players.FirstOrDefault(x => x.UserId == student.PlayerID).CustomProperties.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamId);
            bool tryGetTeam = PhotonTeamsManager.Instance.TryGetTeamByCode((byte)teamId, out team);
            if (tryGetPlayerTeam && tryGetTeam)
            {
                if (team.Code == 1)
                {
                    student.transform.position = _blueSpawns[blueSpawns].position;
                    blueSpawns++;
                }
                else
                {
                    student.transform.position = _redSpawns[redSpawns].position;
                    redSpawns++;
                }
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        if (propertiesThatChanged.ContainsKey(START_TIME_KEY) && !PhotonNetwork.IsMasterClient)
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties[START_TIME_KEY].ToString());
            hasTimerStarted = true;
        }
    }

}
