using Photon.Pun;

public class NetworkCannonBalls : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // var cannon = ArenaManager.Instance.Cannon;
        // transform.position = cannon.CannonBallSeat.position;
        // transform.SetParent(cannon.CannonBallSeat.transform);
    }
}
