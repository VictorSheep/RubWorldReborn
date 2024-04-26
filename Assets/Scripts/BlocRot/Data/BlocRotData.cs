using UnityEngine;

namespace Rubworld.BlocRot.Data
{
    [CreateAssetMenu(menuName = "Data/BlocRot/BlocRotData", fileName = "BlocRotData")]
    public class BlocRotData : ScriptableObject
    {
        [SerializeField]
        protected float _rotationDuration = 0.3f;
        
        public virtual float RotationDuration => _rotationDuration;
    }
}