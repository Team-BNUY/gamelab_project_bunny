using Photon.Pun;
using UnityEngine;

public class NetworkDestroyOnStop : MonoBehaviourPunCallbacks
{
    private void OnParticleSystemStopped()
    {
        if (!photonView.IsMine) return;
        
        Debug.Log("PARTICLE SYSTEM");
        PhotonNetwork.Destroy(gameObject);
    }
}
