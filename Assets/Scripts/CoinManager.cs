using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    public int totalCoins; 
    public Text coinText; 
 
    void Start() 
    { 
        totalCoins = 100; 
        UpdateCoinText(); 
    } 
 
    
    public void AddCoins(int amount) 
    { 
        if (amount < 0) return; 
        totalCoins += amount; 
        UpdateCoinText(); 
    } 
 
   
    public void SpendCoins(int amount)
       {
           if (totalCoins >= amount)
           {
               totalCoins -= amount;
               UpdateCoinText();
           }
           else
           {
               Debug.Log("Недостаточно монет!");
           }
       }
 
    
     public void BuyItem(int itemCost)
   {
       SpendCoins(itemCost);
       Debug.Log("Куплено!");
   }

   public void SellItem(int itemSalePrice)
   {
       AddCoins(itemSalePrice);
       Debug.Log("Продано!");
   }
    
    private void UpdateCoinText() 
    { 
        coinText.text = "Монеты: " + totalCoins.ToString(); 
    }
}
