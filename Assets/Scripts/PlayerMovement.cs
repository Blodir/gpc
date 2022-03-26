using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
  public float movementSpeed = 0.0001f;
  public float cameraSensitivity = 0.001f;
  private Movement movement;
    
  private void Awake() {
    movement = new Movement();
  }

  private void OnEnable() {
		movement.Enable();
  }

  private void Update() {
    Vector2 movementInput = movement.Player.move.ReadValue<Vector2>().normalized / Time.deltaTime;
		Vector2 rotationInput = movement.Player.look.ReadValue<Vector2>() / Time.deltaTime;

		Vector3 deltaPosition = new Vector3(movementInput.x, 0f, movementInput.y);
		Quaternion deltaRotation = Quaternion.Euler(0f, rotationInput.x * cameraSensitivity, 0f);
		//Quaternion deltaRotation = Quaternion.Euler(rotationInput.y * 0.001f, 0f, 0f);

		this.transform.position += deltaPosition * movementSpeed;
		this.transform.rotation *= deltaRotation;
  }
}
