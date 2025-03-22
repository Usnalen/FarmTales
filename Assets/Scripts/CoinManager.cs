using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    private int coinCount;
    public TMP_Text coinText; 

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
            Debug.LogWarning("Не хватает монет!");
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



