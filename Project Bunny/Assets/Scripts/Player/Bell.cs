using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bell : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _ringWaves;
    
    public void RingBellRPC()
    {
        StartCoroutine(RingBell());
    }
    
    private IEnumerator RingBell()
    {
        _animator.enabled = true;
        _animator.Play("Ring");
        _ringWaves.Play();
        yield return new WaitForSeconds(2.3f);
        _ringWaves.Stop();
        _animator.enabled = false;
    }
}
