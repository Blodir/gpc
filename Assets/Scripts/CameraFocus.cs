using Unity.Netcode;
using UnityEngine;

public class CameraFocus : MonoBehaviour {
  [SerializeField]
  public float translationSensitivity = 10f;
  [SerializeField]
  public float rotationSensitivity = .2f;
  [SerializeField]
  public float minVerticalCameraAngle = -10f;
  [SerializeField]
  public float maxVerticalCameraAngle = 60f;

  public Transform cameraPivotTransform;

  public void RotateCamera(Vector2 v)
  {
    Vector2 rotDelta = v * rotationSensitivity;
    Vector3 angles = cameraPivotTransform.localRotation.eulerAngles + new Vector3(rotDelta.x, rotDelta.y, 0f);
    if (angles.x > 180)
    {
      angles.x = Mathf.Max(angles.x, 360 + minVerticalCameraAngle);
    } else
    {
      angles.x = Mathf.Min(angles.x, maxVerticalCameraAngle);
    }
    cameraPivotTransform.localRotation = Quaternion.Euler(angles);
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
