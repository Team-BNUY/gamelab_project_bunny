using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    private Vector3 velocity;
    
    private void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");
        velocity = new Vector3(x, 0f, z).normalized;
        
        transform.Translate(velocity * Time.deltaTime * speed);
    }
}
