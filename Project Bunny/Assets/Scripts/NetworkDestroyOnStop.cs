using Photon.Pun;

public class NetworkDestroyOnStop : MonoBehaviourPunCallbacks
{
    private void OnParticleSystemStopped()
    {
        if (!photonView.IsMine) return;
        
        PhotonNetwork.Destroy(gameObject);
    }
}
