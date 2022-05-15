using Unity.Netcode;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	private static UIManager _instance;

  public string joinCode;
  public string debugString;

  public static UIManager Instance {
		get {
			if (_instance == null) {
				Debug.LogError("Input manager is null");
			}
			return _instance;
		}
	}

  private void Awake() {
		_instance = this;
    Cursor.visible = false;
  }

  void OnGUI()
  {
    GUILayout.BeginArea(new Rect(10, 10, 300, 300));
    StatusLabels();

    GUILayout.EndArea();
  }

  private void StatusLabels()
  {
    var mode = NetworkManager.Singleton.IsHost ?
      "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

    if (joinCode.Length > 0)
    {
      GUILayout.Label("JoinCode: " + joinCode);
    }

    if (debugString.Length > 0)
    {
      GUILayout.Label("debugString: " + debugString);
    }

    GUILayout.Label("Transport: " +
      NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
    GUILayout.Label("Mode: " + mode);
  }
}