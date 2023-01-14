using Rubworld.Gravity.Behaviors;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterGrabberController : MonoBehaviour
{
    private ThirdPersonActionAsset _playerActionsAsset;

    private InteractableCube _interactableCube;
    private InteractableCube _grabbedInteractableCube;

    [SerializeField] private LayerMask _gpeLayerMask;

    [Header("Autofill")]
    [SerializeField, DependencyInjection.ReadOnly]
    private GravityApplier _gravityApplier;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _gravityApplier = GetComponent<GravityApplier>();
    }
#endif
    
    private void Awake()
    {
        _playerActionsAsset = new ThirdPersonActionAsset();
    }

    private void OnEnable()
    {
        _playerActionsAsset.Player.Grab.started += DoGrab;
        _playerActionsAsset.Player.Enable();
    }

    private void OnDisable()
    {
        _playerActionsAsset.Player.Grab.started -= DoGrab;
        _playerActionsAsset.Player.Disable();
    }

    private void DoGrab(InputAction.CallbackContext callbackContext)
    {
        if (_gravityApplier.ShouldApplyGravity)
        {
            return;
        }

        Debug.Log($"[FIX] Grab.callbackContext\n" +
                  $"{{\n" +
                  $"  action : {callbackContext.action}\n" +
                  $"  canceled : {callbackContext.canceled}\n" +
                  $"  control : {callbackContext.control}\n" +
                  $"  duration : {callbackContext.duration}\n" +
                  $"  interaction : {callbackContext.interaction}\n" +
                  $"  performed : {callbackContext.performed}\n" +
                  $"  phase : {callbackContext.phase}\n" +
                  $"  started : {callbackContext.started}\n" +
                  $"  time : {callbackContext.time}\n" +
                  $"  startTime : {callbackContext.startTime}\n" +
                  $"  valueType : {callbackContext.valueType}\n" +
                  $"  valueSizeInBytes : {callbackContext.valueSizeInBytes}\n" +
                  $"}}");

        if (_grabbedInteractableCube)
        {
            // todo: create several methods from a CharacterGrabber and InteractableCube
            // release current grabbed cube
            foreach (var collider in _grabbedInteractableCube.GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }
            _grabbedInteractableCube.GetComponent<GravityApplier>().enabled = true;
            _grabbedInteractableCube.GetComponent<Rigidbody>().isKinematic = false;
            _grabbedInteractableCube.transform.parent = null;
            _grabbedInteractableCube.transform.position = transform.position + transform.up * 0.5f + transform.forward * 0.7f;
            _grabbedInteractableCube = null;
            return;
        }
        
        if (_interactableCube == null)
        {
            // nothing to grab
            return;
        }

        // todo: create several methods from a CharacterGrabber and InteractableCube
        _grabbedInteractableCube = _interactableCube;
        _grabbedInteractableCube.transform.parent = transform;
        _grabbedInteractableCube.GetComponent<GravityApplier>().enabled = false;
        _grabbedInteractableCube.GetComponent<Rigidbody>().isKinematic = true;
        foreach (var collider in _grabbedInteractableCube.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        _grabbedInteractableCube.transform.position = transform.position + transform.up * 0.5f + transform.forward * 0.5f;
        _grabbedInteractableCube.transform.rotation = transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateActiveGPE();
    }

    private void UpdateActiveGPE()
    {
        RaycastHit hit;

        Vector3 origin = transform.position + transform.up * 0.5f;

        if (Physics.SphereCast(origin, 0.3f, transform.forward, out hit, .8f, _gpeLayerMask))
        {
            Debug.DrawRay(hit.transform.position, Vector3.up * 5f, Color.green);
            _interactableCube = hit.transform.GetComponent<InteractableCube>();
        }
        else
        {
            _interactableCube = null;
        }
    }
}
