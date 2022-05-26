using System.Collections;
using Interfaces;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Player;
using UnityEngine;

namespace Classroom
{
    public class NetworkDoor : MonoBehaviour, INetworkTriggerable
    {
        [SerializeField] private Transform[] waitPoints;
        [SerializeField] public Transform doorPoint;
        [SerializeField] public GameObject hoverEButtonUI;
        [SerializeField] public GameObject doorPrefab;
        [SerializeField] private int _schoolBellClipId;
        [SerializeField] private int _openDoorSoundId;

        private const float WaitTime = 5f;
        private GameObject _door;
        private bool _triggered;

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _door = PhotonNetwork.Instantiate(doorPrefab.name, new Vector3(-15.97f, 1.58f, 2.42f), Quaternion.Euler(-90,0,0));
                _door.transform.position = new Vector3(-15.97f, 1.58f, 2.42f);
                _door.transform.rotation = Quaternion.Euler(-90, 0, 0);
                _triggered = false;
            }
            
            Invoke(nameof(SetDoorPosition), 0.1f);
        }

        public void SetDoorPosition() 
        {
            if (!PhotonNetwork.IsMasterClient) return;
        
            _door.transform.position = new Vector3(-15.97f, 1.58f, 2.42f);
            _door.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }

        public void TriggerableTrigger(NetworkStudentController currentStudentController)
        {
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam() == null || _triggered) return;

            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    if ((!player.CustomProperties.ContainsKey("isReady") || (bool)player.CustomProperties["isReady"] == false) && !player.IsMasterClient)
                    {
                        return;
                    }
                }

                _triggered = true;
                _door.transform.position = new Vector3(-15, 1.7f, 3.2f);
                _door.transform.rotation = new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f);

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
        
            student.SyncIsReady(false, student.PlayerID);
            student.SetControlledMovement(Vector3.zero, true);
            yield return new WaitForSeconds(WaitTime);
            student.SetControlledMovement(doorPoint.position, true);
        }

        private IEnumerator RegularStudentLeaveClassroom(NetworkStudentController student, Vector3 pos)
        {
            student.SyncIsReady(false, student.PlayerID);
            student.SetControlledMovement(pos, false);

            yield return new WaitForSeconds(WaitTime);

            student.SetControlledMovement(doorPoint.position, true);
        }
    }
}
