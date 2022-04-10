using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
  public float moveSpeed = 1;
  public float turnSpeed = 1;
  [SerializeField]
  private Animator animator;
  [SerializeField]
  private float animationDampTime = 0.1f;
  private Vector3 move;
  private float delta;

  Transform cameraObject;

  private void Start()
  {
    cameraObject = Camera.main.transform;
  }

  private void Update()
  {
    delta = Time.deltaTime;
    MovementManager.Instance.UpdatePlayerMovement(delta);
    MovePlayer();
    TurnPlayer();
  }

  private void MovePlayer()
  {
    move = new Vector3(MovementManager.Instance.moveX, 0, MovementManager.Instance.moveZ);
    move.Normalize();
    transform.position += move * moveSpeed;
    
    if (move.magnitude > 0)
    {
      animator.SetFloat("Speed", 1f, animationDampTime, Time.deltaTime);
    }
    else
    {
      animator.SetFloat("Speed", 0f, animationDampTime, Time.deltaTime);
    }
  }

  private void TurnPlayer()
  {
    Vector3 newDirection = new Vector3(MovementManager.Instance.moveX, 0, MovementManager.Instance.moveZ);
    newDirection.Normalize();

    if (newDirection == Vector3.zero)
    {
      newDirection = transform.forward;
    }

    Quaternion rotateToDirection = Quaternion.LookRotation(newDirection);
    rotateToDirection = Quaternion.Slerp(transform.rotation, rotateToDirection, turnSpeed);
    transform.rotation = rotateToDirection;
  }
}
