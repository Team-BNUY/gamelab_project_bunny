using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    private AudioSource _audioSource;
    private bool _muted;

    public bool Muted
    {
        set => _muted = value;
    }

    private void Awake()
    {
        // Don't make it a real singleton
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (_muted) return;
        
        _audioSource.PlayOneShot(clip, volume);
    }

    public void Play(AudioClip clip, float volume = 1f, bool loop = false)
    {
        if (_muted) return;
        
        _audioSource.clip = clip;
        _audioSource.loop = loop;
        _audioSource.Play();
    }

    public void PlayFollowedBy(AudioClip firstClip, AudioClip secondClip, bool loop)
    {
        if (_muted) return;

        _audioSource.clip = firstClip;
        _audioSource.loop = false;
        StartCoroutine(PlaySecondClip(secondClip, loop));
    }

    public void Stop()
    {
        if (_muted) return;
        
        _audioSource.Stop();
    }

    private IEnumerator PlaySecondClip(AudioClip clip, bool loop)
    {
        while (_audioSource.isPlaying)
        {
            yield return null;
        }

        _audioSource.clip = clip;
        _audioSource.loop = loop;
        _audioSource.Play();
    }
}
