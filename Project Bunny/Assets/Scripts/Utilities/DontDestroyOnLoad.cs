using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public GameObject _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = gameObject;
        }
        else if (_instance != gameObject)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
