using Rubworld.Gravity.Behaviors;
using UnityEngine;

namespace Rubworld.Player.Behaviors
{
    public class PlayerBlocRotController : MonoBehaviour
    {
        private BlocRot.Behaviors.BlocRot _currentBlocRot;

        [Header("Autofill")]
        [SerializeField, DependencyInjection.ReadOnly]
        private GravityApplier _gravityApplier;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            _gravityApplier = GetComponent<GravityApplier>();
        }
#endif
        
        public void SetBlocRot(BlocRot.Behaviors.BlocRot blocRot)
        {
            _currentBlocRot = blocRot;
        }
        public void RemoveBlocRot()
        {
            _currentBlocRot = null;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if (_currentBlocRot)
                {
                    _currentBlocRot.Rotate(_gravityApplier);
                }
            }
        }
    }
}