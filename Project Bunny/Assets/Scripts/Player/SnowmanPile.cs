using System.Collections;
using System.Collections.Generic;
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
    public void Trigger(NetworkStudentController currentPlayer)
    {
        
    }
    
    public void Enter()
    {
        _hoverEButtonUI.enabled = true;
        _hoverEButtonUI.StartPlayback();
        _hoverEButtonUI.gameObject.SetActive(true);
    }

    public void Exit()
    {
        _hoverEButtonUI.StopPlayback();
        _hoverEButtonUI.enabled = false;
        _hoverEButtonUI.gameObject.SetActive(false);
    }
    
    #endregion
}
