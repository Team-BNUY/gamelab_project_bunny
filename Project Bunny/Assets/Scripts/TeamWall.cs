using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamWall : MonoBehaviour
{
    [SerializeField] public int AllowedTeam = 0;
    [SerializeField] public BoxCollider collider;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
