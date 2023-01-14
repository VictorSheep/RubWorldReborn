using Rubworld.Gravity.Behaviors;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rubworld.Player.Behaviors
{
    public class PlayerBlocRotController : MonoBehaviour
    {
        private BlocRot.Behaviors.BlocRot _currentBlocRot;
        private ThirdPersonActionAsset _playerActionsAsset;

        [Header("Autofill")]
        [SerializeField, DependencyInjection.ReadOnly]
        private GravityApplier _gravityApplier;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            _gravityApplier = GetComponent<GravityApplier>();
        }
#endif
        
        private void Awake()
        {
            _playerActionsAsset = new ThirdPersonActionAsset();
            _gravityApplier = GetComponent<GravityApplier>();
        }

        private void OnEnable()
        {
            _playerActionsAsset.Player.Rotate.started += DoRotate;
            _playerActionsAsset.Player.Enable();
        }

        private void OnDisable()
        {
            _playerActionsAsset.Player.Rotate.started -= DoRotate;
            _playerActionsAsset.Player.Disable();
        }

        private void DoRotate(InputAction.CallbackContext callbackContext)
        {
            if (_gravityApplier.ShouldApplyGravity)
            {
                return;
            }
            if (_currentBlocRot)
            {
                _currentBlocRot.Rotate(_gravityApplier);
            }
            Debug.Log($"[FIX] Rotate.callbackContext\n" +
                      $"{{\n" +
                      $"  action : {callbackContext.action}\n" +
                      $"  canceled : {callbackContext.canceled}\n" +
                      $"  control : {callbackContext.control}\n" +
                      $"  duration : {callbackContext.duration}\n" +
                      $"  interaction : {callbackContext.interaction}\n" +
                      $"  performed : {callbackContext.performed}\n" +
                      $"  phase : {callbackContext.phase}\n" +
                      $"  started : {callbackContext.started}\n" +
                      $"  time : {callbackContext.time}\n" +
                      $"  startTime : {callbackContext.startTime}\n" +
                      $"  valueType : {callbackContext.valueType}\n" +
                      $"  valueSizeInBytes : {callbackContext.valueSizeInBytes}\n" +
                      $"}}");
        }
        
        public void SetBlocRot(BlocRot.Behaviors.BlocRot blocRot)
        {
            _currentBlocRot = blocRot;
        }
        public void RemoveBlocRot()
        {
            _currentBlocRot = null;
        }
    }
}