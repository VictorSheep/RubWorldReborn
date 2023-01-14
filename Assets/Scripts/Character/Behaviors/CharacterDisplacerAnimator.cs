using Rubworld.Gravity.Behaviors;
using TMPro;
using UnityEngine;

namespace Rubworld.Character.Behavior
{
    [RequireComponent(typeof(CharacterDisplacer), typeof(GravityApplier))]
    public class CharacterDisplacerAnimator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator _animator;
        
        [Header("Autofill")]
        [SerializeField, DependencyInjection.ReadOnly]
        private CharacterDisplacer _characterDisplacer;
        [SerializeField, DependencyInjection.ReadOnly]
        private GravityApplier _gravityApplier;

        private bool _isFalling;
        private bool _isWalking;
        private static readonly int _animatorBoolIsFalling = Animator.StringToHash("IsFalling");
        private static readonly int _animatorBoolIsWalking = Animator.StringToHash("IsWalking");

#if UNITY_EDITOR
        private void OnValidate()
        {
            _characterDisplacer = GetComponent<CharacterDisplacer>();
            _gravityApplier     = GetComponent<GravityApplier>();
        }
#endif

        private void Update()
        {
            UpdateIsFalling();
            UpdateIsWalking();
        }

        private void UpdateIsFalling()
        {
            if (_isFalling == _gravityApplier.ShouldApplyGravity)
            {
                return;
            }

            _isFalling = _gravityApplier.ShouldApplyGravity;
            _animator.SetBool(_animatorBoolIsFalling, _isFalling);
        }

        private void UpdateIsWalking()
        {
            if (_isWalking == _characterDisplacer.IsMoving)
            {
                return;
            }

            _isWalking = _characterDisplacer.IsMoving;
            _animator.SetBool(_animatorBoolIsWalking, _isWalking);
        }
    }
}