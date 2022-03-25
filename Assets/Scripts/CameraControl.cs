using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
  public Transform cameraTarget;
	public float pLerp = 0.001f;
	public float rLerp = 0.001f;

  void Update() {
    transform.position = Vector3.Lerp(transform.position, cameraTarget.position, pLerp / Time.deltaTime);
		transform.rotation = Quaternion.Lerp(transform.rotation, cameraTarget.rotation, rLerp / Time.deltaTime);
  }
}
