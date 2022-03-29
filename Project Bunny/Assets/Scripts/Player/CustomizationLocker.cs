using Interfaces;
using Player;
using UnityEngine;

public class CustomizationLocker : MonoBehaviour, INetworkTriggerable
{
    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this customization menu
    /// </summary>
    public void Trigger(NetworkStudentController currentPlayer)
    {
        Debug.Log("You just triggered the Customization Menu!");
    }
    
    #endregion
}

