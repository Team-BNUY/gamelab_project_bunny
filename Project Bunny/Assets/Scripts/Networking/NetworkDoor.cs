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
    [SerializeField] private Transform doorPoint;
    [SerializeField] public Animator hoverEButtonUI;
    [SerializeField] public GameObject door;

    [SerializeField] private Vector3 openPos;
    [SerializeField] private Vector3 openRot;
    [SerializeField] private Vector3 closedPos;
    [SerializeField] private Vector3 closedRot;

    private readonly float waitTime = 5f;

    // Start is called before the first frame update
    private void Start()
    {
        if (door != null)
        {
            door.transform.localPosition = closedPos;
            door.transform.localRotation = Quaternion.Euler(closedRot);
        }

    }

    public void Trigger(NetworkStudentController currentStudentController)
    {
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam() == null) return;

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Photon.Realtime.Player player in PhotonNetwork.CurrentRoom.Players.Values) {
                if ((!player.CustomProperties.ContainsKey("isReady") || (bool)player.CustomProperties["isReady"] == false) && !player.IsMasterClient) {
                    return;               
                }
            }

            if (door != null)
            {
                door.transform.localPosition = openPos;
                door.transform.localRotation = Quaternion.Euler(openRot);
            }

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
            ExitGames.Client.Photon.Hashtable ht = PhotonNetwork.LocalPlayer.CustomProperties;
            if (ht.ContainsKey("isReady"))
            {
                bool isReady = (bool)ht["isReady"];
                ht["isReady"] = !isReady;
            }
            else
            {
                ht.Add("isReady", true);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
        }
    }

    public void Enter()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        hoverEButtonUI.enabled = true;
        hoverEButtonUI.StartPlayback();
        hoverEButtonUI.gameObject.SetActive(true);
    }

    public void Exit()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        hoverEButtonUI.StopPlayback();
        hoverEButtonUI.enabled = false;
        hoverEButtonUI.gameObject.SetActive(false);
    }

    private IEnumerator ExitClassroom(NetworkStudentController student)
    {
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
