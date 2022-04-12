using Interfaces;
using Player;
using UnityEngine;

public class SnowmanPile : MonoBehaviour, INetworkTriggerable
{
    [SerializeField] private Animator _hoverEButtonUI;

    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this blackboard
    /// </summary>
    public void TriggerableTrigger(NetworkStudentController currentPlayer)
    {
        
    }
    
    public void TriggerableEnter()
    {
        _hoverEButtonUI.enabled = true;
        _hoverEButtonUI.StartPlayback();
        _hoverEButtonUI.gameObject.SetActive(true);
    }

    public void TriggerableExit()
    {
        _hoverEButtonUI.StopPlayback();
        _hoverEButtonUI.enabled = false;
        _hoverEButtonUI.gameObject.SetActive(false);
    }
    
    #endregion
}
