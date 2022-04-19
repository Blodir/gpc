using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Health : NetworkBehaviour
{
  [SerializeField]
  private float maxHp = 100f;

  private NetworkVariable<float> hp = new NetworkVariable<float>();

  public override void OnNetworkSpawn()
  {
    //Check that there is a proper collider
    Collider col = GetComponent<Collider>();
    if (col == null || col.isTrigger)
    {
      Debug.LogWarning("Health script needs a non-trigger collider on the script holder to work properly!");
    }

    if (NetworkManager.Singleton.IsServer)
    {
      hp.Value = maxHp;
    }
  }

  public void TakeDamage(float damage)
  {
    hp.Value -= damage;
    Debug.Log($"{gameObject.name} took {damage} damage ({hp.Value} hp left)");
    if (hp.Value <= 0f)
    {
      Debug.Log($"Oh no, {gameObject.name} died :(");
      Destroy(gameObject);
    }
  }
}