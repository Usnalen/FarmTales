using System.Collections.Generic;
using UnityEngine;

public class ResourceIconManager : MonoBehaviour
{
    [System.Serializable]
    public class ResourceIcon
    {
        public ResourceType resourceType;
        public Sprite icon;
    }
    
    [Header("Иконки ресурсов")]
    [SerializeField] private List<ResourceIcon> resourceIcons = new List<ResourceIcon>();
    
    // Словарь для быстрого доступа к иконкам
    private Dictionary<ResourceType, Sprite> resourceIconsDict = new Dictionary<ResourceType, Sprite>();
    
    // Реализация синглтона
    private static ResourceIconManager _instance;
    public static ResourceIconManager Instance => _instance;
    
    private void Awake()
    {
        // Проверка на единственный экземпляр
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // Инициализация словаря иконок
        InitializeIconsDictionary();
    }
    
    // Инициализация словаря иконок
    private void InitializeIconsDictionary()
    {
        resourceIconsDict.Clear();
        
        foreach (ResourceIcon icon in resourceIcons)
        {
            if (!resourceIconsDict.ContainsKey(icon.resourceType))
            {
                resourceIconsDict.Add(icon.resourceType, icon.icon);
            }
            else
            {
                Debug.LogWarning($"Дублирующаяся иконка для ресурса {icon.resourceType}");
            }
        }
    }
    
    // Получение иконки по типу ресурса
    public Sprite GetIcon(ResourceType resourceType)
    {
        if (resourceIconsDict.TryGetValue(resourceType, out Sprite icon))
        {
            return icon;
        }
        
        Debug.LogWarning($"Иконка для ресурса {resourceType} не найдена");
        return null;
    }
    
    // Добавление новой иконки в рантайме
    public void AddIcon(ResourceType resourceType, Sprite icon)
    {
        if (resourceIconsDict.ContainsKey(resourceType))
        {
            resourceIconsDict[resourceType] = icon;
        }
        else
        {
            resourceIconsDict.Add(resourceType, icon);
            
            // Добавляем и в сериализуемый список для видимости в редакторе
            ResourceIcon newResourceIcon = new ResourceIcon
            {
                resourceType = resourceType,
                icon = icon
            };
            
            resourceIcons.Add(newResourceIcon);
        }
    }
    
    // Проверка наличия иконки для типа ресурса
    public bool HasIcon(ResourceType resourceType)
    {
        return resourceIconsDict.ContainsKey(resourceType);
    }
    
    // Загрузка иконок из ресурсов (опционально)
    public void LoadIconsFromResources(string pathToIcons)
    {
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>(pathToIcons);
        
        foreach (Sprite sprite in loadedSprites)
        {
            // Предполагаем, что имя спрайта соответствует имени перечисления ResourceType
            if (System.Enum.TryParse(sprite.name, out ResourceType resourceType))
            {
                AddIcon(resourceType, sprite);
            }
        }
    }
}
