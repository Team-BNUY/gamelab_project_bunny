using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class NetworkStudentAnimator : MonoBehaviour
    {

        [SerializeField] private Animator _animator;
        [SerializeField] private NetworkStudentController _studentController;


        public void ThrowSnowball()
        {
            Debug.Log("Throw!");
            _studentController.ThrowStudentSnowball();
        }

        public void SetWalking()
        {
            _studentController.SetWalkingAnimator();
        }
    }

}


