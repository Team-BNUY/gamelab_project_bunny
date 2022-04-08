using System;
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
        var volume = AudioManager.Instance.Volume;
        if (Math.Abs(volume - 1f) < 0.01f)
        {
            _audioSource.volume = 0.75f;
            AudioManager.Instance.Volume = 0.75f;
            AudioManager.Instance.Muted = false;
        }
        else if (Mathf.Abs(volume - 0.75f) < 0.01f)
        {
            _audioSource.volume = 0.5f;
            AudioManager.Instance.Volume = 0.5f;
            AudioManager.Instance.Muted = false;
        }
        else if (Mathf.Abs(volume - 0.5f) < 0.01f)
        {

            _audioSource.volume = 0.25f;
            AudioManager.Instance.Volume = 0.25f;
            AudioManager.Instance.Muted = false;
        }
        else if (Mathf.Abs(volume - 0.25f) < 0.01f)
        {
            AudioManager.Instance.Volume = 0.0f;
            AudioManager.Instance.Muted = true;
            AudioManager.Instance.Stop();
            _audioSource.volume = 0.0f;
            _audioSource.Stop();
            _particleSystem.Stop();
            
            return;
        }
        else
        {
            _audioSource.volume = 1.0f;
            AudioManager.Instance.Volume = 1.0f;
            AudioManager.Instance.Muted = false;
        }

        if (_audioSource.isPlaying) return;
        
        _audioSource.Play();
        _particleSystem.Play();
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