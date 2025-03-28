using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimalPen : MonoBehaviour
{
    [SerializeField] private ResourceType productType = ResourceType.None;
    [SerializeField] private GameObject animalPrefab;
    [SerializeField] private int initialAnimalCount = 2;
    [SerializeField] private int maxAnimalCount = 10;
    [SerializeField] private string productDisplayName = "Продукт";
    [SerializeField] private int costUpgrade = 25;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject penUIPanel;
    [SerializeField] private Button harvestButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI upgradeCountText;
    [SerializeField] private TextMeshProUGUI productCountText;
    
    private List<Animal> animals = new List<Animal>();
    private Bounds penBounds;
    private int upgradeCount = 0;
    
    private void Start()
    {
        // Определяем границы загона
        BoxCollider2D penCollider = GetComponent<BoxCollider2D>();
        if (penCollider != null)
        {
            penBounds = penCollider.bounds;
        }
        else
        {
            penBounds = new Bounds(transform.position, new Vector3(5, 5, 0));
        }
        
        // Добавляем начальных животных
        for (int i = 0; i < initialAnimalCount; i++)
        {
            AddAnimal();
        }
        
        // Настраиваем UI
        if (penUIPanel != null)
        {
            penUIPanel.SetActive(false);
        }
        
        if (harvestButton != null)
        {
            harvestButton.onClick.AddListener(HarvestProducts);
        }
        
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(UpgradePen);
            upgradeButton.interactable = (animals.Count < maxAnimalCount);
        }
        
        UpdateUITexts();
    }
    
    private void UpdateUITexts()
    {
        if (upgradeCountText != null)
        {
            upgradeCountText.text = $"Улучшения: {upgradeCount}/{maxAnimalCount - initialAnimalCount}";
        }
        
        if (productCountText != null)
        {
            productCountText.text = $"{productDisplayName}: {CountAvailableProducts()}/{animals.Count}";
        }
    }
    
    private void AddAnimal()
    {
        if (animals.Count >= maxAnimalCount || animalPrefab == null)
            return;
        
        // Выбираем случайную позицию внутри загона
        Vector3 randomPosition = new Vector3(
            Random.Range(penBounds.min.x + 0.5f, penBounds.max.x - 0.5f),
            Random.Range(penBounds.min.y + 0.5f, penBounds.max.y - 0.5f),
            0
        );
        
        GameObject animalObj = Instantiate(animalPrefab, randomPosition, Quaternion.identity);
        animalObj.transform.SetParent(transform);
        
        Animal animal = animalObj.GetComponent<Animal>();
        if (animal != null)
        {
            animal.SetPenBounds(penBounds);
            animals.Add(animal);
        }
    }
    
    private void OnMouseDown()
    {
        if(EventSystem.current.IsPointerOverGameObject())
            return;
        if (penUIPanel != null)
        {
            // Закрываем другие UI перед открытием нашего
            if (!penUIPanel.activeSelf)
            {
                CloseAllOtherPenUIs();
            }
            
            penUIPanel.SetActive(!penUIPanel.activeSelf);
            
            if (penUIPanel.activeSelf)
            {
                UpdateUITexts();
                if (upgradeButton != null)
                {
                    upgradeButton.interactable = (animals.Count < maxAnimalCount);
                }
            }
        }
    }
    
    private void CloseAllOtherPenUIs()
    {
        AnimalPen[] allPens = FindObjectsOfType<AnimalPen>();
        foreach (AnimalPen pen in allPens)
        {
            if (pen != this && pen.penUIPanel != null)
            {
                pen.penUIPanel.SetActive(false);
            }
        }
    }
    
    private void HarvestProducts()
    {
        int collectedProducts = 0;
        
        foreach (Animal animal in animals)
        {
            if (animal.HasProduct)
            {
                animal.CollectProduct();
                collectedProducts++;
            }
        }
        
        if (collectedProducts > 0 && productType != ResourceType.None)
        {
            // Отправляем продукты в GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddResource(productType, collectedProducts);
            }
            
            UpdateUITexts();
        }
    }
    
    private void UpgradePen()
    {
        if (!CoinManager.instance.SubtractCoins(costUpgrade))
            return;
        
        if (animals.Count < maxAnimalCount)
        {
            AddAnimal();
            upgradeCount++;
            
            UpdateUITexts();
            
            if (upgradeButton != null)
            {
                upgradeButton.interactable = (animals.Count < maxAnimalCount);
            }
        }
    }
    
    private int CountAvailableProducts()
    {
        int count = 0;
        foreach (Animal animal in animals)
        {
            if (animal.HasProduct)
            {
                count++;
            }
        }
        return count;
    }
}
