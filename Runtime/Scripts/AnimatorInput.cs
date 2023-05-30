using UnityEngine;
using UnityEngine.InputSystem;

namespace Tutorials.RootMotion
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class AnimatorInput : MonoBehaviour
    {
        [field: SerializeField, HideInInspector] public Animator animator { get; private set; }
        [field: SerializeField, HideInInspector] public new Rigidbody rigidbody { get; private set; }

        public Transform lookPivot;
        [Range(0, 180f)] public float lookSpeed = 180f;

        private Vector2 _movement;
        private bool _isSprinting;
        private float _lookYaw;
        private float _lookPitch;

        private void InitializeComponents()
        {
            animator = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Reset()
        {
            InitializeComponents();
        }

        private void Awake()
        {
            InitializeComponents();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void OnMove(InputValue value)
        {
            _movement = value.Get<Vector2>();
        }

        private void OnLook(InputValue value)
        {
            var look = Time.deltaTime * lookSpeed * -value.Get<Vector2>();

            _lookYaw += look.x;
            _lookPitch += look.y * 0.5f;
            _lookPitch = Mathf.Clamp(_lookPitch, -15f, 70f);

            lookPivot.rotation = Quaternion.Euler(_lookPitch, _lookYaw, 0f);
        }

        private void OnSprint(InputValue value)
        {
            _isSprinting = value.isPressed;
        }

        private void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                animator.SetTrigger("Jump");
            }
        }

        private void Update()
        {
            // Set Speed
            var speed = _movement.magnitude;
            if (!_isSprinting)
            {
                speed *= 0.1f;
            }
            animator.SetFloat("Speed", speed, 0.025f, Time.deltaTime);

            if (speed > 0f)
            {
                // Set Turn
                var turnDirection = Vector3.ProjectOnPlane(lookPivot.forward, transform.up); // Look Direction
                var planarRotation = Quaternion.LookRotation(turnDirection != Vector3.zero ? turnDirection : Vector3.forward, transform.up);
                var direction = planarRotation * new Vector3(_movement.x, 0f, _movement.y).normalized;
                var turn = Vector3.SignedAngle(transform.forward, direction, transform.up);

                // Set parameters based on if we're pivoting or not.
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Pivot"))
                {
                    animator.SetFloat("Turn", turn / 180f, 0.25f, Time.deltaTime);
                    animator.SetFloat("Pivot", turn / 180f);
                }
                else
                {
                    animator.SetFloat("Turn", turn / 180f);
                }
            }
        }
    }
}
