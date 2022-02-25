using UnityEngine;
using Photon.Pun;
using Player;

namespace Networking
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _playerCamera;

        void Start()
        {
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            NetworkStudentController player = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<NetworkStudentController>();
            player.SetCamera(_playerCamera);
        }
    }
}
