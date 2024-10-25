using UnityEngine;
using TMPro;

public class EconomyManager : MonoBehaviour
{
    public int startingMoney = 1000;
    public int currentMoney;
    
    public TextMeshProUGUI moneyText;

    void Start()
    {
        currentMoney = startingMoney;
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true; 
        }
        else
        {
            Debug.Log("Not enough money!");
            return false;
        }
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "$" + currentMoney.ToString();
        }
    }
}
