using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Barn : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private int maxCapacity = 50; // Максимальная вместимость амбара
    
    [Header("UI элементы")]
    [SerializeField] private GameObject barnUI; // Панель UI амбара
    [SerializeField] private Transform itemsContainer; // Контейнер для отображения предметов
    [SerializeField] private GameObject itemPrefab; // Префаб для отображения одного типа предмета
    [SerializeField] private TextMeshProUGUI capacityText; // Текст для отображения текущей вместимости
    
    [Header("Иконки ресурсов")]
    [SerializeField] private List<ResourceIcon> resourceIcons = new List<ResourceIcon>();
    
    // Словарь иконок ресурсов для быстрого доступа
    private Dictionary<ResourceType, Sprite> resourceIconsDict = new Dictionary<ResourceType, Sprite>();
    
    // Ссылка на экземпляр класса для организации паттерна Singleton
    private static Barn _instance;
    public static Barn Instance => _instance;
    
    // Событие при обновлении амбара
    public delegate void OnBarnUpdatedDelegate();
    public event OnBarnUpdatedDelegate OnBarnUpdated;
    
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
            return;
        }
        
        // Инициализация словаря иконок
        foreach (ResourceIcon icon in resourceIcons)
        {
            if (!resourceIconsDict.ContainsKey(icon.resourceType))
            {
                resourceIconsDict.Add(icon.resourceType, icon.icon);
            }
        }
    }
    
    private void Start()
    {
        if (barnUI != null)
        {
            barnUI.SetActive(false);
        }
    }
    
    private void OnMouseDown()
    {
        ToggleBarnUI();
    }
    
    public void ToggleBarnUI()
    {
        if (barnUI != null)
        {
            barnUI.SetActive(!barnUI.activeSelf);
            
            if (barnUI.activeSelf)
            {
                UpdateBarnUI();
            }
        }
    }
    
    public void UpdateBarnUI()
    {
        if (itemsContainer == null || GameManager.Instance == null)
            return;
        
        // Очищаем текущий список предметов
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }
        
        int totalItems = 0;
        
        // Для каждого типа ресурса в игре
        foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType)))
        {
            if (resourceType == ResourceType.None)
                continue;
            
            int count = GameManager.Instance.GetResourceCount(resourceType);
            totalItems += count;
            
            // Если есть хотя бы один ресурс этого типа, отображаем его
            if (count > 0)
            {
                CreateResourceDisplay(resourceType, count);
            }
        }
        
        // Обновляем текст вместимости
        if (capacityText != null)
        {
            capacityText.text = $"Вместимость: {totalItems}/{maxCapacity}";
        }
    }
    
    private void CreateResourceDisplay(ResourceType resourceType, int count)
    {
        if (itemPrefab == null || itemsContainer == null)
            return;
        
        GameObject itemObject = Instantiate(itemPrefab, itemsContainer);
        BarnItemDisplay itemDisplay = itemObject.GetComponent<BarnItemDisplay>();
        
        if (itemDisplay != null)
        {
            Sprite icon = null;
            if (resourceIconsDict.TryGetValue(resourceType, out icon))
            {
                itemDisplay.SetItem(icon, count, resourceType);
            }
        }
    }
    
    public bool CanAddResources(int amount)
    {
        int currentTotal = 0;
        
        // Подсчитываем текущее количество ресурсов
        if (GameManager.Instance != null)
        {
            foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType)))
            {
                if (resourceType == ResourceType.None)
                    continue;
                
                currentTotal += GameManager.Instance.GetResourceCount(resourceType);
            }
        }
        
        // Проверяем, не превысит ли добавление нового количества максимальную вместимость
        return (currentTotal + amount) <= maxCapacity;
    }
    
    // Проверка наличия ресурсов
    public bool HasEnoughResources(ResourceType resourceType, int amount)
    {
        if (GameManager.Instance == null)
            return false;
        
        return GameManager.Instance.GetResourceCount(resourceType) >= amount;
    }
    
    // Проверка наличия набора ресурсов
    public bool HasEnoughResources(Dictionary<ResourceType, int> requiredResources)
    {
        if (GameManager.Instance == null)
            return false;
        
        foreach (var requirement in requiredResources)
        {
            if (GameManager.Instance.GetResourceCount(requirement.Key) < requirement.Value)
                return false;
        }
        return true;
    }
    
    // Метод для увеличения вместимости амбара (для возможных улучшений)
    public void UpgradeCapacity(int additionalCapacity)
    {
        maxCapacity += additionalCapacity;
        
        // Обновляем UI, если он активен
        if (barnUI != null && barnUI.activeSelf)
        {
            UpdateBarnUI();
        }
        
        if (OnBarnUpdated != null)
        {
            OnBarnUpdated.Invoke();
        }
    }
    
    [System.Serializable]
    public class ResourceIcon
    {
        public ResourceType resourceType;
        public Sprite icon;
    }
}
