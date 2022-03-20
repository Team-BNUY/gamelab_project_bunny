using Interfaces;
using Player;
using UnityEngine;

public class OrangeTeamTable : MonoBehaviour, INetworkTriggerable
{
    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this blackboard
    /// </summary>
    public void Trigger(NetworkStudentController currentPlayer)
    {
        Debug.Log("You just triggered the Orange Team Table!");
    }
    
    #endregion
}