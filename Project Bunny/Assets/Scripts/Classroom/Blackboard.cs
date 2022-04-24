using Interfaces;
using Player;
using UnityEngine;

public class Blackboard : MonoBehaviour, INetworkTriggerable
{
    [SerializeField] public Animator hoverEButtonUI;
    
    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this blackboard
    /// </summary>
    public void TriggerableTrigger(NetworkStudentController currentStudentController)
    {
        Debug.Log("You just triggered the Blackboard!");
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
