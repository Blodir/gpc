using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
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
    col.enabled = true;
  }

  public void DisableHitbox()
  {
    col.enabled = false;
  }
}
