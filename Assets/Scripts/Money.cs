using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    private int _currentMoney;
    
    private static Money _instance;

    public static event Action<int> OnChangeMoney;

    private void OnEnable()
    {
        _instance = this;
    }
    
    public void Initialize(int startMoney)
    {
        _currentMoney = startMoney;
        OnChangeMoney?.Invoke(_instance._currentMoney);
    }
    
    public static void AddMoney(int additionalValue)
    {
        _instance._currentMoney += additionalValue;
        OnChangeMoney?.Invoke(_instance._currentMoney);
    }

    public static bool TryDecreaseMoney(int priceValue)
    {
        if (_instance._currentMoney < priceValue)
            return false;

        _instance._currentMoney -= priceValue;
        OnChangeMoney?.Invoke(_instance._currentMoney);
        return true;
    }
}
