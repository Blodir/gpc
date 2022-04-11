using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour {
  public float cameraSensitivity = 10;
  public Transform playerTransform;
  public Transform cameraPivotTransform;
  private float horizontalAngle;
  private float verticalAngle;
  public float minPivot = -40;
  public float maxPivot = 40;
  private float delta;
    
  private void Update() {
    delta = Time.deltaTime;
    MovementManager.Instance.UpdatePlayerMovement(delta);
		RotateCamera(delta);
    CenterToPlayer(delta);
  }
  
  private void RotateCamera(float delta) {
    horizontalAngle += MovementManager.Instance.mouseHorizontal * cameraSensitivity;
    verticalAngle -= MovementManager.Instance.mouseVertical * cameraSensitivity;
    verticalAngle = Mathf.Clamp(verticalAngle, minPivot, maxPivot);

    Quaternion cameraRotation = Quaternion.Euler(0, horizontalAngle, 0);
    transform.rotation = cameraRotation;
    //playerTransform.localRotation = playerTransform.forward * cameraRotation;

    cameraRotation = Quaternion.identity;
    cameraRotation = Quaternion.Euler(verticalAngle, 0, 0);
    cameraPivotTransform.localRotation = cameraRotation;
    
  }

  private void CenterToPlayer(float delta) {
    Vector3 moveToPos = Vector3.Lerp(transform.position, playerTransform.position, delta * cameraSensitivity);
    transform.position = moveToPos;
  }
}
