using DG.Tweening;
using EasyCharacterMovement;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShamanCharacter : Character
{
    [SerializeField] private SheepDetector _sheepDetector;
    [SerializeField] private Transform _carryPositionTransform;
    
    [SerializeField] private GameObject _carrySheepDummyPrefab;

    private SheepCharacter _carriedSheep;
    private GameObject _carriedSheepDummy;

    private bool _isPickingUpSheep;
    
    #region LIFECYCLE

    protected override void OnEnable()
    {
        base.OnEnable();
        _sheepDetector.NoSheep += OnNoSheep;
        _sheepDetector.SheepDetected += OnSheepDetected;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _sheepDetector.NoSheep -= OnNoSheep;
        _sheepDetector.SheepDetected -= OnSheepDetected;
    }

    #endregion

    protected override void OnCollided(ref CollisionResult collisionResult)
    {
        base.OnCollided(ref collisionResult);
        
        // Debug.Log("Source: " + collisionResult.collider.gameObject.name);
        // Debug.Log("Collided: " + collisionResult.hitResult.collider.gameObject.name + " at " + collisionResult.hitLocation);

        // TODO check for front
        if (collisionResult.hitLocation == HitLocation.Sides)
        {
            SheepCharacter sheepCharacter = collisionResult.hitResult.collider.gameObject.GetComponent<SheepCharacter>();
            if (sheepCharacter != null)
            {
                // Debug.Log("Hit sheep character");
            }
        }
    }

    #region METHODS

    public void PickUpSheep()
    {
        _carriedSheep = _sheepDetector.Sheep;
        _carriedSheep.gameObject.SetActive(false);

        _carriedSheepDummy = Instantiate(_carrySheepDummyPrefab, transform);
        _carriedSheepDummy.transform.position = _carriedSheep.transform.position;
        _carriedSheepDummy.transform.rotation = _carriedSheep.transform.rotation;

        _isPickingUpSheep = true;
        _carriedSheepDummy.transform.DOMove(_carryPositionTransform.position, 1f);
        _carriedSheepDummy.transform.DORotate(_carryPositionTransform.rotation.eulerAngles, 1f).OnComplete(() =>
        {
            _isPickingUpSheep = false;
        });
    }

    #endregion
    
    #region INPUT ACTIONS

    private InputAction interactInputAction { get; set; }
    
    protected override void InitPlayerInput()
    {
        // Setup base input actions (eg: Movement, Jump, Sprint, Crouch)

        base.InitPlayerInput();

        // Setup Interaction input action handlers

        interactInputAction = inputActions.FindAction("Interact");
        if (interactInputAction != null)
        {
            interactInputAction.started += OnInteract;
            interactInputAction.canceled += OnInteract;

            interactInputAction.Enable();
        }
    }

    protected override void DeinitPlayerInput()
    {
        base.DeinitPlayerInput();

        if (interactInputAction != null)
        {
            interactInputAction.started -= OnInteract;
            interactInputAction.canceled -= OnInteract;

            interactInputAction.Disable();
            interactInputAction = null;
        }
    }

    #endregion

    #region INPUT ACTION HANDLERS

    private void OnInteract(InputAction.CallbackContext context)
    {
        // if (context.started && !IsClimbing())
        //     Climb();
        // else if (context.started && IsClimbing() && _climbingState == ClimbingState.Grabbed)
        //     StopClimbing();

        if (context.started && _carriedSheep == null && _sheepDetector.Sheep != null)
        {
            PickUpSheep();
        }
    }
    
    #endregion

    #region EVENT HANDLERS

    private void OnSheepDetected(SheepCharacter sheep)
    {
        Debug.Log("Sheep detected: " + sheep.gameObject.name);
    }

    private void OnNoSheep()
    {
        Debug.Log("No sheep");
    }

    #endregion

}
