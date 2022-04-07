using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour, PlayerInput.ICombatActions
{
  [SerializeField]
  private Animator animator;
  [SerializeField]
  private float attackSpeed = 1f;
  private PlayerInput controls;

  public void OnEnable()
  {
    if (controls == null)
    {
      controls = new PlayerInput();
      controls.combat.SetCallbacks(this);
    }
    controls.combat.Enable();
  }

  void Update()
  {
    
  }

  public void OnAttack(InputAction.CallbackContext context)
  {
    if (context.phase == InputActionPhase.Started)
    {
      animator.SetFloat("AttackSpeed", attackSpeed);
      animator.SetTrigger("Attack");
    }
  }

}
