using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public CoinManager coinManager; 
    public int itemPrice = 10; 

    public void BuyItem() {
        coinManager.SubtractCoins(itemPrice);
    }

    public void SellItem() {
        coinManager.AddCoins(itemPrice);
    }
}
