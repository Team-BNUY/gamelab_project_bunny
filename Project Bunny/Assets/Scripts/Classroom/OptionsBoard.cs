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
    public void TriggerableTrigger(NetworkStudentController currentStudentController)
    {
        Debug.Log("You just triggered the Options Board!");
    }
    
    public void TriggerableEnter()
    {
        hoverEButtonUI.enabled = true;
        hoverEButtonUI.StartPlayback();
        hoverEButtonUI.gameObject.SetActive(true);
    }

    public void TriggerableExit()
    {
        hoverEButtonUI.StopPlayback();
        hoverEButtonUI.enabled = false;
        hoverEButtonUI.gameObject.SetActive(false);
    }
    
    #endregion
}