using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class HealthBar : NetworkBehaviour
{
  [SerializeField]
  private Slider slider;
  private NetworkVariable<float> fill = new NetworkVariable<float>(1f);

  public void SetFill(float val)
  {
    fill.Value = Mathf.Clamp(val, 0f, 1f);
  }

  void Update()
  {
    slider.value = fill.Value;
  }

}
