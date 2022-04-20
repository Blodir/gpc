using Unity.Netcode;
using UnityEngine;
using System.Collections;

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

  [SerializeField]
  private float despawnAfterSeconds = 5f;

  private bool isDead = false;
  private Health hlt;

  public override void OnNetworkSpawn()
  {
    hlt = GetComponent<Health>();
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

  public void Move(Vector2 direction)
  {
    if (IsOwner)
    {
      PlayerMovementInputServerRpc(direction);
    }
  }

  public void Attack()
  {
    PlayerAttackServerRpc();
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

    if (isDead)
      return;
    if (hlt.Current <= 0f)
    {
      StartCoroutine(Die());
    }

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
    transform.rotation = Quaternion.RotateTowards(
      transform.rotation, Quaternion.LookRotation(newDirection), rotationDegreesPerSec * Time.deltaTime
    );

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

  private IEnumerator Die()
  {
    isDead = true;
    animator.SetTrigger("Death");
    yield return new WaitForSeconds(despawnAfterSeconds);
    Destroy(gameObject);
  }
}
