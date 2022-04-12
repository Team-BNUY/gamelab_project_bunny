using System;
using Interfaces;
using Player;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class NetworkDoor : MonoBehaviour, INetworkTriggerable
{
    private const string ARENA_SCENE_NAME = "4-Arena";

    [SerializeField] private Transform[] waitPoints;
    [SerializeField] public Transform doorPoint;
    [SerializeField] public GameObject hoverEButtonUI;
    [SerializeField] public GameObject doorPrefab;
    [SerializeField] public Transform doorParent;
    [SerializeField] private int _schoolBellClipId;
    [SerializeField] private int _openDoorSoundId;
    [SerializeField] private Vector3 openPos;
    [SerializeField] private Vector3 openRot;
    [SerializeField] private Vector3 closedPos;
    [SerializeField] private Vector3 closedRot;

    private readonly float waitTime = 5f;
    private GameObject door;

    // Start is called before the first frame update
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            door = PhotonNetwork.Instantiate(doorPrefab.name, new Vector3(-15.97f, 1.58f, 2.42f), Quaternion.Euler(-90,0,0));
            door.transform.position = new Vector3(-15.97f, 1.58f, 2.42f);
            door.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }
        Invoke(nameof(SetDoorPosition), 0.1f);
    }

    public void SetDoorPosition() 
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        door.transform.position = new Vector3(-15.97f, 1.58f, 2.42f);
        door.transform.rotation = Quaternion.Euler(-90, 0, 0);
    }

    public void TriggerableTrigger(NetworkStudentController currentStudentController)
    {
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam() == null) return;

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if ((!player.CustomProperties.ContainsKey("isReady") || (bool)player.CustomProperties["isReady"] == false) && !player.IsMasterClient)
                {
                    return;
                }
            }

            door.transform.position = new Vector3(-15, 1.7f, 3.2f);
            door.transform.rotation = new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f);

            var allPlayers = FindObjectsOfType<NetworkStudentController>();
            var waitPointIndex = 0;
            foreach (var student in allPlayers)
            {
                if (student != currentStudentController)
                {
                    var coroutine = RegularStudentLeaveClassroom(student, waitPoints[waitPointIndex].position);
                    StartCoroutine(coroutine);
                    waitPointIndex++;
                }
                else
                {
                    StartCoroutine(nameof(ExitClassroom), student);
                }
            }
        }
        else
        {
            var ht = PhotonNetwork.LocalPlayer.CustomProperties;
            if (ht.ContainsKey("isReady"))
            {
                var isReady = (bool)ht["isReady"];
                ht["isReady"] = !isReady;
            }
            else
            {
                ht.Add("isReady", true);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
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

    private IEnumerator ExitClassroom(NetworkStudentController student)
    {
        AudioManager.Instance.PlaySync(_openDoorSoundId, 1f);
        AudioManager.Instance.PlaySync(_schoolBellClipId, 0.25f);
        
        student.SetControlledMovement(Vector3.zero, true);
        yield return new WaitForSeconds(waitTime);
        student.SetControlledMovement(doorPoint.position, true);
    }

    private IEnumerator RegularStudentLeaveClassroom(NetworkStudentController student, Vector3 pos)
    {
        student.SetControlledMovement(pos, false);

        yield return new WaitForSeconds(waitTime);

        student.SetControlledMovement(doorPoint.position, true);
    }
}
