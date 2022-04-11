using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
  [SerializeField]
  private float maxHp = 100f;

  private NetworkVariable<float> hp = new NetworkVariable<float>();

  public override void OnNetworkSpawn()
  {
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