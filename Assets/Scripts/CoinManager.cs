using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 
public class CoinManager : MonoBehaviour
{
    private int coinCount;
    public TMP_Text coinText; 

    void Start() {
        coinCount = 100; 
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

    
    public void SubtractCoins(int amount) {
        if (amount < 0) {
            Debug.LogError("Вычитаемая сумма не может быть отрицательной.");
            return;
        }
        if (coinCount >= amount) {
            coinCount -= amount;
            UpdateCoinText();
        } else {
            Debug.LogWarning("Не хватает монет!");
        }
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


