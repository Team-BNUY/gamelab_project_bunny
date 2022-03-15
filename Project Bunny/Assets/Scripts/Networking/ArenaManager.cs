using Photon.Pun;
using Player;
using UnityEngine;

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
    public GameObject SnowballPrefab => _snowballPrefab;
    public GameObject IceballPrefab => _iceballPrefab;
    public GameObject SnowballBurst => _snowballBurst;
    public GameObject GiantRollballBurst => _giantRollballBurst;
    public GameObject CannonBall => _cannonBall;

    [SerializeField] private Transform _teamSpawns;

    void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        NetworkStudentController player = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<NetworkStudentController>();
        PhotonNetwork.LocalPlayer.TagObject = player;
        player.SetCamera(Instantiate(_playerCamera));
    }
    
}
