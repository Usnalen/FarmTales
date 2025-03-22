using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    [System.Serializable]
    public class ResourceDisplay
    {
        public ResourceType resourceType;
        public Text displayText;
        public string displayName;
    }
    
    [Header("Resource Display")]
    [SerializeField] private List<ResourceDisplay> resourceDisplays = new List<ResourceDisplay>();
    
    private Dictionary<ResourceType, int> resourceCounts = new Dictionary<ResourceType, int>();
    
    public static GameManager Instance => _instance;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Инициализация словаря ресурсов
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
    
    public bool AddResource(ResourceType resourceType, int amount)
    {
        if (resourceType == ResourceType.None) 
            return false;

        if (resourceCounts.ContainsKey(resourceType))
        {
            resourceCounts[resourceType] += amount;
        }
        else
        {
            resourceCounts[resourceType] = amount;
        }
    
        UpdateResourceDisplay(resourceType);
        
        return true;
    }
    
    public int GetResourceCount(ResourceType resourceType)
    {
        if (resourceCounts.ContainsKey(resourceType))
        {
            return resourceCounts[resourceType];
        }
        return 0;
    }
    
    public bool UseResource(ResourceType resourceType, int amount)
    {
        if (resourceType == ResourceType.None) 
            return false;
    
        if (resourceCounts.ContainsKey(resourceType) && resourceCounts[resourceType] >= amount)
        {
            resourceCounts[resourceType] -= amount;
            UpdateResourceDisplay(resourceType);
            return true;
        }
    
        return false;
    }
    
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
    
    private void UpdateAllResourceDisplays()
    {
        foreach (ResourceDisplay display in resourceDisplays)
        {
            UpdateResourceDisplay(display.resourceType);
        }
    }
}
