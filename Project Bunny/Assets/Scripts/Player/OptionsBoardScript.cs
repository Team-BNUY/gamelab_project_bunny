using Interfaces;
using Player;
using UnityEngine;

public class OptionsBoardScript : MonoBehaviour, INetworkTriggerable
{
    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this Options Board
    /// </summary>
    public void Trigger(NetworkStudentController currentPlayer)
    {
        Debug.Log("You just triggered the Options Board!");
    }
    
    #endregion
}