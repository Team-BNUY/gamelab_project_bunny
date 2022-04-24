using System;
using Interfaces;
using Player;
using UnityEngine;

public class TutorialBoard : MonoBehaviour, INetworkTriggerable
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
    public void TriggerableTrigger(NetworkStudentController currentStudentController)
    {
        isActive = !isActive;
        canvasImage.SetActive(isActive);
    }
    
    public void TriggerableEnter()
    {
        hoverEButtonUI.enabled = true;
        hoverEButtonUI.gameObject.SetActive(true);
        hoverEButtonUI.Play("EInteract");
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