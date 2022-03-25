using Interfaces;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace Player
{
    public class OrangeTeamTable : MonoBehaviour, INetworkTriggerable
    {
        public static OrangeTeamTable instance;
    
        [Header("Orange Table Instantiation")]
        [SerializeField] private GameObject[] _jerseys;

        [SerializeField] private Material _highlightMaterial;
        [SerializeField] private Material _regularMaterial;
    
    
        private int _teamCount = -1;
        private const int _teamMaxSize = 4;
        public PhotonView _view;
   
    
        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
            } else{
                instance = this;
            }
            
            if (_view == null)
            {
                _view = GetComponent<PhotonView>();
            }
        }
    
    
        #region InterfaceMethods
    
        /// <summary>
        /// Method that runs when you trigger this blackboard
        /// </summary>
        public void Trigger(NetworkStudentController currentPlayer)
        {
            if (_teamCount >= (_teamMaxSize-1)) return;
            
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam() != null)
            {
                if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == "Red")
                {
                    PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
                    _view.RPC("SubtractTeamCount", RpcTarget.AllBuffered);
                    currentPlayer.RestoreTeamlessColors_RPC();
                }
                else
                {
                    PhotonNetwork.LocalPlayer.SwitchTeam(2);
                    BlueTeamTable.instance.SubtractTeamCount_RPC();
                    _view.RPC("AddTeamCount", RpcTarget.AllBuffered);
                }
            }
            else
            {
                PhotonNetwork.LocalPlayer.JoinTeam(2);
                _view.RPC("AddTeamCount", RpcTarget.AllBuffered);
            }
            
            
        }
    
        #endregion
    
        #region Table Actions
    
        /// <summary>
        /// Method to add the team count and update the jersey visuals 
        /// </summary>
        [PunRPC]
        public void AddTeamCount()
        {
            if (_teamCount < _teamMaxSize)
            {
                _teamCount++;
                _jerseys[_teamCount].GetComponent<Renderer>().material = _highlightMaterial;
            }
        }
    
        /// <summary>
        /// Method to subtract the team count and update the jersey visuals 
        /// </summary>
        [PunRPC]
        public void SubtractTeamCount()
        {
            if (_teamCount >= 0)
            {
                _jerseys[_teamCount].GetComponent<Renderer>().material = _regularMaterial;
                _teamCount--;
            }
        
        }
        
        public void SubtractTeamCount_RPC()
        { 
            _view.RPC("SubtractTeamCount", RpcTarget.AllBuffered);
        }
    
    
        #endregion
    }
}