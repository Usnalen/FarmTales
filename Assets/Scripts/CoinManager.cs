using System;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;
    public TMP_Text coinText;
    
    private int coinCount;

    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Start() {
        coinCount = 10; 
        UpdateCoinText();
    }

    public void AddCoins(int amount) {
        if (amount < 0) {
            Debug.LogError("Добавляемая сумма не может быть отрицательной.");
            return;
        }
        coinCount += amount;
        UpdateCoinText();
    }

    public bool SubtractCoins(int amount) {
        if (amount < 0) {
            Debug.LogError("Вычитаемая сумма не может быть отрицательной.");
            return false;
        }
        if (coinCount >= amount) {
            coinCount -= amount;
            UpdateCoinText();
            return true; // Возвращаем true, если вычитание прошло успешно
        } else {
            Debug.Log("Не хватает монет!");
            return false; // Возвращаем false, если недостаточно монет
        }
    }

    public bool HasEnoughCoins(int amount) {
        return coinCount >= amount; // Возвращает true, если монет достаточно
    }

    private void UpdateCoinText() {
        if (coinText != null) {
            coinText.text = coinCount.ToString();
        } 
        else {
            Debug.LogError("CoinText не назначен в инспекторе!");
        }
    }
}



