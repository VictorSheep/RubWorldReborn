using System;
using Rubworld.Character.Behavior;
using UnityEngine;
using UnityEngine.Serialization;

namespace Rubworld.Gravity.Behaviors
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityApplier : MonoBehaviour
    {
        [Header("Base parameters")]
        [SerializeField]
        private Vector3 _gravityDirection = Vector3.down;
        [SerializeField]
        private float   _gravityForce     = 7f;

        [Header("FloorDetection")]
        [FormerlySerializedAs("Enable")]
        [SerializeField] private bool _floorDetectionEnabled = true;
        [SerializeField] private float _maxDistance = .23f;
        [SerializeField] private float _start       = 1f;
        [SerializeField] private float _radius      = .5f;
        private CastType _castType                  = CastType.Box;
        [SerializeField] private LayerMask _buttonLayerMask;
        [SerializeField] private LayerMask _layerMask;

        // PUBLIC
        public Vector3 GravityDirection => _gravityDirection;
        public Vector3 NormalizedGravityDirection
        {
            get
            {
                Vector3 result = new Vector3(0f, 0f, 0f)
                {
                    x = Mathf.Abs(_gravityDirection.x),
                    y = Mathf.Abs(_gravityDirection.y),
                    z = Mathf.Abs(_gravityDirection.z)
                };
                return result.normalized;
            }
        }

        public float GravityForce                  => _gravityForce;
        public float CurrentGravityVelocityApplied => _currentGravityVelocityApplied;
        public bool  ShouldApplyGravity            => _shouldApplyGravity;

        [Header("Autofill")]
        [SerializeField, DependencyInjection.ReadOnly]
        private Rigidbody _rigidbody;

        private float _currentGravityVelocityApplied;
        private float _previousPosition;
        private bool _shouldApplyGravity;
        private bool _forceApplyGravity;
        private bool _previousFloorDetectionEnabled;
        private float fdsave_maxDistance; // floor detection save
        private float fdsave_start; // floor detection save
        private float fdsave_radius; // floor detection save

#if UNITY_EDITOR
        private void OnValidate()
        {
            AutoSetGravityFromRotation();
            _gravityDirection = _gravityDirection.normalized;
            _rigidbody = GetComponent<Rigidbody>();
            _previousPosition = GetCurrentGravityPosition();

            if (_previousFloorDetectionEnabled && _floorDetectionEnabled == false)
            {
                fdsave_maxDistance = _maxDistance;
                fdsave_start       = _start;
                fdsave_radius      = _radius;
                _maxDistance = 0f;
                _start       = 0f;
                _radius      = 0f;
            }
            
            _previousFloorDetectionEnabled = _floorDetectionEnabled;
        }
#endif
        
        void Update()
        {
            UpdateShouldApplyGravity();
            if (_shouldApplyGravity || _forceApplyGravity)
            {
                _rigidbody.AddForce(_gravityDirection * _gravityForce * _rigidbody.mass, ForceMode.Force);
                _currentGravityVelocityApplied = GetCurrentGravityVelocityApplied();
            }
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

        private float GetCurrentGravityVelocityApplied()
        {
            if (_gravityDirection.x > 0)
            {
                return _rigidbody.velocity.x;
            }
            
            if (_gravityDirection.y > 0)
            {
                return _rigidbody.velocity.y;
            }
            
            return _rigidbody.velocity.z;

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
        
        private void UpdateShouldApplyGravity()
        {
            if (_floorDetectionEnabled == false)
            {
                _shouldApplyGravity = true;
                _forceApplyGravity = true;
                return;
            }
            RaycastHit m_Hit;
            var center      = transform.position + transform.up * _start;
            var halfExtents = transform.localScale * _radius;
            var direction   = -transform.up;
            _forceApplyGravity = Physics.BoxCast(center, halfExtents, direction, out m_Hit, Quaternion.identity, _maxDistance, _buttonLayerMask);
            _shouldApplyGravity = !Physics.BoxCast(center, halfExtents, direction, out m_Hit, Quaternion.identity, _maxDistance, _layerMask);
            ExtDebug.DrawBoxCastBox(center, halfExtents, direction, Quaternion.identity, _maxDistance, Color.blue);
            
        }

        public Vector3 GetGravityVelocityValue(Vector3 v)
        {
            Vector3 normalizedGravityDirection = NormalizedGravityDirection;
            v.x *= normalizedGravityDirection.x;
            v.y *= normalizedGravityDirection.y;
            v.z *= normalizedGravityDirection.z;
            return v;
        }

        public Vector3 RemoveGravityVelocity(Vector3 v)
        {
            v.x = _gravityDirection.x == 0 ? v.x : 0f;
            v.y = _gravityDirection.y == 0 ? v.y : 0f;
            v.z = _gravityDirection.z == 0 ? v.z : 0f;
            return v;
        }

        public Vector3 ConvertToCurrentGravity(Vector3 v)
        {
            if (_gravityDirection.y != 0)
            {
                return v;
            }
            
            float baseX = v.x;
            float baseY = v.y;
            float baseZ = v.z;
            
            if (_gravityDirection.x != 0)
            {
                return new Vector3(baseY, baseX, baseZ);
            }
            
            if (_gravityDirection.z != 0)
            {
                return new Vector3(baseX, baseZ, baseY);
            }
            
            return v;
        }

        public Vector3 ApplyGravityVelocity(Vector3 v, float multiplier = 1f)
        {
            v.x = _gravityDirection.x == 0 ? v.x : _currentGravityVelocityApplied * multiplier;
            v.y = _gravityDirection.y == 0 ? v.y : _currentGravityVelocityApplied * multiplier;
            v.z = _gravityDirection.z == 0 ? v.z : _currentGravityVelocityApplied * multiplier;
            return v;
        }
    }
}