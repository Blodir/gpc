using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
  //private MovementManager movementManager;
  public float moveSpeed = 10;
  public float turnSpeed = 10;
  [SerializeField]
  private Animator animator;
  [SerializeField]
  private float animationDampTime = 0.1f;
  private Vector3 move;
  private Vector3 direction;
  private float delta;

  private void Start() {
    //movementManager = GetComponent<MovementManager>();
  }

  void Update() {
    delta = Time.deltaTime;
    MovementManager.Instance.UpdatePlayerMovement(delta);
    //movementManager.UpdatePlayerMovement(delta);
    MovePlayer();
    TurnPlayer();
  }

  private void MovePlayer() {
    //Vector3 move = new Vector3(movementManager.moveX, 0, movementManager.moveZ);
    move = new Vector3(MovementManager.Instance.moveX, 0, MovementManager.Instance.moveZ);
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

  private void TurnPlayer() {
    //Vector3 direction = new Vector3(movementManager.moveX, 0, movementManager.moveZ);
    Vector3 newDirection = new Vector3(MovementManager.Instance.moveX, 0, MovementManager.Instance.moveZ);
    direction += newDirection;
    Quaternion rotateToDirection = Quaternion.LookRotation(direction);
    Quaternion rotate = Quaternion.Lerp(transform.rotation, rotateToDirection, turnSpeed);
    transform.rotation = rotate;
  }
}
