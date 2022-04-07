using Interfaces;
using Player;
using UnityEngine;

public class BoomboxController : MonoBehaviour, INetworkTriggerable
{
    [SerializeField] private Animator hoverEButtonUI;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ParticleSystem _particleSystem;
 
    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this boombox
    /// </summary>
    public void Trigger(NetworkStudentController currentPlayer)
    {
        var listener = currentPlayer.PlayerCamera.GetComponent<AudioListener>();
        listener.enabled = !listener.enabled;

        if (listener.enabled)
        {
            _audioSource.Play();
            _particleSystem.Play();
            FindObjectOfType<AudioManager>().Muted = false;
        }
        else
        {
            _audioSource.Stop();
            _particleSystem.Stop();
            FindObjectOfType<AudioManager>().Muted = true;
        }
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