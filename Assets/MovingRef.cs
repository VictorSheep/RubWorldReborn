using System;
using Rubworld.Gravity.Behaviors;
using UnityEngine;

public class MovingRef : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private GravityApplier _gravityApplier;
    [SerializeField] private LayerMask _onlyRaycastLayerMask;
    [SerializeField] private GameObject _refColliderX;
    [SerializeField] private GameObject _refColliderY;
    [SerializeField] private GameObject _refColliderZ;
    
    private Vector3 _previousGravity;
    private Vector3 _vectorUp = Vector3.zero;
    private Vector3 _vectorRight = Vector3.zero;
    private Vector3 _cameraOffsetPosition;

    public Vector3 VectorUp => _vectorUp;
    public Vector3 VectorRight => _vectorRight;

    private void Start()
    {
        _cameraOffsetPosition = transform.position - _playerCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePositionFromCamera();
        UpdateMovingColliderRefFromGravity();
        UpdateMovingVector();
    }

    private void UpdatePositionFromCamera()
    {
        transform.position = _playerCamera.transform.position + _cameraOffsetPosition;
    }

    private void UpdateMovingVector()
    {
        RaycastHit hit;

        var cameraTransform = _playerCamera.transform;
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 20f, Color.yellow);
        Debug.DrawRay(cameraTransform.position+ transform.up, cameraTransform.forward * 20f, Color.yellow);
        Debug.DrawRay(cameraTransform.position+ transform.right, cameraTransform.forward * 20f, Color.yellow);
        
        Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 20f, _onlyRaycastLayerMask);
        Vector3 pivotPoint = hit.point;
        Physics.Raycast(cameraTransform.position + cameraTransform.up, cameraTransform.forward, out hit, 20f, _onlyRaycastLayerMask);
        Vector3 upPoint = hit.point;
        Physics.Raycast(cameraTransform.position + cameraTransform.right, cameraTransform.forward, out hit, 20f, _onlyRaycastLayerMask);
        Vector3 rightPoint = hit.point;

        _vectorUp = (upPoint - pivotPoint).normalized;
        _vectorRight = (rightPoint - pivotPoint).normalized;
    }

    private void UpdateMovingColliderRefFromGravity()
    {
        if (_previousGravity == _gravityApplier.GravityDirection)
        {
            return;
        }
        
        _refColliderX.SetActive(_gravityApplier.GravityDirection.x != 0);
        _refColliderY.SetActive(_gravityApplier.GravityDirection.y != 0);
        _refColliderZ.SetActive(_gravityApplier.GravityDirection.z != 0);
        
        _previousGravity = _gravityApplier.GravityDirection;
    }
    
}
