using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArenaManager : MonoBehaviour
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
    [SerializeField] private GameObject _giantRollballBurst;
    [SerializeField] private GameObject _cannonBall;

    private NetworkStudentController[] _allPlayers;
    
    public GameObject SnowballPrefab => _snowballPrefab;
    public GameObject IceballPrefab => _iceballPrefab;
    public GameObject SnowballBurst => _snowballBurst;
    public GameObject GiantRollballBurst => _giantRollballBurst;
    public GameObject CannonBall => _cannonBall;
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
    }

    private void SpawnPlayer()
    {
        NetworkStudentController player = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<NetworkStudentController>();
        player.transform.position = GetPlayerSpawnPoint(player);
        player.PlayerID = PhotonNetwork.LocalPlayer.UserId;
        PhotonNetwork.LocalPlayer.TagObject = player;
        player.SetCamera(Instantiate(_playerCamera));
    }

    private Vector3 GetPlayerSpawnPoint(NetworkStudentController player)
    {
        float spawnRadius = 1f;
        float randx, randz;
        randx = Random.Range(-spawnRadius, spawnRadius);
        randz = Random.Range(-spawnRadius, spawnRadius);

        object teamId;
        PhotonTeam team;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamId) && PhotonTeamsManager.Instance.TryGetTeamByCode((byte)teamId, out team))
        {
            return (_teamSpawns[team.Code - 1].position + new Vector3(randx, 0, randz));
        }
        return Vector3.zero;
    }
    
    private void GetAllPlayers()
    {
        _allPlayers = FindObjectsOfType<NetworkStudentController>();
    }
}
