using UnityEngine;
using Photon.Pun;

namespace Networking
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] GameObject playerPrefab;

        void Start()
        {
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        }
    }
}
