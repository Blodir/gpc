using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementManager : MonoBehaviour {
	private static MovementManager _instance;
	private PlayerInput controls;
	//public float cameraSensitivity = 1;
	private Vector2 movementInput;
	private Vector2 rotationInput;
	public float moveX;
	public float moveZ;
	public float mouseHorizontal;
	public float mouseVertical;
	private float delta;
  
	public static MovementManager Instance {
		get {
			if (_instance == null) {
				Debug.LogError("Movement manager is null");
			}

			return _instance;
		}
	}
  private void Awake() {
		_instance = this;
    controls = new PlayerInput();
  }

	private void OnEnable() {
		controls.Enable();
	}

  private void Update() {
		delta = Time.deltaTime;
		UpdatePlayerMovement(delta);
		//MovePlayer();

		//Quaternion deltaRotation = Quaternion.Euler(-rotationInput.y * cameraSensitivity, rotationInput.x * cameraSensitivity, 0f);
		//this.transform.localRotation *= deltaRotation;
		//this.transform.position += deltaPosition * movementSpeed;
		//this.transform.rotation *= deltaRotation;
  }

	public void UpdatePlayerMovement(float delta) {
		movementInput = controls.movement.move.ReadValue<Vector2>().normalized * delta;
		moveX = movementInput.x;
		moveZ = movementInput.y;

		rotationInput = controls.movement.look.ReadValue<Vector2>().normalized * delta;
		mouseHorizontal = rotationInput.x;
		mouseVertical = rotationInput.y;
	}

}
