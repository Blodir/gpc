using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
  public NetworkVariable<Vector3> position = new NetworkVariable<Vector3>();
  public NetworkVariable<Vector3> rotation = new NetworkVariable<Vector3>();
  private NetworkVariable<Vector2> movementInput = new NetworkVariable<Vector2>();

  [SerializeField]
  private float movementSpeed = 1f;
  [SerializeField]
  private float rotationDegreesPerSec = 360f;

  [SerializeField]
  private Animator animator;
  [SerializeField]
  private float animationDampTime = 0.1f;

  public override void OnNetworkSpawn()
  {
    if (NetworkManager.Singleton.IsServer)
    {
      Vector3 randomPosition = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
      transform.position = randomPosition;
      position.Value = randomPosition;
    }
  }

  [ServerRpc]
  public void PlayerInputServerRpc(Vector2 movement)
  {
    // input is normalized on server so that players can't cheat by sending large inputs
    movementInput.Value = movement.normalized;
  }

  public void Move(Vector2 movement)
  {
    if (IsOwner)
    {
      PlayerInputServerRpc(movement);
    }
  }

  private void Update()
  {
    // update authoritative position
    if (NetworkManager.Singleton.IsServer)
    {
      Vector2 mvt = movementInput.Value.normalized * movementSpeed * Time.deltaTime;
      position.Value += new Vector3(mvt.x, 0, mvt.y);
    }

    // update local position
    transform.position = position.Value;

    // update local rotation
    Vector3 newDirection = new Vector3(movementInput.Value.x, 0f, movementInput.Value.y);
    if (newDirection == Vector3.zero)
    {
      newDirection = transform.forward;
    }
    transform.rotation = Quaternion.RotateTowards(
      transform.rotation, Quaternion.LookRotation(newDirection), rotationDegreesPerSec * Time.deltaTime
    );

    // animate
    if (movementInput.Value.magnitude > 0)
    {
      animator.SetFloat("Speed", 1f, animationDampTime, Time.deltaTime);
    }
    else
    {
      animator.SetFloat("Speed", 0f, animationDampTime, Time.deltaTime);
    }
  }
}
