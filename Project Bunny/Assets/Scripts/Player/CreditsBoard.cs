using System;
using Interfaces;
using Player;
using UnityEngine;

public class CreditsBoard : MonoBehaviour, INetworkTriggerable
{
    [SerializeField] public Animator hoverEButtonUI;
    [SerializeField] public GameObject canvasImage;

    private bool isActive;

    private void Awake()
    {
        isActive = false;
    }

    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this Credits Board
    /// </summary>
    public void Trigger(NetworkStudentController currentPlayer)
    {
        isActive = !isActive;
        canvasImage.SetActive(isActive);
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
        
        isActive = false;
        canvasImage.SetActive(isActive);
    }
    
    #endregion
}