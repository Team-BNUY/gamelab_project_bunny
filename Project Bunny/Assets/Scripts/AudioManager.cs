using Photon.Pun;
using UnityEngine;

public class AudioManager : MonoBehaviourPunCallbacks
{
    public static AudioManager Instance;

    [SerializeField] private AudioClip[] syncClips;
    
    private AudioSource _audioSource;
    private bool _muted;
    private float _volume = 1.0f;

    public bool Muted
    {
        set => _muted = value;
    }

    public float Volume
    {
        get => _volume;
        set => _volume = value;
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
        
        _audioSource.PlayOneShot(clip, _volume * volume);
    }

    public void Play(AudioClip clip, float volume = 1f, bool loop = false)
    {
        if (_muted) return;
        
        _audioSource.clip = clip;
        _audioSource.volume = _volume * volume;
        _audioSource.loop = loop;
        _audioSource.Play();
    }

    public void Stop()
    {
        _audioSource.Stop();
    }

    public void PlaySync(int clipID, float volume)
    {
        if (clipID >= syncClips.Length) return;

        photonView.RPC(nameof(PlaySyncRPC), RpcTarget.All, clipID, _volume * volume);
    }

    [PunRPC]
    private void PlaySyncRPC(int clipID, int volume)
    {
        var clip = syncClips[clipID];
        _audioSource.volume = _volume * volume;
        PlayOneShot(clip, volume);
    }
}
