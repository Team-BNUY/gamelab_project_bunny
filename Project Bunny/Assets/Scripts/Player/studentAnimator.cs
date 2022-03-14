using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class studentAnimator : MonoBehaviour
    {

        [SerializeField] private Animator _animator;
        [SerializeField] private StudentController _studentController;


        public void ThrowSnowball()
        {
            _studentController.ThrowStudentSnowball();
        }
    }

}


