using Interfaces;
using Player;
using System.Collections;
using Networking;
using UnityEngine;
using Photon.Pun;

public class LeaveRoomDoor : MonoBehaviour, INetworkTriggerable
{
    
    [SerializeField] public GameObject hoverEButtonUI;

    public void Trigger(NetworkStudentController currentStudentController)
    {
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.PlayerLeaveRoom();
        }
    }

    public void Enter()
    {
        hoverEButtonUI.SetActive(true);
    }

    public void Exit()
    {
        hoverEButtonUI.SetActive(false);
    }
    
}
