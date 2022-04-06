using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementManager : MonoBehaviour {
	private static MovementManager _instance;
	private Movement movement;
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
    movement = new Movement();
  }

	private void OnEnable() {
		movement.Enable();
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
		movementInput = movement.Player.move.ReadValue<Vector2>().normalized * delta;
		moveX = movementInput.x;
		moveZ = movementInput.y;

		rotationInput = movement.Player.look.ReadValue<Vector2>().normalized * delta;
		mouseHorizontal = rotationInput.x;
		mouseVertical = rotationInput.y;
	}

}
