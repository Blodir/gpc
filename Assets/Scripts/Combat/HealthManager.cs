using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
  [SerializeField]
  private float maxHp = 100f;

  private float hp;
  // Start is called before the first frame update
  void Start()
  {
    hp = maxHp;
  }

  public void TakeDamage(float val)
  {
    hp -= val;
    Debug.Log($"{gameObject.name} took {val} damage ({hp} hp left)");
    if (hp <= 0f)
    {
      Debug.Log($"Oh no, {gameObject.name} died :(");
      Die();
    }
  }

  private void Die()
  {
    Destroy(gameObject);
  }
}
