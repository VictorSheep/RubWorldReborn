using Rubworld.Gravity.Behaviors;
using UnityEngine;

namespace Rubworld.Spawner.Behaviors
{
    public class InteractableCubeSpawner : MonoBehaviour
    {
        [SerializeField] private GravityApplier _currentCube;
        [SerializeField] private Vector3 _gravityDirection;
        
        [SerializeField, HideInInspector] private Vector3 _respawnGravityDirection;
        [SerializeField, HideInInspector] private Vector3 _respawnPosition;
        [SerializeField, HideInInspector] private Quaternion _respawnRotation;
        [SerializeField, HideInInspector] private Rigidbody _currentCubeRigidbody;
        [SerializeField, HideInInspector] private GravityApplier _currentCubeGravityApplier;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_currentCube)
            {
                _currentCubeRigidbody      = _currentCube.GetComponent<Rigidbody>();
                _currentCubeGravityApplier = _currentCube.GetComponent<GravityApplier>();
                _respawnPosition           = _currentCube.transform.position;
                _respawnRotation           = _currentCube.transform.rotation;
            }
            _respawnGravityDirection = _gravityDirection;
        }
#endif
        
        public void SpawnCube()
        {
            _currentCubeRigidbody.velocity = Vector3.zero;
            _currentCubeRigidbody.angularVelocity = Vector3.zero;
            _currentCube.transform.position = _respawnPosition;
            _currentCube.transform.rotation = _respawnRotation;
            _currentCubeGravityApplier.SetGravityDirection(_respawnGravityDirection);
        }
    }
}