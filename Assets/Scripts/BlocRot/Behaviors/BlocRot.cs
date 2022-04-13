using System;
using System.Collections;
using Rubworld.BlocRot.Data;
using Rubworld.Gravity.Behaviors;
using UnityEngine;

namespace Rubworld.BlocRot.Behaviors
{
    public class BlocRot : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _rotationAxis = Vector3.zero;
        [SerializeField]
        private Transform _pivot;
        [SerializeField]
        private BlocRotData _blocRotData;
        
        private Transform _savedParent;
        private int _rotationDirection = 1;
        private bool _isRotating;

        // events
        public event Action OnRotationStart = delegate { };

        public void Rotate(GravityApplier toRotate)
        {
            if (_isRotating)
            {
                return;
            }
            StartCoroutine(RotateRoutine(toRotate));
        }

        private IEnumerator RotateRoutine(GravityApplier toRotate)
        {
            _isRotating = true;
            OnRotationStart?.Invoke();
            toRotate.SetGravityDirection(Vector3.zero);
            
            Transform  toRotateTransform        = toRotate.transform;
                       _savedParent             = toRotateTransform.parent;
                       toRotateTransform.parent = _pivot;
            float      currentDuration          = 0;
            Quaternion startRotation            = _pivot.rotation;
            Quaternion targetRotation           = _pivot.rotation * Quaternion.Euler(_rotationAxis * (90f * _rotationDirection));
            
            while (currentDuration < _blocRotData.RotationDuration)
            {
                _pivot.rotation = Quaternion.Lerp(startRotation, targetRotation, currentDuration/_blocRotData.RotationDuration);
                yield return null;
                currentDuration += Time.deltaTime;
            }

            _pivot.rotation = targetRotation;
            toRotate.transform.parent = _savedParent;

            toRotate.AutoSetGravityFromRotation();
            _isRotating = false;
        }

        public void AxisTrigger(Vector3 axis)
        {
            if (axis == _rotationAxis)
            {
                return;
            }

            if (_rotationAxis.x != 0)
            {
                if (axis.y != 0)
                {
                    _rotationDirection = -1;
                }

                if (axis.z != 0)
                {
                    _rotationDirection = 1;
                }
            }

            if (_rotationAxis.y != 0)
            {
                if (axis.x != 0)
                {
                    _rotationDirection = -1;
                }

                if (axis.z != 0)
                {
                    _rotationDirection = 1;
                }
            }

            if (_rotationAxis.z != 0)
            {
                if (axis.x != 0)
                {
                    _rotationDirection = -1;
                }

                if (axis.y != 0)
                {
                    _rotationDirection = 1;
                }
            }
            
        }
    }
}