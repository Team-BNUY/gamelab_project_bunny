using System;
using Interfaces;
using Player;
using UnityEngine;

public class BoomboxController : MonoBehaviour, INetworkTriggerable
{
    [SerializeField] public Animator hoverEButtonUI;
    [SerializeField] public AudioClip[] songs;
    [SerializeField] public AudioSource source;
 
    private AudioClip _currentSong;
    private int songIndex;

    private void Awake()
    {
        if (songs.Length != 0)
        {
            _currentSong = songs[0];
        }

        songIndex = 0;
        source.clip = _currentSong;
        source.Play();
    }

    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this boombox
    /// </summary>
    public void Trigger(NetworkStudentController currentPlayer)
    {
        if (songs.Length == 0) return;
        
        source.Stop();

        if (songIndex + 1 >= (songs.Length)) songIndex = 0;
        else songIndex++;

        _currentSong = songs[songIndex];
        
        source.clip = _currentSong;
        source.Play();

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