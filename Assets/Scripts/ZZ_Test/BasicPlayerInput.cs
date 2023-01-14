using System;
using DependencyInjection;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicPlayerInput : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private Camera _camera;
    
    [SerializeField, ReadOnly] private Rigidbody _rb;

    private Vector3 _forceDirection;

    private void OnValidate()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MoveLogicMethod();
    }

    private void MoveLogicMethod()
    {
        //_forceDirection += 100f * Time.fixedDeltaTime;
    }

    private Vector3 GetCameraForward()
    {
        Vector3 forward = _camera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight()
    {
        Vector3 forward = _camera.transform.right;
        forward.y = 0;
        return forward.normalized;
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.25f, Vector3.down);
        return Physics.Raycast(ray, out RaycastHit hit, 0.3f);
    }
}
