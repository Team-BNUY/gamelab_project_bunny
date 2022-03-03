using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        private const string LOBBY_SCENE_NAME = "2-Lobby";

        void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            SceneManager.LoadScene(LOBBY_SCENE_NAME);
        }
    }
}