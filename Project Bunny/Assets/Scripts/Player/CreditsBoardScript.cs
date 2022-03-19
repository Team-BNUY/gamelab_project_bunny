using Interfaces;
using Player;
using UnityEngine;

public class CreditsBoardScript : MonoBehaviour, INetworkTriggerable
{
    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this Credits Board
    /// </summary>
    public void Trigger(NetworkStudentController currentPlayer)
    {
        Debug.Log("You just triggered the Credits Board!");
    }
    
    #endregion
}