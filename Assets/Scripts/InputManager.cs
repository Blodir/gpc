using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager: MonoBehaviour, PlayerInputActions.ICombatActions, PlayerInputActions.IMovementActions
{
	private static InputManager _instance;
  private PlayerInputActions controls;
  public CameraFocus cameraFocus;
  private bool inAction;

  public static InputManager Instance
  {
		get {
			if (_instance == null)
      {
				Debug.LogError("Input manager is null");
			}
			return _instance;
		}
	}

  public void OnBecomeClient()
  {
    controls = new PlayerInputActions();
    controls.movement.SetCallbacks(this);
    controls.combat.SetCallbacks(this);
    controls.Enable();
  }

  private void Awake()
  {
		_instance = this;
  }

  public void OnAttack(InputAction.CallbackContext context)
  {
    if (context.phase == InputActionPhase.Started)
    {
      NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerCharacter>()
        .Attack();
    }
  }

  public void OnStrafe(InputAction.CallbackContext context)
  {
    if (context.phase == InputActionPhase.Started)
    {
      NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerCharacter>()
        .Strafe(true);
    }
    else if (context.phase == InputActionPhase.Canceled)
    {
      NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerCharacter>()
        .Strafe(false);
    }
  }

  public void OnMove(InputAction.CallbackContext context) { OnMove(); }
  public void OnMove()
  {
    // movement direction should depend on the rotation of the camera
    // eg. pressing 'a' should result in character moving 'left' on the screen (relative to the camera)
    Vector2 moveInput = controls.movement.move.ReadValue<Vector2>();
    // should just do the math on vector2 here, but unity is crap and cba to implement it myself
    Vector3 rotatedMoveInput = cameraFocus.cameraPivotTransform.localRotation * new Vector3(moveInput.x, 0f, moveInput.y);
    NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerCharacter>()
      .Move(
        new Vector2(rotatedMoveInput.x, rotatedMoveInput.z)
      );
  }

  public void OnLook(InputAction.CallbackContext context)
  {
    Vector2 rotationInput = controls.movement.look.ReadValue<Vector2>();
    cameraFocus.RotateCamera(new Vector2(-rotationInput.y, rotationInput.x));
    OnMove(); // move direction needs to be adjusted when camera rotation changes
  }
}
