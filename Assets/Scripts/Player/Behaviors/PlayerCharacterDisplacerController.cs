using Rubworld.Character.Behavior;
using Rubworld.Gravity.Behaviors;
using UnityEngine;

namespace Rubworld.Player.Behaviors
{
    [RequireComponent(typeof(CharacterDisplacer), typeof(GravityApplier))]
    public class PlayerCharacterDisplacerController : MonoBehaviour
    {
        [SerializeField]
        private AnimationCurve _accelerationCurve;
        
        [Header("Autofill")]
        [SerializeField, DependencyInjection.ReadOnly]
        private CharacterDisplacer _characterDisplacer;
        [SerializeField, DependencyInjection.ReadOnly]
        private GravityApplier _gravityApplier;

        // PRIVATE
        private Vector3 _horizontalAxis;
        private Vector3 _verticalAxis;
        private Vector3 _previousGravity;
        private float _horizontalForce;
        private float _verticalForce;
        private float _previousHorizontalForce;
        private float _previousVerticalForce;
        private float _finaleHorizontalForce;
        private float _finaleVerticalForce;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _characterDisplacer = GetComponent<CharacterDisplacer>();
            _gravityApplier     = GetComponent<GravityApplier>();
        }
#endif
        
        private void Update()
        {
            UpdateDisplacementAxisFromGravity();
            UpdateDirectionFromInput();
        }

        private void UpdateDisplacementAxisFromGravity()
        {
            if (_previousGravity == _gravityApplier.GravityDirection)
            {
                return;
            }
            
            if (_gravityApplier.GravityDirection.x != 0)
            {
                _horizontalAxis = Vector3.Normalize(Vector3.back + Vector3.up * 0.11f);
                _verticalAxis   = Vector3.up;
            }
            else if (_gravityApplier.GravityDirection.y != 0)
            {
                _horizontalAxis = -Vector3.Normalize(Vector3.forward + Vector3.left);
                _verticalAxis   = Vector3.Normalize(Vector3.right + Vector3.forward);
            }
            else if (_gravityApplier.GravityDirection.z != 0)
            {
                _horizontalAxis = Vector3.Normalize(Vector3.right + Vector3.down * 0.11f);
                _verticalAxis   = Vector3.up;
            }
            _previousGravity = _gravityApplier.GravityDirection;
        }

        private void UpdateDirectionFromInput()
        {
            if (_gravityApplier.IsFalling)
            {
                _characterDisplacer.SetDirection(Vector3.zero);
                return;
            }
            _horizontalForce = Input.GetAxis("Horizontal");
            _verticalForce   = Input.GetAxis("Vertical");

            //if (_horizontalForce == 0)
            //{
            //    _finaleHorizontalForce = 0f;
            //}
            //else
            //{
            //    float abs = Mathf.Abs(_horizontalForce);
            //    _finaleHorizontalForce = abs < _previousHorizontalForce
            //        ? 0
            //        : _accelerationCurve.Evaluate(abs) * Mathf.Sign(_horizontalForce);
            //}
//
            //if (_verticalForce == 0f)
            //{
            //    _finaleVerticalForce = 0f;
            //}
            //else
            //{
            //    float abs = Mathf.Abs(_verticalForce);
            //    _finaleVerticalForce = abs < _previousVerticalForce
            //        ? 0
            //        : _accelerationCurve.Evaluate(abs) * Mathf.Sign(_verticalForce);
            //}
            
            _finaleHorizontalForce = _horizontalForce;
            _finaleVerticalForce   = _verticalForce;

            if (UseDigitalDirection())
            {
                Vector2 normalizedDirection = new Vector2(_finaleHorizontalForce, _finaleVerticalForce).normalized;
                _finaleHorizontalForce = normalizedDirection.x;
                _finaleVerticalForce = normalizedDirection.y;
            }
            
            _previousHorizontalForce = Mathf.Abs(_horizontalForce);
            _previousVerticalForce   = Mathf.Abs(_verticalForce);

            Vector3 wantedDirection = _horizontalAxis * _finaleHorizontalForce + _verticalAxis * _finaleVerticalForce;
            _characterDisplacer.SetDirection(wantedDirection);
        }

        private bool UseDigitalDirection()
        {
            return Input.GetKey(KeyCode.DownArrow)
                || Input.GetKey(KeyCode.UpArrow)
                || Input.GetKey(KeyCode.LeftArrow)
                || Input.GetKey(KeyCode.RightArrow);
        }
    }
}