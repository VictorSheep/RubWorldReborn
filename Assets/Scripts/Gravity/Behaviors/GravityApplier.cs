using System;
using UnityEngine;

namespace Rubworld.Gravity.Behaviors
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityApplier : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _gravityDirection = Vector3.down;
        [SerializeField]
        private float   _gravityForce     = 7f;
        
        // PUBLIC
        public Vector3 GravityDirection => _gravityDirection;
        public float   GravityForce     => _gravityForce;
        public bool    IsFalling        => _isFalling;

        [Header("Autofill")]
        [SerializeField, DependencyInjection.ReadOnly]
        private Rigidbody _rigidbody;

        private float _previousPosition;
        private bool _isFalling;

#if UNITY_EDITOR
        private void OnValidate()
        {
            AutoSetGravityFromRotation();
            _gravityDirection = _gravityDirection.normalized;
            _rigidbody = GetComponent<Rigidbody>();
            _previousPosition = GetCurrentGravityPosition();
        }
#endif
        
        // Update is called once per frame
        void Update()
        {
            _rigidbody.AddForce(_gravityDirection * _gravityForce * _rigidbody.mass, ForceMode.Force);

            _isFalling = Math.Abs(GetCurrentGravityPosition() - _previousPosition) > 0.1f;
            
            _previousPosition = GetCurrentGravityPosition();
        }

        private float GetCurrentGravityPosition()
        {
            if (_gravityDirection.x > 0)
            {
                return _rigidbody.position.x;
            }
            
            if (_gravityDirection.y > 0)
            {
                return _rigidbody.position.y;
            }
            
            return _rigidbody.position.z;

        }

        public void SetGravityDirection(Vector3 gravityDirection)
        {
            _gravityDirection = gravityDirection;
        }

        public void AutoSetGravityFromRotation()
        {
            Vector3 gravityDirection = transform.up;
            gravityDirection.x = Mathf.Abs(gravityDirection.x) < 0.5 ? 0 : Mathf.Round(gravityDirection.x);
            gravityDirection.y = Mathf.Abs(gravityDirection.y) < 0.5 ? 0 : Mathf.Round(gravityDirection.y);
            gravityDirection.z = Mathf.Abs(gravityDirection.z) < 0.5 ? 0 : Mathf.Round(gravityDirection.z);
            SetGravityDirection(-gravityDirection);
        }
    }
}