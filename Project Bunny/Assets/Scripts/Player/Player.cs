using UnityEngine;
using Mirror;

namespace Player
{
    public class Player : NetworkBehaviour
    {
        public static Player local;

        [SerializeField] private float moveSpeed;

        private new Rigidbody rigidbody;
        private new Camera camera;

        private Vector3 movementInput;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            if (isLocalPlayer)
            {
                local = this;
                camera = Camera.main;
            }
        }

        void Update()
        {
            if (isLocalPlayer)
                movementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        private void FixedUpdate()
        {
            if (isLocalPlayer)
                rigidbody.velocity = moveSpeed * movementInput;
        }

        private void LateUpdate()
        {
            if (isLocalPlayer && camera)
                camera.transform.position = transform.position + new Vector3(0, 3, -5);
        }
    }
}
