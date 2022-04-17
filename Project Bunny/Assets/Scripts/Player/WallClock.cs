using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClock : MonoBehaviour
{
    [SerializeField] private Transform _hourHand;
    [SerializeField] private Transform _minuteHand;
    [SerializeField] private Transform _secondHand;

    private float _timer;
    private float _secondDegrees = 6f;
    private float _minuteDegrees = 6f/60f;
    private float _hourDegrees = 6f/360f;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        SetTimer();
    }

    private void SetTimer()
    {
        if (_timer >= 1f)
        {
            TickClock();
            _timer = 0f;
        }
        else
        {
            _timer += Time.fixedDeltaTime;
        }
    }
    
    private void TickClock()
    {
        _secondHand.rotation *= Quaternion.Euler(0f, 0f, _secondDegrees);
        _minuteHand.rotation *= Quaternion.Euler(0f, 0f, _minuteDegrees);
        _hourHand.rotation *= Quaternion.Euler(0f, 0f, _hourDegrees);
    }
}
