using Interfaces;
using Player;
using UnityEngine;

public class BoomboxController : MonoBehaviour, INetworkTriggerable
{
    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this boombox
    /// </summary>
    public void Trigger(NetworkStudentController currentPlayer)
    {
        Debug.Log("You just triggered the Boombox!");
    }
    
    #endregion
}