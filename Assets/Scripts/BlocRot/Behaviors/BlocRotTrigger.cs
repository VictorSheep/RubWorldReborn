using Rubworld.Player.Behaviors;
using UnityEngine;

namespace Rubworld.BlocRot.Behaviors
{
    public class BlocRotTrigger : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _axis;

        [Header("Autofill")]
        [SerializeField, DependencyInjection.ReadOnly]
        private BlocRot _blocRot;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _blocRot = GetComponentInParent<BlocRot>();
        }
#endif

        private void OnTriggerEnter(Collider other)
        {
            _blocRot.AxisTrigger(_axis);
            other.gameObject.GetComponentInParent<PlayerBlocRotController>().SetBlocRot(_blocRot);
        }
        private void OnTriggerExit(Collider other)
        {
            other.gameObject.GetComponentInParent<PlayerBlocRotController>().RemoveBlocRot();
        }
    }
}