using System;
using Rubworld.SwitchBloc.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Rubworld.SwitchBloc.Behaviors
{
    public class SwitchBloc : MonoBehaviour
    {
        [SerializeField] private SwitchBlocData _switchBlocData;
        [SerializeField] private UnityEvent _onTrigger;
        [SerializeField] private UnityEvent _onUntrigger;

        private int _triggerCount;
        private bool _triggered;
        private Vector3 _upPosition;
        [SerializeField] private MeshRenderer _switchMeshRenderer;

        public bool Triggered => _triggered;

        private void FixedUpdate()
        {
            if (IsPressed())
            {
                SetTriggered(true);
            }
            else
            {
                SetTriggered(false);
            }
        }

        private void SetTriggered(bool triggered)
        {
            if (_triggered == triggered)
            {
                return;
            }
            _triggered = triggered;
            if (_triggered)
            {
                OnTrigger();
            }
            else
            {
                OnUntrigger();
            }
        }

        private void OnTrigger()
        {
            _switchMeshRenderer.material = _switchBlocData.TriggeredMaterial;
            _onTrigger?.Invoke();
        }

        private void OnUntrigger()
        {
            _switchMeshRenderer.material = _switchBlocData.NormalMaterial;
            _onUntrigger?.Invoke();
        }

        private bool IsPressed()
        {
            return _triggerCount > 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Map"))
            {
                return;
            }
            _triggerCount++;
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Map"))
            {
                return;
            }
            _triggerCount--;
        }
    }
}
