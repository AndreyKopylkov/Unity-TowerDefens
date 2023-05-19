using TMPro;
using UnityEngine;

public class MoneyView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moneyTMP;

    private void OnEnable()
    {
        Money.OnChangeMoney += SetNewValue;
    }

    private void OnDisable()
    {
        Money.OnChangeMoney -= SetNewValue;
    }
    
    private void SetNewValue(int currentValue)
    {
        _moneyTMP.SetText($"Money: {currentValue}");
    }
}