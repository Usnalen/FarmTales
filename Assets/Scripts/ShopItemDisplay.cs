using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDisplay : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button sellAllButton;
    
    private ResourceType itemType;
    private int itemCount;
    private int itemPrice;
    
    // Событие, которое будет вызываться при нажатии на кнопку продажи
    public event Action<ResourceType, int> OnSellButtonClicked;
    
    private void Awake()
    {
        if (sellButton != null)
        {
            sellButton.onClick.AddListener(OnSellOne);
        }
        
        if (sellAllButton != null)
        {
            sellAllButton.onClick.AddListener(OnSellAll);
        }
    }
    
    private void OnDestroy()
    {
        if (sellButton != null)
        {
            sellButton.onClick.RemoveListener(OnSellOne);
        }
        
        if (sellAllButton != null)
        {
            sellAllButton.onClick.RemoveListener(OnSellAll);
        }
    }
    
    public void SetItem(Sprite icon, ResourceType type, int count, int price)
    {
        itemType = type;
        itemCount = count;
        itemPrice = price;
        
        if (itemIcon != null)
        {
            itemIcon.sprite = icon;
        }
        
        if (itemNameText != null)
        {
            itemNameText.text = GetItemName(type);
        }
        
        if (itemCountText != null)
        {
            itemCountText.text = "Кол-во:\n" + count.ToString();
        }
        
        if (itemPriceText != null)
        {
            itemPriceText.text = $"{price}м.";
        }
    }
    
    public void UpdateCount(int newCount)
    {
        itemCount = newCount;
        
        if (itemCountText != null)
        {
            itemCountText.text = newCount.ToString();
        }
    }
    
    private void OnSellOne()
    {
        if (itemCount > 0 && OnSellButtonClicked != null)
        {
            OnSellButtonClicked.Invoke(itemType, 1);
        }
    }
    
    private void OnSellAll()
    {
        if (itemCount > 0 && OnSellButtonClicked != null)
        {
            OnSellButtonClicked.Invoke(itemType, itemCount);
        }
    }
    
    private string GetItemName(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Bun:
                return "Булочка";
            case ResourceType.Сupcake:
                return "Кекс";
            case ResourceType.Cake:
                return "Торт";
            case ResourceType.AppleCake:
                return "Торт яблочный";
            case ResourceType.RaspberryCake:
                return "Торт малиновый";
            case ResourceType.StrawberryCake:
                return "Торт клубничный";
            default:
                return type.ToString();
        }
    }
}
