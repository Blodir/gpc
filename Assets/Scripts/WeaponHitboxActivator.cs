using UnityEngine;
using Unity.Netcode;

public class WeaponHitboxActivator : MonoBehaviour
{
  [SerializeField]
  [Tooltip("Set collider to be enabled and disabled by the attack animation")]
  private CapsuleCollider col;

  void Start()
  {
    col.enabled = false;
  }

  public void EnableHitbox()
  {
    if (NetworkManager.Singleton.IsServer)
    {
      col.enabled = true;
    }
  }

  public void DisableHitbox()
  {
    col.enabled = false;
  }
}