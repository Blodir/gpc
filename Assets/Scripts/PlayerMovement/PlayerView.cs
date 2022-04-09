using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour {
  public float cameraSensitivity = 10;
  public Transform playerTransform;
  public Transform cameraPivotTransform;
  private float horizontalAngle;
  private float verticalAngle;
  private float delta;
    
  private void Update() {
    delta = Time.deltaTime;
    MovementManager.Instance.UpdatePlayerMovement(delta);
		RotateCamera(delta);
    CenterToPlayer(delta);
  }
  
  private void RotateCamera(float delta) {
    horizontalAngle += (MovementManager.Instance.mouseHorizontal * cameraSensitivity);
    verticalAngle += MovementManager.Instance.mouseVertical * cameraSensitivity;

    Quaternion cameraRotation = Quaternion.Euler(0, horizontalAngle, 0);
    transform.rotation = cameraRotation;
    cameraRotation = Quaternion.identity;
    cameraRotation = Quaternion.Euler(verticalAngle, 0, 0);
    cameraPivotTransform.localRotation = cameraRotation;
  }

  private void CenterToPlayer(float delta) {
    Vector3 moveToPos = Vector3.Lerp(transform.position, playerTransform.position, delta * cameraSensitivity);
    transform.position = moveToPos;
  }
}
