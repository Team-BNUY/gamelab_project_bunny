using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Player;
using Networking;

public class NetworkBallonPoire : MonoBehaviour
{
    private PhotonView _view;
    private Rigidbody _rb;

    void Start()
    {
        _view = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            _view.TransferOwnership(PhotonNetwork.LocalPlayer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (collision.gameObject.TryGetComponent<NetworkSnowball>(out NetworkSnowball snowball))
        {
            _rb.AddForce(collision.impulse, ForceMode.Impulse);
        }

        if (collision.gameObject.TryGetComponent<NetworkStudentController>(out NetworkStudentController student))
        {
            Vector3 dir = (collision.transform.position - transform.position).normalized;
            _rb.AddForce(15 * dir, ForceMode.Impulse);
        }


        Debug.Log(collision.gameObject.name);
    }
}
