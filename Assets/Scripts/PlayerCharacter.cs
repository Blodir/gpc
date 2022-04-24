using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
  public NetworkVariable<Vector3> position = new NetworkVariable<Vector3>();
  private NetworkVariable<Vector2> movementInput = new NetworkVariable<Vector2>();

  [SerializeField]
  private float movementUnitsPerSec = 3f;
  [SerializeField]
  private float rotationDegreesPerSec = 360f;

  [SerializeField]
  private float attackDamage = 25f;

  [SerializeField]
  private Animator animator;
  [SerializeField]
  private float attackAnimationSpeed = 1f;
  [SerializeField]
  private float movementAnimationDampTime = 0.1f;
  private bool strafe;
  private Transform cameraTransform;
  private Stamina stamina;
  private float regenerationTimer;
  public bool playerInAction = false;

  private void Awake()
  {
    cameraTransform = Camera.main.transform;
    stamina = this.gameObject.GetComponent<Stamina>();
  }

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
  public void PlayerMovementInputServerRpc(Vector2 movement)
  {
    // input is normalized on server so that players can't cheat by sending large inputs
    movementInput.Value = movement.normalized;
  }

  [ServerRpc]
  private void PlayerAttackServerRpc()
  {
    BeginAttackAnimation();
    PlayerAttackClientRpc();
  }

  /*private void PlayerStrafeServerRpc()
  {
    BeginStrafeAnimation();
  }*/

  [ClientRpc]
  private void PlayerAttackClientRpc()
  {
    BeginAttackAnimation();
  }

  private void BeginAttackAnimation()
  {
    animator.SetFloat("AttackSpeed", attackAnimationSpeed);
    animator.SetTrigger("Attack");
  }

  /*privateStrafeAnimation()
  {
    animator.
  }*/

  public void Move(Vector2 direction)
  {
    if (IsOwner)
    {
      PlayerMovementInputServerRpc(direction);
    }
  }

  public void Attack()
  {
    if (stamina.GetStamina() > 0)
    {
      stamina.TakeFatigue(20f);
      PlayerAttackServerRpc();
    }
  }

  public void Strafe(bool status)
  {
    //PlayerStrafeServerRpc();
    strafe = status;
  }

  public void OnTriggerEnter(Collider col)
  {
    if (NetworkManager.Singleton.IsServer)
    {
      Health health = col.gameObject.GetComponent<Health>();
      if (health != null)
      {
        health.TakeDamage(attackDamage);
      }
    }
  }

  private void Update()
  {
    // update authoritative position
    if (NetworkManager.Singleton.IsServer)
    {
      Vector2 mvt = movementInput.Value.normalized * movementUnitsPerSec * Time.deltaTime;
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
    
    stamina.RegenerateStamina(playerInAction);

    if (!strafe)
    {
      transform.rotation = Quaternion.RotateTowards(
        transform.rotation, Quaternion.LookRotation(newDirection), 
        rotationDegreesPerSec * Time.deltaTime);
    }
    else
    {
      transform.rotation = Quaternion.Euler(0, cameraTransform.transform.eulerAngles.y, 0);
    }

    // animate
    if (movementInput.Value.magnitude > 0)
    {
      animator.SetFloat("Speed", 1f, movementAnimationDampTime, Time.deltaTime);
    }
    else
    {
      animator.SetFloat("Speed", 0f, movementAnimationDampTime, Time.deltaTime);
    }
  }
}
