using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Plant : MonoBehaviour
{
    [System.Serializable]
    public class StateObjects
    {
        public PlantState state;
        public GameObject[] objects;
    }
    
    public enum PlantState
    {
        Empty,
        Growing,
        Ready
    }
    
    [Header("Настройки растения")]
    [SerializeField] private PlantSettings plantSettings;
    
    [Header("Визуальные состояния")]
    [SerializeField] private List<StateObjects> stateObjectsList = new List<StateObjects>();
    private Dictionary<PlantState, GameObject[]> stateObjectsDict = new Dictionary<PlantState, GameObject[]>();
    
    [Header("UI элементы")]
    [SerializeField] private GameObject plantBedUI;
    [SerializeField] private Button plantButton;
    [SerializeField] private Button harvestButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI upgradeCountText;
    [SerializeField] private TextMeshProUGUI productNameText;
    
    private PlantState currentState = PlantState.Empty;
    private float growthTimer = 0f;
    private int upgradeCount = 0;
    private Coroutine growthCoroutine;
    private float currentGrowthTime;
    
    private void Awake()
    {
        // Инициализация словаря из списка
        InitializeStateObjectsDictionary();
        
        if (plantSettings != null)
        {
            currentGrowthTime = plantSettings.initialGrowthTime;
        }
    }
    
    private void InitializeStateObjectsDictionary()
    {
        stateObjectsDict.Clear();
        foreach (StateObjects stateObj in stateObjectsList)
        {
            if (!stateObjectsDict.ContainsKey(stateObj.state))
            {
                stateObjectsDict.Add(stateObj.state, stateObj.objects);
            }
        }
    }
    
    private void Start()
    {
        // Установка начального состояния
        SetState(PlantState.Empty);
        
        if (plantBedUI != null)
        {
            plantBedUI.SetActive(false);
        }
        
        // Настройка кнопок
        if (plantButton != null)
        {
            plantButton.onClick.AddListener(PlantCrop);
        }
        
        if (harvestButton != null)
        {
            harvestButton.onClick.AddListener(HarvestCrop);
        }
        
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(UpgradeBed);
        }
        
        // Установка названия продукта
        if (productNameText != null && plantSettings != null)
        {
            productNameText.text = plantSettings.productName;
        }
        
        UpdateUI();
    }
    
    private void OnMouseDown()
    {
        if(EventSystem.current.IsPointerOverGameObject())
            return;
        if (plantBedUI != null)
        {
            if (!plantBedUI.activeSelf)
            {
                CloseAllOtherBedUIs();
            }
            
            plantBedUI.SetActive(!plantBedUI.activeSelf);
            
            if (plantBedUI.activeSelf)
            {
                UpdateUI();
            }
        }
    }
    
    private void CloseAllOtherBedUIs()
    {
        Plant[] allBeds = FindObjectsOfType<Plant>();
        foreach (Plant bed in allBeds)
        {
            if (bed != this && bed.plantBedUI != null)
            {
                bed.plantBedUI.SetActive(false);
            }
        }
    }
    
    private void UpdateUI()
    {
        if (plantSettings == null) return;
        
        if (plantButton != null)
        {
            plantButton.gameObject.SetActive(currentState == PlantState.Empty);
        }
        
        if (harvestButton != null)
        {
            harvestButton.gameObject.SetActive(currentState == PlantState.Ready);
        }
        
        if (upgradeButton != null)
        {
            upgradeButton.gameObject.SetActive(true);
            upgradeButton.interactable = upgradeCount < plantSettings.maxUpgrades;
        }
        
        if (upgradeCountText != null)
        {
            upgradeCountText.text = $"Улучшения: {upgradeCount}/{plantSettings.maxUpgrades}";
        }
        
        if (timerText != null)
        {
            timerText.gameObject.SetActive(currentState == PlantState.Growing);
            
            if (currentState == PlantState.Growing)
            {
                int secondsLeft = Mathf.CeilToInt(currentGrowthTime - growthTimer);
                timerText.text = $"Осталось: {secondsLeft} сек.";
            }
        }
    }
    
    public void PlantCrop()
    {
        if (currentState != PlantState.Empty || plantSettings == null)
            return;
        
        // Меняем состояние на "растущее"
        SetState(PlantState.Growing);
        
        growthTimer = 0f;
        
        if (growthCoroutine != null)
        {
            StopCoroutine(growthCoroutine);
        }
        
        growthCoroutine = StartCoroutine(GrowthProcess());
        
        UpdateUI();
    }
    
    public void HarvestCrop()
    {
        if (currentState != PlantState.Ready || plantSettings == null)
            return;
        
        // Возвращаем состояние в "пустое"
        SetState(PlantState.Empty);
        
        growthTimer = 0f;
        
        // Добавляем ресурс в GameManager
        if (GameManager.Instance != null && plantSettings.resourceType != ResourceType.None)
        {
            GameManager.Instance.AddResource(plantSettings.resourceType, 1);
        }
        
        UpdateUI();
    }
    
    public void UpgradeBed()
    {
        if (plantSettings == null || upgradeCount >= plantSettings.maxUpgrades
            || !CoinManager.instance.SubtractCoins(plantSettings.costUpgrade))
            return;
        
        upgradeCount++;
        currentGrowthTime = Mathf.Max(plantSettings.minGrowthTime, 
                                     plantSettings.initialGrowthTime - 
                                     (upgradeCount * plantSettings.timeReductionPerUpgrade));
        
        UpdateUI();
    }
    
    private IEnumerator GrowthProcess()
    {
        while (growthTimer < currentGrowthTime)
        {
            growthTimer += Time.deltaTime;
            
            // Обновляем UI таймера
            if (timerText != null && currentState == PlantState.Growing)
            {
                int secondsLeft = Mathf.CeilToInt(currentGrowthTime - growthTimer);
                int minutes = secondsLeft / 60;
                int seconds = secondsLeft % 60;
                timerText.text = string.Format("{0:00} {1:00}", minutes, seconds);
            }
            
            yield return null;
        }
        
        // Растение выросло - меняем состояние на "готовое"
        SetState(PlantState.Ready);
        
        UpdateUI();
    }
    
    private void SetState(PlantState newState)
    {
        currentState = newState;
        
        // Деактивируем все объекты для всех состояний
        foreach (var kvp in stateObjectsDict)
        {
            if (kvp.Value != null)
            {
                foreach (GameObject obj in kvp.Value)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }
        
        // Активируем объекты для текущего состояния
        if (stateObjectsDict.TryGetValue(currentState, out GameObject[] stateObjects))
        {
            foreach (GameObject obj in stateObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
}