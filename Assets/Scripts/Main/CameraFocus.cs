using Unity.Netcode;
using UnityEngine;

public class CameraFocus : MonoBehaviour {
  [SerializeField]
  public float translationSensitivity = 10;
  [SerializeField]
  public float rotationSensitivity = .2f;

  public Transform cameraPivotTransform;

  public void RotateCamera(Vector2 v)
  {
    Vector2 rotDelta = v * rotationSensitivity;
    cameraPivotTransform.localRotation = Quaternion.Euler(
      cameraPivotTransform.localRotation.eulerAngles + new Vector3(rotDelta.y, rotDelta.x, 0f)
    );
  }

  private void Update()
  {
    if (NetworkManager.Singleton.IsClient)
    {
      transform.position =
        Vector3.Lerp(
          transform.position,
          NetworkManager.Singleton.LocalClient.PlayerObject.transform.position,
            Time.deltaTime * translationSensitivity
        );
    }
  }
}
