using Rubworld.Gravity.Behaviors;
using UnityEngine;

namespace Rubworld.Character.Behavior
{
    [RequireComponent(typeof(Rigidbody), typeof(GravityApplier))]
    public class CharacterRespawner : MonoBehaviour
    {
        private GravityApplier _gravityApplier;
        private Rigidbody _rigidbody;
        
        private Vector3    _respawnGravityDirection;
        private Vector3    _respawnPosition;
        private Quaternion _respawnRotation;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _gravityApplier = GetComponent<GravityApplier>();
            _rigidbody      = GetComponent<Rigidbody>();
        }
#endif
        
        // Start is called before the first frame update
        void Start()
        {
            _respawnGravityDirection = _gravityApplier.GravityDirection;
            _respawnPosition         = transform.position;
            _respawnRotation         = transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.y <= -18)
            {
                Respawn();
                return;
            }
            if (transform.position.z >= 18)
            {
                Respawn();
                return;
            }
            if (transform.position.x >= 18)
            {
                Respawn();
            }
        }

        private void Respawn()
        {
            _rigidbody.velocity = Vector3.zero;
            transform.position  = _respawnPosition;
            transform.rotation  = _respawnRotation;
            _gravityApplier.SetGravityDirection(_respawnGravityDirection);
        }
    }
}