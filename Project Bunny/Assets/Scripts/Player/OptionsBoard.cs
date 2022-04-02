using Interfaces;
using Player;
using UnityEngine;

public class OptionsBoard : MonoBehaviour, INetworkTriggerable
{
    [SerializeField] public Animator hoverEButtonUI;
    
    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this Options Board
    /// </summary>
    public void Trigger(NetworkStudentController currentPlayer)
    {
        Debug.Log("You just triggered the Options Board!");
    }
    
    public void Enter()
    {
        hoverEButtonUI.enabled = true;
        hoverEButtonUI.StartPlayback();
        hoverEButtonUI.gameObject.SetActive(true);
    }

    public void Exit()
    {
        hoverEButtonUI.StopPlayback();
        hoverEButtonUI.enabled = false;
        hoverEButtonUI.gameObject.SetActive(false);
    }
    
    #endregion
}