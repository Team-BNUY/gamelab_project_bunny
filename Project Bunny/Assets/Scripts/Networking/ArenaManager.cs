using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
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

    private NetworkStudentController[] _allPlayers;
    

    [Header("Timer")]
    [SerializeField] private TMP_Text timerDisplay;
    [SerializeField] private bool hasTimerStarted = false;
    private int timeElapsed = 0;
    private int oldTimeElapsed = 0;
    private double startTime;
    private bool returnToLobbyHasRun = false;
    private const int TIMER_DURATION = 60 * 5;
    private const string START_TIME_KEY = "StartTime";
    private const string ROOM_SCENE_NAME = "3-Room";

    public GameObject SnowballPrefab => _snowballPrefab;
    public GameObject IceballPrefab => _iceballPrefab;
    public GameObject SnowballBurst => _snowballBurst;
    public GameObject GiantRollballBurst => _giantRollballBurst;
    public GameObject CannonBall => _cannonBall;
    public GameObject SnowmanPrefab => _snowmanPrefab;
    public NetworkStudentController[] AllPlayers => _allPlayers;

    [SerializeField] private Transform[] _teamSpawns;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _allPlayers = Array.Empty<NetworkStudentController>();
            Invoke(nameof(GetAllPlayers), 1f);
        }
        
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
            //Timer has completed countdown.
            ReturnToLobby();
        }
    }

    private void ReturnToLobby() {
        if (returnToLobbyHasRun) return;
        returnToLobbyHasRun = true;

        if (PhotonNetwork.IsMasterClient)
        {
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
        player.SetCamera(Instantiate(_playerCamera));
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
            return _teamSpawns[team.Code - 1].position + new Vector3(randx, 0, randz);
        }
        return Vector3.zero;
    }
    
    private void GetAllPlayers()
    {
        _allPlayers = FindObjectsOfType<NetworkStudentController>();
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
