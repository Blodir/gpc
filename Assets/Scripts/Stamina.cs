using Unity.Netcode;
using UnityEngine;

public class Stamina : NetworkBehaviour
{
  [SerializeField]
  private float maxStamina = 100f;
  public float staminaRegenerationValue = 1f;
  private float regenerationWaitTime = 0;
  private PlayerCharacter playerCharacter;
  private NetworkVariable<float> stamina = new NetworkVariable<float>();

  public override void OnNetworkSpawn()
  {
    if (NetworkManager.Singleton.IsServer)
    {
      stamina.Value = maxStamina;
    }
  }

  public void TakeFatigue(float fatigue)
  {
    stamina.Value -= fatigue;
    if (stamina.Value < 0f) stamina.Value = 0f;
    Debug.Log($"{gameObject.name} took {fatigue} fatigue ({stamina.Value} stamina left)");
    if (stamina.Value <= 0f)
    {
      Debug.Log($"Oh no, {gameObject.name} can't take action :(");
    }
  }

  public void RegenerateStamina(bool playerInAction)
  {
      if (playerInAction)
      {
        regenerationWaitTime = 0;
      }
      else
      {
        regenerationWaitTime += Time.deltaTime;
        if (stamina.Value < 100f && regenerationWaitTime >= 1f)
        {
          stamina.Value += staminaRegenerationValue;
        }
      }
      if (stamina.Value > 100f) stamina.Value = 100f;
  }

  public float GetStamina()
  {
      return stamina.Value;
  }
}