using Rubworld.Gravity.Behaviors;
using UnityEngine;

namespace Rubworld.Character.Behavior
{
    [RequireComponent(typeof(Rigidbody), typeof(GravityApplier), typeof(CharacterDisplacer))]
    public class CharacterRotator : MonoBehaviour
    {
        [SerializeField]
        private Transform _pivot;
        
        [Header("Autofill")]
        [SerializeField, DependencyInjection.ReadOnly]
        private Rigidbody _rigidbody;
        [SerializeField, DependencyInjection.ReadOnly]
        private GravityApplier _gravityApplier;
        [SerializeField, DependencyInjection.ReadOnly]
        private CharacterDisplacer _characterDisplacer;

        //PRIVATE
        private Vector3 _lookAtTarget;
        private Vector3 _previousLookAtTarget;
        

#if UNITY_EDITOR
        private void OnValidate()
        {
            _rigidbody          = GetComponent<Rigidbody>();
            _gravityApplier     = GetComponent<GravityApplier>();
            _characterDisplacer = GetComponent<CharacterDisplacer>();
        }
#endif
        private void Awake()
        {
            _lookAtTarget = transform.position + Vector3.forward;
            _previousLookAtTarget = _lookAtTarget;
        }

        private void Update()
        {
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            _lookAtTarget = transform.position + _characterDisplacer.Direction;
            if (_characterDisplacer.IsMoving)
            {
                _pivot.LookAt(Vector3.Lerp(_previousLookAtTarget, _lookAtTarget, 0.2f));
                _pivot.localRotation = Quaternion.Euler(0, _pivot.localEulerAngles.y, 0);
            }
            else
            {
                _pivot.LookAt(_lookAtTarget);
                _pivot.localRotation = Quaternion.Euler(0, _pivot.localEulerAngles.y, 0);
            }
            _previousLookAtTarget = _lookAtTarget;
        }
        
    }
}