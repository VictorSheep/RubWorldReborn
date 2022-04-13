using System;
using Rubworld.Character.Data;
using Rubworld.Gravity.Behaviors;
using UnityEngine;

namespace Rubworld.Character.Behavior
{
    public enum CastType
    {
        Box, Sphere
    }
    [RequireComponent(typeof(Rigidbody), typeof(GravityApplier))]
    public class CharacterDisplacer : MonoBehaviour
    {
        [SerializeField]
        private CharacterDisplacerData _characterDisplacerData;
        [SerializeField]
        private Transform _pivot;
        
        [SerializeField]
        private Vector3 _direction = Vector3.zero;

        public Vector3 Direction => _direction;
        
        public bool IsOnTheGround => _isOnTheGround;
        public bool IsMoving => _isMoving;

        [Header("Autofill")]
        [SerializeField, DependencyInjection.ReadOnly]
        private Rigidbody _rigidbody;
        [SerializeField, DependencyInjection.ReadOnly]
        private GravityApplier _gravityApplier;

        // PRIVATE
        private bool _isMoving;
        private bool _isOnTheGround;
        // for boxcast
        [SerializeField] private float m_MaxDistance = .23f;
        [SerializeField] private float _start        = 1f;
        [SerializeField] private float _radius       = .5f;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private CastType _castType = CastType.Box;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _rigidbody      = GetComponent<Rigidbody>();
            _gravityApplier = GetComponent<GravityApplier>();
        }
#endif

        public void SetDirection(Vector3 direction)
        {
            _direction = direction;
        }

        private void Update()
        {
            UpdateIsOnTheGround();
            if (_isMoving && !_isOnTheGround)
            {
                _isMoving = false;
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.MovePosition(_rigidbody.position);
                return;
            }

            if (!_isOnTheGround)
            {
                _isMoving = false;
                return;
            }

            _isMoving = _direction != Vector3.zero;
            _rigidbody.velocity = _direction * _characterDisplacerData.speed;
        }

        private void UpdateIsOnTheGround()
        {
            RaycastHit m_Hit;
            var center      = transform.position + transform.up * _start;
            var halfExtents = transform.localScale * _radius;
            var direction   = -transform.up;
            if (_castType == CastType.Box)
            {
                _isOnTheGround = Physics.BoxCast(center, halfExtents, direction, out m_Hit, Quaternion.identity, m_MaxDistance, _layerMask);
                ExtDebug.DrawBoxCastBox(center, halfExtents, direction, Quaternion.identity, m_MaxDistance, Color.blue);
            }
            else
            {
                _isOnTheGround = Physics.SphereCast(center, _radius, direction, out m_Hit, m_MaxDistance, _layerMask);
                ExtDebug.DrawSphereCastBox(center, _radius, direction, m_MaxDistance, Color.blue);
            }
            
        }
    }
}