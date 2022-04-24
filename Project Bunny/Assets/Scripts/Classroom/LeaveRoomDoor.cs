using Interfaces;
using Player;
using System.Collections;
using Networking;
using UnityEngine;
using Photon.Pun;

public class LeaveRoomDoor : MonoBehaviour, INetworkTriggerable
{
    
    [SerializeField] public GameObject hoverEButtonUI;

    public void TriggerableTrigger(NetworkStudentController currentStudentController)
    {
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.PlayerLeaveRoom();
        }
    }

    public void TriggerableEnter()
    {
        hoverEButtonUI.SetActive(true);
    }

    public void TriggerableExit()
    {
        hoverEButtonUI.SetActive(false);
    }
    
}
