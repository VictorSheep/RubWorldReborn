using UnityEngine;

namespace Rubworld.SwitchBloc.Data
{
    [CreateAssetMenu(menuName = "Data/SwitchBloc/SwitchBlocData", fileName = "SwitchBlocData")]
    public class SwitchBlocData : ScriptableObject
    {
        [SerializeField]
        protected Material _normalMaterial = null;
        [SerializeField]
        protected Material _triggeredMaterial = null;

        public Material TriggeredMaterial => _triggeredMaterial;
        public Material NormalMaterial => _normalMaterial;
    }
}