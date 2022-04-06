using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour {
  public float cameraSensitivity = 10;
  //private MovementManager movementManager;
  public Transform playerTransform;
  public Transform cameraPivotTransform;
  private float delta;
    
  private void Awake() {
    //movementManager = GetComponent<MovementManager>();
  }

  private void Update() {
    delta = Time.deltaTime;
		//movementManager.UpdatePlayerMovement(delta);
    MovementManager.Instance.UpdatePlayerMovement(delta);
		RotateCamera(delta);
    CenterToPlayer(delta);
    
    //Vector2 rotationInput = movement.Player.look.ReadValue<Vector2>() * Time.deltaTime;
		//Quaternion deltaRotation = Quaternion.Euler(-rotationInput.y * cameraSensitivity, rotationInput.x * cameraSensitivity, 0f);
		//this.transform.localRotation *= deltaRotation;
  }
  float horizontalAngle;
  float verticalAngle;
  //private void RotateCamera() {
  private void RotateCamera(float delta) {
    horizontalAngle += (MovementManager.Instance.mouseHorizontal * cameraSensitivity);
    verticalAngle += MovementManager.Instance.mouseVertical * cameraSensitivity;

    Quaternion cameraRotation = Quaternion.Euler(0, horizontalAngle, 0);
    transform.rotation = cameraRotation;
    cameraRotation = Quaternion.identity;
    cameraRotation = Quaternion.Euler(verticalAngle, 0, 0);
    cameraPivotTransform.localRotation = cameraRotation;
    
    /*Vector3 cameraRotation = Vector3.zero;
    cameraRotation.y = horizontalAngle;
    Quaternion targetRotation = Quaternion.Euler(cameraRotation);
    transform.rotation = targetRotation;
    cameraRotation = Vector3.zero;
    cameraRotation.x = verticalAngle;
    targetRotation = Quaternion.Euler(cameraRotation);
    cameraPivotTransform.localRotation = targetRotation;//*/
  }

  private void CenterToPlayer(float delta) {
    Vector3 moveToPos = Vector3.Lerp(transform.position, playerTransform.position, delta * cameraSensitivity);
    transform.position = moveToPos;

  }
}
