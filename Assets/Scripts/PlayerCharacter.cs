using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerCharacter : NetworkBehaviour
{
  private Vector3 _serverPosition;
  private Vector3 serverPosition
  {
    get => _serverPosition;
    set
    {
      _serverPosition = value;
      if (NetworkManager.Singleton.IsServer)
      {
        PositionClientRpc(value);
      }
    }
  }
  private PositionRecord[] clientPositions = {null, null};
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

  [SerializeField]
  private float respawnCooldown = 5f;
  private NetworkVariable<bool> isDead = new NetworkVariable<bool>(false);
  private bool respawnEnabled = false;
  private Health hlt;

  private void Awake()
  {
    cameraTransform = Camera.main.transform;
    stamina = this.gameObject.GetComponent<Stamina>();
  }

  public override void OnNetworkSpawn()
  {
    hlt = GetComponent<Health>();
    if (NetworkManager.Singleton.IsServer)
    {
      Vector3 randomPosition = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
      transform.position = randomPosition;
      serverPosition = randomPosition;
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
  private void PositionClientRpc(Vector3 pos)
  {
    clientPositions[1] = clientPositions[0];
    clientPositions[0] = new PositionRecord(System.DateTime.Now.ToFileTime(), pos);
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
    if (!isDead.Value)
    {
      //stamina.TakeFatigue(20f);
      PlayerAttackServerRpc();
    }
  }

  [ServerRpc]
  public void RespawnServerRpc()
  {
    hlt.ResetHP();
    isDead.Value = false;
    RespawnClientRpc();
  }

  [ClientRpc]
  public void RespawnClientRpc()
  {
    respawnEnabled = false;
    BeginRespawnAnimation();
  }

  public void Respawn()
  {
    if (respawnEnabled)
    {
      RespawnServerRpc();
    }
    else
    {
      Debug.Log("Can't respawn right now");
    }
  }

  public void BeginRespawnAnimation()
  {
    animator.SetBool("Dead", false);
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
    //If dead, do not update
    if (isDead.Value)
      return;
    //Check death conditions
    if (IsOwner && hlt.Current <= 0f)
    {
      PlayerDeathServerRpc();
      StartCoroutine(WaitForRespawn());
    }
    

    // update authoritative position
    if (NetworkManager.Singleton.IsServer)
    {
      Vector2 mvt = movementInput.Value.normalized * movementUnitsPerSec * Time.deltaTime;
      serverPosition += new Vector3(mvt.x, 0, mvt.y);
    }

    if (NetworkManager.Singleton.IsClient && clientPositions[0] != null && clientPositions[1] != null)
    {
      long timestep = clientPositions[0].timestamp - clientPositions[1].timestamp;
      long timeSinceUpdate = System.DateTime.Now.ToFileTime() - clientPositions[0].timestamp;
      float t = timeSinceUpdate > 0 ? timestep / timeSinceUpdate : 1;
      UIManager.Instance.debugString = t.ToString();
      // update local position
      transform.position = Vector3.Lerp(clientPositions[1].position, clientPositions[0].position, t);
    }

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
  
  [ServerRpc]
  public void PlayerDeathServerRpc()
  {
    isDead.Value = true;
    PlayerDeathClientRpc();
  }

  [ClientRpc]
  public void PlayerDeathClientRpc()
  {
    animator.SetBool("Dead", true);
  }

  private IEnumerator WaitForRespawn()
  {
    yield return new WaitForSeconds(respawnCooldown);
    respawnEnabled = true;
  }

  void OnGUI()
  {
    if (respawnEnabled)
    {
      GUILayout.BeginArea(new Rect(Screen.width / 2 - 50, 100, 300, 300));
      GUILayout.Label("Press 'R' to respawn");
      GUILayout.EndArea();
    }
  }
}
