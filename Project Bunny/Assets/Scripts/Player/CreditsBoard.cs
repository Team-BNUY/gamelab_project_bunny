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
    public void TriggerableTrigger(NetworkStudentController currentPlayer)
    {
        isActive = !isActive;
        canvasImage.SetActive(isActive);
    }
    
    public void TriggerableEnter()
    {
        hoverEButtonUI.enabled = true;
        hoverEButtonUI.Play("EInteract");
        hoverEButtonUI.gameObject.SetActive(true);
    }

    public void TriggerableExit()
    {
        hoverEButtonUI.enabled = false;
        hoverEButtonUI.gameObject.SetActive(false);
        
        isActive = false;
        canvasImage.SetActive(isActive);
    }
    
    #endregion
}