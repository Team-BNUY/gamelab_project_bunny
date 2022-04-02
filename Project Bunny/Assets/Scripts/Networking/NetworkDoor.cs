using Interfaces;
using Player;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class NetworkDoor : MonoBehaviour, INetworkTriggerable
{
    private const string ARENA_SCENE_NAME = "4-Arena";

    [SerializeField] private Transform[] waitPoints;
    [SerializeField] private Transform doorPoint;

    [SerializeField] private Vector3 openPos;
    [SerializeField] private Vector3 openRot;
    [SerializeField] private Vector3 closedPos;
    [SerializeField] private Vector3 closedRot;

    private readonly float waitTime = 5f;

    // Start is called before the first frame update
    private void Start()
    {
        transform.position = closedPos;
        transform.rotation = Quaternion.Euler(closedRot);
    }

    public void Trigger(NetworkStudentController currentStudentController)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        transform.position = openPos;
        transform.rotation = Quaternion.Euler(openRot);
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

    public void Enter()
    {
        Debug.Log("Entered Door");
    }

    public void Exit()
    {
        Debug.Log("Exited Door");
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
