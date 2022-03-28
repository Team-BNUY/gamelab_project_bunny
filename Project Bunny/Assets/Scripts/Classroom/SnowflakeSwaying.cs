using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowflakeSwaying : MonoBehaviour
{   
    // Speed of rotation
    float rotationSpeed = 0.4f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        // Change rotation direction randomly
        if (Random.value < 0.01)
        {
            rotationSpeed *= -1;
        }

        // Rotate snowflake
        transform.Rotate(0, 0, rotationSpeed);
    }
}