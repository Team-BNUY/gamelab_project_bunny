using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamWall : MonoBehaviour
{
    [SerializeField] public int _allowedTeam = 0;
    [SerializeField] public BoxCollider _collider;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
