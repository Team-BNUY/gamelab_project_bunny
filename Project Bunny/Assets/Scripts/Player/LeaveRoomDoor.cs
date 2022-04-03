using Interfaces;
using Player;
using System.Collections;
using Networking;
using UnityEngine;
using Photon.Pun;

public class LeaveRoomDoor : MonoBehaviour, INetworkTriggerable
{
    
    [SerializeField] public Animator hoverEButtonUI;

    public void Trigger(NetworkStudentController currentStudentController)
    {
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.PlayerLeaveRoom();
        }
    }

    public void Enter()
    {
        hoverEButtonUI.enabled = true;
        hoverEButtonUI.StartPlayback();
        hoverEButtonUI.gameObject.SetActive(true);
    }

    public void Exit()
    {
        hoverEButtonUI.StopPlayback();
        hoverEButtonUI.enabled = false;
        hoverEButtonUI.gameObject.SetActive(false);
    }
    
}
