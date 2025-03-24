using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Factory : MonoBehaviour
{
    [System.Serializable]
    public class ResourceSpawnPoint
    {
        public ResourceType resourceType;
        public Vector2 spawnPosition;
    }
    
    [System.Serializable]
    public class ResourceIcon
    {
        public ResourceType resourceType;
        public Sprite icon;
    }
    
    [Header("UI Elements")]
    [SerializeField] protected GameObject factoryUI;
    [SerializeField] protected Button createButton;
    [SerializeField] protected Button closeButton;
    [SerializeField] protected Transform resultSlotParent;
    [SerializeField] protected Transform ingredientSlotsParent;
    [SerializeField] protected Transform storageItemsParent;
    [SerializeField] protected TextMeshProUGUI factoryTitle;
    
    [Header("Resource Spawn Points")]
    [SerializeField] protected List<ResourceSpawnPoint> resourceSpawnPoints = new List<ResourceSpawnPoint>();
    protected Dictionary<ResourceType, Vector2> resourceSpawnPositions = new Dictionary<ResourceType, Vector2>();
    
    [Header("Prefabs")]
    [SerializeField] protected GameObject storageItemPrefab;
    
    [Header("Icons")]
    [SerializeField] protected List<ResourceIcon> resourceIcons = new List<ResourceIcon>();
    
    protected Dictionary<ResourceType, Sprite> resourceIconsDict = new Dictionary<ResourceType, Sprite>();
    protected List<IngredientSlot> ingredientSlots = new List<IngredientSlot>();
    protected IngredientSlot resultSlot;
    protected Dictionary<ResourceType, DraggableItem> resourceItems = new Dictionary<ResourceType, DraggableItem>();
    protected List<Recipe> recipes = new List<Recipe>();
    
    protected virtual void Awake()
    {
        foreach (ResourceIcon icon in resourceIcons)
        {
            if (!resourceIconsDict.ContainsKey(icon.resourceType))
            {
                resourceIconsDict.Add(icon.resourceType, icon.icon);
            }
        }
        
        foreach (ResourceSpawnPoint spawnPoint in resourceSpawnPoints)
        {
            if (!resourceSpawnPositions.ContainsKey(spawnPoint.resourceType))
            {
                resourceSpawnPositions.Add(spawnPoint.resourceType, spawnPoint.spawnPosition);
            }
        }
        
        if (ingredientSlotsParent != null)
        {
            IngredientSlot[] slots = ingredientSlotsParent.GetComponentsInChildren<IngredientSlot>();
            ingredientSlots.AddRange(slots);
        }
        
        if (resultSlotParent != null)
        {
            resultSlot = resultSlotParent.GetComponentInChildren<IngredientSlot>();
        }
    }
    
    protected virtual void Start()
    {
        if (factoryUI != null)
        {
            factoryUI.SetActive(false);
        }
        
        if (createButton != null)
        {
            createButton.onClick.AddListener(OnCreateButtonClicked);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseFactoryUI);
        }
    }
    
    // Регистрируем фабрику при активации
    protected virtual void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterActiveFactory(this);
        }
    }
    
    // Отменяем регистрацию при деактивации
    protected virtual void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterActiveFactory(this);
        }
    }
    
    protected virtual void OnMouseDown()
    {
        OpenFactoryUI();
    }
    
    public virtual void OpenFactoryUI()
    {
        if (factoryUI != null)
        {
            Factory[] factories = FindObjectsOfType<Factory>();
            foreach (Factory factory in factories)
            {
                if (factory != this && factory.factoryUI != null && factory.factoryUI.activeSelf)
                {
                    factory.CloseFactoryUI();
                }
            }
            
            factoryUI.SetActive(true);
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterActiveFactory(this);
            }
            
            UpdateStorageItems();
        }
    }
    
    public virtual void CloseFactoryUI()
    {
        if (factoryUI != null)
        {
            factoryUI.SetActive(false);
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnregisterActiveFactory(this);
            }
        }
    }
    
    public virtual bool IsActive()
    {
        return factoryUI != null && factoryUI.activeSelf;
    }
    
    public virtual void UpdateStorageItems()
    {
        if (storageItemsParent == null || GameManager.Instance == null)
            return;
        
        foreach (ResourceType resourceType in GetAllowedStorageResourceTypes())
        {
            UpdateResourceDisplay(resourceType);
        }
    }
    
    // Метод для обновления конкретного ресурса
    public virtual void UpdateResourceDisplay(ResourceType resourceType)
    {
        if (GameManager.Instance == null || storageItemsParent == null ||
            !GetAllowedStorageResourceTypes().Contains(resourceType))
            return;
            
        int count = GameManager.Instance.GetResourceCount(resourceType);

        if (resourceItems.ContainsKey(resourceType))
        {
            DraggableItem item = resourceItems[resourceType];

            if (item != null)
            {
                if (count > 0)
                {
                    item.SetItem(GetResourceIcon(resourceType), count, resourceType);
                    item.gameObject.SetActive(true);
                    SetDraggableItemActive(item, true);
                    Debug.Log($"Обновлен ресурс {resourceType}: {count} (активен)");
                }
                else
                {
                    item.SetItem(GetResourceIcon(resourceType), 0, resourceType);
                    SetDraggableItemActive(item, false);
                    Debug.Log($"Обновлен ресурс {resourceType}: пустышка (неактивен)");
                }

                Canvas parentCanvas = storageItemsParent.GetComponentInParent<Canvas>();
                if (parentCanvas != null)
                {
                    Canvas.ForceUpdateCanvases();

                    CanvasScaler scaler = parentCanvas.GetComponent<CanvasScaler>();
                    if (scaler != null)
                    {
                        float oldScale = scaler.scaleFactor;
                        scaler.scaleFactor = oldScale + 0.001f;
                        scaler.scaleFactor = oldScale;
                    }
                }
            }
        }
        else if (count > 0 || (factoryUI != null && factoryUI.activeSelf))
        {
            CreateResourceItemAtPosition(resourceType, count);
            Debug.Log($"Создан новый элемент для {resourceType}: {count}");
        }
    }
    
    // Метод для получения иконки ресурса
    protected Sprite GetResourceIcon(ResourceType resourceType)
    {
        if (resourceIconsDict.TryGetValue(resourceType, out Sprite icon))
        {
            return icon;
        }
        return null;
    }
    
    // Метод для создания ресурса в определенной позиции
    protected virtual void CreateResourceItemAtPosition(ResourceType resourceType, int count)
    {
        if (storageItemPrefab == null || storageItemsParent == null)
            return;
        
        GameObject itemObject = Instantiate(storageItemPrefab, storageItemsParent);
        DraggableItem item = itemObject.GetComponent<DraggableItem>();
        
        if (item != null)
        {
            Sprite icon = GetResourceIcon(resourceType);
            if (icon != null)
            {
                item.SetItem(icon, count, resourceType);
            }
            
            RectTransform rectTransform = itemObject.GetComponent<RectTransform>();
            if (rectTransform != null && resourceSpawnPositions.TryGetValue(resourceType, out Vector2 position))
            {
                rectTransform.anchoredPosition = position;
            }
            
            if (count <= 0)
            {
                SetDraggableItemActive(item, false);
            }
            
            resourceItems[resourceType] = item;
        }
    }
    
    // Включение/выключение интерактивности для DraggableItem
    protected void SetDraggableItemActive(DraggableItem item, bool active)
    {
        if (item == null) return;
        
        CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = active ? 1.0f : 0.3f;
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
        }
    }
    
    protected virtual void OnCreateButtonClicked()
    {
        Dictionary<ResourceType, int> providedIngredients = new Dictionary<ResourceType, int>();
        
        foreach (IngredientSlot slot in ingredientSlots)
        {
            ResourceType type = slot.GetResourceType();
            if (type != ResourceType.None)
            {
                int slotCount = slot.GetCount();
                if (providedIngredients.ContainsKey(type))
                {
                    providedIngredients[type] += slotCount;
                }
                else
                {
                    providedIngredients[type] = slotCount;
                }
            }
        }
        
        Recipe matchingRecipe = FindMatchingRecipe(providedIngredients);
        
        if (matchingRecipe != null)
        {
            bool hasEnoughIngredients = true;
            foreach (var ingredient in matchingRecipe.ingredients)
            {
                if (!providedIngredients.ContainsKey(ingredient.Key) || 
                    providedIngredients[ingredient.Key] < ingredient.Value)
                {
                    hasEnoughIngredients = false;
                    break;
                }
            }
            
            if (hasEnoughIngredients)
            {
                foreach (var ingredient in matchingRecipe.ingredients)
                {
                    int remainingAmount = ingredient.Value;
                    
                    foreach (IngredientSlot slot in ingredientSlots)
                    {
                        if (slot.GetResourceType() == ingredient.Key && remainingAmount > 0)
                        {
                            int amountInSlot = slot.GetCount();
                            int amountToRemove = Mathf.Min(amountInSlot, remainingAmount);
                            
                            slot.RemoveAmount(amountToRemove);
                            remainingAmount -= amountToRemove;
                            
                            if (remainingAmount <= 0)
                                break;
                        }
                    }
                }
                
                if (resultSlot != null)
                {
                    ResourceType currentResultType = resultSlot.GetResourceType();
                    
                    if (currentResultType == matchingRecipe.resultType)
                    {
                        resultSlot.AddAmount(1);
                        Debug.Log($"Добавлен {matchingRecipe.recipeName} в результат (всего: {resultSlot.GetCount()})");
                    }
                    else
                    {
                        if (currentResultType != ResourceType.None && GameManager.Instance != null)
                        {
                            GameManager.Instance.AddResource(currentResultType, resultSlot.GetCount());
                            Debug.Log($"Возвращен старый продукт {currentResultType} в хранилище");
                        }
                        
                        Sprite resultIcon = matchingRecipe.resultIcon;
                        if (resultIcon == null && resourceIconsDict.TryGetValue(matchingRecipe.resultType, out resultIcon))
                        {
                        }
                        
                        resultSlot.SetResource(matchingRecipe.resultType, 1, resultIcon);
                        Debug.Log($"Создан новый {matchingRecipe.recipeName}");
                    }
                }
            }
            else
            {
                Debug.Log("Недостаточно ингредиентов для создания продукта");
            }
        }
        else
        {
            Debug.Log("Нет подходящего рецепта для этих ингредиентов");
        }
    }
    
    // Метод для проверки, подходит ли ресурс для конкретного слота
    public virtual bool IsValidIngredientForSlot(IngredientSlot slot, ResourceType resourceType)
    {
        return GetAllowedStorageResourceTypes().Contains(resourceType);
    }
    
    // Метод для получения списка разрешенных типов ресурсов
    protected abstract List<ResourceType> GetAllowedStorageResourceTypes();
    
    // Метод для нахождения подходящего рецепта
    protected abstract Recipe FindMatchingRecipe(Dictionary<ResourceType, int> providedIngredients);
}
