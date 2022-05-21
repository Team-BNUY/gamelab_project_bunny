using Interfaces;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace Player
{
    public class RedTeamTable : MonoBehaviour, INetworkTriggerable
    {
        public static RedTeamTable instance;

        [Header("Orange Table Instantiation")]
        [SerializeField] private GameObject[] _jerseys;
        [SerializeField] private AudioClip _shitWear;
        [SerializeField] private AudioClip _shitTakeOff;
        [SerializeField] public Animator hoverEButtonUI;

        private int _teamCount = -1;
        private const int _teamMaxSize = 4;
        public PhotonView _view;
    
        private void Awake() {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
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
        public void TriggerableTrigger(NetworkStudentController currentStudentController)
        {
            if (_teamCount >= _teamMaxSize - 1) return;
            
            var localPlayer = PhotonNetwork.LocalPlayer;
            
            if (localPlayer.CustomProperties.ContainsKey("isReady")) return;
            
            if (localPlayer.GetPhotonTeam() != null)
            {
                if (localPlayer.GetPhotonTeam().Name == "Red")
                {
                    localPlayer.LeaveCurrentTeam();
                    _view.RPC("SubtractTeamCount", RpcTarget.AllBuffered);
                    currentStudentController.GetComponent<PlayerCustomization>().RestoreTeamlessColors();
                    AudioManager.Instance.PlayOneShot(_shitTakeOff, 0.5f);
                }
                else
                {
                    localPlayer.SwitchTeam(2);
                    BlueTeamTable.instance.SubtractTeamCount_RPC();
                    _view.RPC("AddTeamCount", RpcTarget.AllBuffered);
                    AudioManager.Instance.PlayOneShot(_shitWear, 0.5f);
                }
            }
            else
            {
                localPlayer.JoinTeam(2);
                _view.RPC("AddTeamCount", RpcTarget.AllBuffered);
                AudioManager.Instance.PlayOneShot(_shitWear, 0.5f);
            }
        }
        
        public void TriggerableEnter()
        {
            hoverEButtonUI.enabled = true;
            hoverEButtonUI.gameObject.SetActive(true);
            hoverEButtonUI.Play("EInteract");
        }
        
        public void TriggerableExit()
        {
            hoverEButtonUI.enabled = false;
            hoverEButtonUI.gameObject.SetActive(false);
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
                _jerseys[_teamCount].gameObject.SetActive(false);
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
                _jerseys[_teamCount].gameObject.SetActive(true);
                _teamCount--;
            }
        
        }
        
        public void SubtractTeamCount_RPC()
        { 
            _view.RPC("SubtractTeamCount", RpcTarget.AllBuffered);
        }
        
        
        public void AddTeamCount_RPC()
        { 
            _view.RPC("AddTeamCount", RpcTarget.AllBuffered);
        }
    
    
        #endregion
    }
}