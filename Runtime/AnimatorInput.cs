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
            var speed = _movement.magnitude;
            if (speed < 0.01f)
            {
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                if (!_isSprinting)
                {
                    speed *= 0.1f;
                }
                animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
            }

            //animator.SetFloat("Turn", movemenent.x);

            Debug.Log(animator.gravityWeight);
        }
    }
}
