using UnityEngine;


namespace Player
{
    public class NetworkStudentAnimator : MonoBehaviour
    {
        [SerializeField] private NetworkStudentController _studentController;

        public void ThrowSnowball()
        {
            _studentController.ThrowStudentSnowball();
        }

        public void SetThrewSnowball(bool value)
        {
            _studentController.SetThrewSnowball(value);
        }
    }

}


