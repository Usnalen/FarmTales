using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Header("Настройки UI")]
    [SerializeField] private GameObject shopUI; // Панель UI магазина
    [SerializeField] private Transform itemsContainer; // Контейнер для товаров
    [SerializeField] private GameObject shopItemPrefab; // Префаб для отображения товара
    
    // Словарь цен на товары
    private Dictionary<ResourceType, int> itemPrices = new Dictionary<ResourceType, int>
    {
        { ResourceType.Bun, 10 },             // Булочка
        { ResourceType.Сupcake, 15 },         // Кекс
        { ResourceType.Cake, 30 },            // Торт
        { ResourceType.AppleCake, 50 },       // Торт яблочный
        { ResourceType.RaspberryCake, 60 },   // Торт малиновый
        { ResourceType.StrawberryCake, 60 }   // Торт клубничный
    };
    
    // Список типов ресурсов, которые можно продавать
    private List<ResourceType> sellableItems = new List<ResourceType>
    {
        ResourceType.Bun,
        ResourceType.Сupcake,
        ResourceType.Cake,
        ResourceType.AppleCake,
        ResourceType.RaspberryCake,
        ResourceType.StrawberryCake
    };
    
    // Ссылка на экземпляр класса для паттерна Singleton
    private static Shop _instance;
    public static Shop Instance => _instance;
    
    private void Awake()
    {
        // Реализация синглтона
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false);
        }
    }
    
    private void OnMouseDown()
    {
        if(EventSystem.current.IsPointerOverGameObject())
            return;
        ToggleShopUI();
    }
    
    public void ToggleShopUI()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(!shopUI.activeSelf);
            
            if (shopUI.activeSelf)
            {
                UpdateShopUI();
            }
        }
    }
    
    public void UpdateShopUI()
    {
        if (itemsContainer == null || GameManager.Instance == null)
            return;
        
        // Очищаем текущий список товаров
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Отображаем только товары, которые есть у игрока
        foreach (ResourceType resourceType in sellableItems)
        {
            int count = GameManager.Instance.GetResourceCount(resourceType);
            
            if (count > 0)
            {
                CreateShopItemDisplay(resourceType, count);
            }
        }
    }
    
    private void CreateShopItemDisplay(ResourceType resourceType, int count)
    {
        if (shopItemPrefab == null || itemsContainer == null)
            return;
        
        GameObject itemObject = Instantiate(shopItemPrefab, itemsContainer);
        ShopItemDisplay itemDisplay = itemObject.GetComponent<ShopItemDisplay>();
        
        if (itemDisplay != null)
        {
            Sprite icon = ResourceIconManager.Instance.GetIcon(resourceType);
            int price = GetItemPrice(resourceType);
            
            itemDisplay.SetItem(icon, resourceType, count, price);
            itemDisplay.OnSellButtonClicked += SellItem;
        }
    }
    
    public int GetItemPrice(ResourceType resourceType)
    {
        if (itemPrices.ContainsKey(resourceType))
        {
            return itemPrices[resourceType];
        }
        return 0; // Если товар не найден, цена равна 0
    }
    
    public void SellItem(ResourceType resourceType, int amount)
    {
        if (amount <= 0 || GameManager.Instance == null)
            return;
        
        // Проверяем, есть ли такой товар в списке продаваемых
        if (!sellableItems.Contains(resourceType))
        {
            Debug.LogWarning($"Ресурс {resourceType} нельзя продать в магазине!");
            return;
        }
        
        int availableCount = GameManager.Instance.GetResourceCount(resourceType);
        
        // Проверяем, достаточно ли товаров у игрока
        if (availableCount < amount)
        {
            Debug.LogWarning($"Недостаточно товаров для продажи! Доступно: {availableCount}, требуется: {amount}");
            return;
        }
        
        // Вычисляем стоимость товаров
        int totalPrice = GetItemPrice(resourceType) * amount;
        
        // Удаляем товары из амбара
        GameManager.Instance.UseResource(resourceType, amount);
        
        // Добавляем монеты игроку
        CoinManager.instance.AddCoins(totalPrice);
        
        // Обновляем UI магазина
        UpdateShopUI();
        
        // Обновляем UI амбара, если он открыт
        if (Barn.Instance != null)
        {
            Barn.Instance.UpdateBarnUI();
        }
        
        Debug.Log($"Продано {amount} ед. {resourceType} за {totalPrice} монет");
    }
}
