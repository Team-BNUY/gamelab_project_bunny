using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public GameObject _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this.gameObject;
        }
        else if (_instance != this.gameObject)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
