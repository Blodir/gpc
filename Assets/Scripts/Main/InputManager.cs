using Unity.Netcode;
using UnityEngine;

public class InputManager: MonoBehaviour
{
	private static InputManager _instance;
  private PlayerInput controls;
  public CameraFocus cameraFocus;

  public static InputManager Instance
  {
		get {
			if (_instance == null) {
				Debug.LogError("Input manager is null");
			}
			return _instance;
		}
	}
  private void Awake()
  {
		_instance = this;
    controls = new PlayerInput();
  }

  private void OnEnable()
  {
    controls.Enable();
  }

  private void Update()
  {
    if (NetworkManager.Singleton.IsClient) {
      NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerCharacter>()
        .Move(
          controls.movement.move.ReadValue<Vector2>()
        );
      cameraFocus.RotateCamera(controls.movement.look.ReadValue<Vector2>());
    }
  }
}
