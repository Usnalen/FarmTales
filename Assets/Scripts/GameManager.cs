using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    [System.Serializable]
    public class ResourceDisplay
    {
        public ResourceType resourceType;
        public TextMeshProUGUI displayText;
        public string displayName;
    }
    
    [Header("Resource Display")]
    [SerializeField] private List<ResourceDisplay> resourceDisplays = new List<ResourceDisplay>();
    
    private Dictionary<ResourceType, int> resourceCounts = new Dictionary<ResourceType, int>();
    private List<Factory> activeFactories = new List<Factory>();
    
    public delegate void ResourceChangedHandler(ResourceType resourceType, int newCount);
    public event ResourceChangedHandler OnResourceChanged;
    
    public static GameManager Instance => _instance;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType)))
            {
                if (resourceType != ResourceType.None)
                {
                    resourceCounts[resourceType] = 0;
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        UpdateAllResourceDisplays();
    }
    
    // Регистрация активной фабрики
    public void RegisterActiveFactory(Factory factory)
    {
        if (!activeFactories.Contains(factory))
        {
            activeFactories.Add(factory);
            Debug.Log($"Фабрика {factory.name} зарегистрирована");
        }
    }
    
    // Удаление фабрики из активных
    public void UnregisterActiveFactory(Factory factory)
    {
        if (activeFactories.Contains(factory))
        {
            activeFactories.Remove(factory);
            Debug.Log($"Фабрика {factory.name} удалена из активных");
        }
    }
    
    // Метод добавления ресурса
    public bool AddResource(ResourceType resourceType, int amount)
    {
        if (resourceType == ResourceType.None) 
            return false;
        
        if (Barn.Instance != null && !Barn.Instance.CanAddResources(amount))
        {
            Debug.Log("Амбар переполнен! Невозможно добавить ресурсы.");
            return false;
        }
        
        int oldCount = GetResourceCount(resourceType);
        
        if (resourceCounts.ContainsKey(resourceType))
        {
            resourceCounts[resourceType] += amount;
        }
        else
        {
            resourceCounts[resourceType] = amount;
        }
        
        int newCount = GetResourceCount(resourceType);
        Debug.Log($"Добавлено {amount} x {resourceType}, стало {newCount}");
        
        UpdateResourceDisplay(resourceType);
        UpdateAllActiveFactories(resourceType);
        
        if (OnResourceChanged != null)
        {
            OnResourceChanged.Invoke(resourceType, newCount);
        }
        
        if (Barn.Instance != null)
        {
            Barn.Instance.UpdateBarnUI();
        }
        
        return true;
    }
    
    // Метод использования ресурса
    public bool UseResource(ResourceType resourceType, int amount)
    {
        if (resourceType == ResourceType.None) 
            return false;
        
        if (resourceCounts.ContainsKey(resourceType) && resourceCounts[resourceType] >= amount)
        {
            int oldCount = resourceCounts[resourceType];
            resourceCounts[resourceType] -= amount;
            int newCount = resourceCounts[resourceType];
            
            Debug.Log($"Использовано {amount} x {resourceType}, осталось {newCount}");
            
            UpdateResourceDisplay(resourceType);
            UpdateAllActiveFactories(resourceType);
            
            if (OnResourceChanged != null)
            {
                OnResourceChanged.Invoke(resourceType, newCount);
            }
            
            return true;
        }
        
        return false;
    }
    
    // Метод прямого обновления всех активных фабрик
    public void UpdateAllActiveFactories(ResourceType resourceType)
    {
        List<Factory> factories = new List<Factory>(activeFactories);
        
        foreach (Factory factory in factories)
        {
            if (factory != null)
            {
                factory.UpdateResourceDisplay(resourceType);
            }
            else
            {
                activeFactories.Remove(factory);
            }
        }
    }
    
    // Получение количества ресурса
    public int GetResourceCount(ResourceType resourceType)
    {
        if (resourceCounts.ContainsKey(resourceType))
        {
            return resourceCounts[resourceType];
        }
        return 0;
    }
    
    // Обновление UI отображения конкретного ресурса
    private void UpdateResourceDisplay(ResourceType resourceType)
    {
        foreach (ResourceDisplay display in resourceDisplays)
        {
            if (display.resourceType == resourceType && display.displayText != null)
            {
                int count = GetResourceCount(resourceType);
                display.displayText.text = $"{display.displayName}: {count}";
            }
        }
    }
    
    // Обновление UI всех ресурсов
    private void UpdateAllResourceDisplays()
    {
        foreach (ResourceDisplay display in resourceDisplays)
        {
            UpdateResourceDisplay(display.resourceType);
        }
    }
    
    // Для тестирования - добавить случайные ресурсы
    public void AddRandomResources()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            if (type != ResourceType.None)
            {
                AddResource(type, Random.Range(1, 5));
            }
        }
    }
    
    // Метод для проверки наличия достаточного количества ресурсов
    public bool HasEnoughResources(ResourceType resourceType, int amount)
    {
        return GetResourceCount(resourceType) >= amount;
    }
    
    // Проверка наличия набора ресурсов
    public bool HasEnoughResources(Dictionary<ResourceType, int> requiredResources)
    {
        foreach (var requirement in requiredResources)
        {
            if (GetResourceCount(requirement.Key) < requirement.Value)
                return false;
        }
        return true;
    }
}
