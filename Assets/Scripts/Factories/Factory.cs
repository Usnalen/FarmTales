using System.Collections;
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
    [SerializeField] protected Button closeButton;
    [SerializeField] protected Transform storageItemsParent;
    [SerializeField] protected TextMeshProUGUI factoryTitle;
    
    [Header("Formula Rows")]
    [SerializeField] protected List<FormulaRow> formulaRows = new List<FormulaRow>();
    
    [Header("Resource Spawn Points")]
    [SerializeField] protected List<ResourceSpawnPoint> resourceSpawnPoints = new List<ResourceSpawnPoint>();
    protected Dictionary<ResourceType, Vector2> resourceSpawnPositions = new Dictionary<ResourceType, Vector2>();
    
    [Header("Prefabs")]
    [SerializeField] protected GameObject storageItemPrefab;

    protected Dictionary<ResourceType, DraggableItem> resourceItems = new Dictionary<ResourceType, DraggableItem>();
    
    protected virtual void Awake()
    {
        foreach (ResourceSpawnPoint spawnPoint in resourceSpawnPoints)
        {
            if (!resourceSpawnPositions.ContainsKey(spawnPoint.resourceType))
            {
                resourceSpawnPositions.Add(spawnPoint.resourceType, spawnPoint.spawnPosition);
            }
        }
        
        // Если FormulaRows не были заданы в инспекторе, найдем их
        if (formulaRows.Count == 0)
        {
            // Находим все строки формул в дочерних объектах
            FormulaRow[] rows = GetComponentsInChildren<FormulaRow>(true);
            formulaRows.AddRange(rows);
        }
    }
    
    protected virtual void Start()
    {
        if (factoryUI != null)
        {
            factoryUI.SetActive(false);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseFactoryUI);
        }
    }
    
    protected virtual void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterActiveFactory(this);
        }
    }
    
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
        
        Canvas.ForceUpdateCanvases();
    }
    
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
                }
                else
                {
                    item.SetItem(GetResourceIcon(resourceType), 0, resourceType);
                    SetDraggableItemActive(item, false);
                }

                Canvas.ForceUpdateCanvases();
            }
        }
        else if (count > 0 || (factoryUI != null && factoryUI.activeSelf))
        {
            CreateResourceItemAtPosition(resourceType, count);
        }
    }
    
    public Sprite GetResourceIcon(ResourceType resourceType)
    {
        return ResourceIconManager.Instance.GetIcon(resourceType);
    }
    
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
    
    
    public virtual bool IsValidIngredientForSlot(IngredientSlot slot, ResourceType resourceType)
    {
        foreach (FormulaRow row in formulaRows)
        {
            if (row.ContainsSlot(slot))
            {
                Recipe recipe = row.GetRecipe();
                return recipe != null && recipe.ingredients.ContainsKey(resourceType);
            }
        }
        
        return GetAllowedStorageResourceTypes().Contains(resourceType);
    }
    
    protected abstract List<ResourceType> GetAllowedStorageResourceTypes();
}
