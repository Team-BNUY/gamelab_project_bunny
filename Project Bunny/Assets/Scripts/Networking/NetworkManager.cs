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
            var player = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity);
            //var cam = PhotonNetwork.Instantiate(_playerCamera.name, Vector3.zero, Quaternion.identity);
            // TODO: Do this without the need of GetComponent
            //player.GetComponent<NetworkStudentController>().SetCamera(cam);
        }
    }
}
