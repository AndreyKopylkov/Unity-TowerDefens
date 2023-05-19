using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthTMP;

    private void OnEnable()
    {
        Game.OnChangeHealth += SetNewValue;
    }

    private void OnDisable()
    {
        Game.OnChangeHealth -= SetNewValue;
    }
    
    private void SetNewValue(int currentValue, int startValue)
    {
        _healthTMP.SetText($"Health: {currentValue}/{startValue}");
    }
}
