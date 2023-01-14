using Rubworld.Gravity.Behaviors;
using UnityEngine;

public class ThirdPersonAnimation : MonoBehaviour
{
    [SerializeField] private GravityApplier _gravityApplier;
    
    private Animator _animator;
    private Rigidbody _rigidbody;
    private float _maxSpeed = 12.2f;
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int Speed = Animator.StringToHash("speed");

    private void OnValidate()
    {
        _gravityApplier = GetComponent<GravityApplier>();
    }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gravityApplier.ShouldApplyGravity)
        {
            _animator.SetBool(IsFalling, true);
        }
        else
        {
            _animator.SetBool(IsFalling, false);
            _animator.SetFloat(Speed, _rigidbody.velocity.magnitude / _maxSpeed);
        }
    }
}
