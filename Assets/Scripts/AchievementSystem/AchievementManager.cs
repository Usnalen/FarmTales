using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementManager : MonoBehaviour
{
    [Header("Achievement Settings")]
    [SerializeField] private List<Achievement> achievements = new List<Achievement>();
    
    [Header("UI Elements")]
    [SerializeField] private GameObject achievementUI;
    [SerializeField] private Transform achievementContainer;
    [SerializeField] private GameObject achievementPrefab;
    [SerializeField] private GameObject achievementNotification;
    [SerializeField] private TextMeshProUGUI notificationText;
    
    // Словари для отслеживания прогресса
    private Dictionary<ResourceType, int> producedItems = new Dictionary<ResourceType, int>();
    private int totalMilkCollected = 0;
    private int totalEggsCollected = 0;
    private int totalConfectionerySold = 0;
    private int totalBreadBaked = 0;
    
    // Ссылка на синглтон
    private static AchievementManager _instance;
    public static AchievementManager Instance => _instance;
    
    private Dictionary<AchievementType, bool> wasNotificationShown = new Dictionary<AchievementType, bool>();
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeNotificationStatus();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Подписка на события GameManager для отслеживания ресурсов
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnResourceChanged += OnResourceChanged;
        }
        
        if (achievementUI != null)
        {
            achievementUI.SetActive(false);
        }
        
        if (achievementNotification != null)
        {
            achievementNotification.SetActive(false);
        }
    }
    
    private void InitializeNotificationStatus()
    {
        foreach (AchievementType type in System.Enum.GetValues(typeof(AchievementType)))
        {
            wasNotificationShown[type] = false;
        }
    }
    
    
    // Обработка изменения ресурсов
    private void OnResourceChanged(ResourceType resourceType, int newCount)
    {
        // Отслеживаем различные типы ресурсов
        switch (resourceType)
        {
            case ResourceType.Milk:
                UpdateMilkAchievement();
                break;
                
            case ResourceType.Eggs:
                UpdateEggAchievement();
                break;
                
            case ResourceType.Bread:
                UpdateBreadAchievement();
                break;
                
            default:
                UpdateProductionAchievement(resourceType);
                break;
        }
    }
    
    // Методы для отслеживания прогресса достижений
    
    public void AddProducedItem(ResourceType type, int amount)
    {
        if (!producedItems.ContainsKey(type))
        {
            producedItems[type] = amount;
        }
        else
        {
            producedItems[type] += amount;
        }
        
        UpdateProductionAchievement(type);
    }
    
    public void AddMilkCollected(int amount)
    {
        totalMilkCollected += amount;
        UpdateMilkAchievement();
    }
    
    public void AddEggsCollected(int amount)
    {
        totalEggsCollected += amount;
        UpdateEggAchievement();
    }
    
    public void AddConfectionerySold(ResourceType type, int amount)
    {
        if (IsConfectionery(type))
        {
            totalConfectionerySold += amount;
            UpdateMarketSellerAchievement();
        }
    }
    
    public void AddBreadBaked(int amount)
    {
        totalBreadBaked += amount;
        UpdateBreadAchievement();
    }
    
    // Методы обновления достижений
    
    private void UpdateProductionAchievement(ResourceType type)
    {
        Achievement achievement = GetAchievement(AchievementType.ProductionMaster);
        if (achievement != null)
        {
            int uniqueProducts = producedItems.Count;
            achievement.UpdateProgress(uniqueProducts);
            
            CheckAndShowNotification(achievement);
        }
    }
    
    private void UpdateMilkAchievement()
    {
        Achievement achievement = GetAchievement(AchievementType.MilkMagnate);
        if (achievement != null)
        {
            achievement.UpdateProgress(totalMilkCollected);
            
            CheckAndShowNotification(achievement);
        }
    }
    
    private void UpdateEggAchievement()
    {
        Achievement achievement = GetAchievement(AchievementType.EggCollector);
        if (achievement != null)
        {
            achievement.UpdateProgress(totalEggsCollected);
            
            CheckAndShowNotification(achievement);
        }
    }
    
    private void UpdateMarketSellerAchievement()
    {
        Achievement achievement = GetAchievement(AchievementType.MarketSeller);
        if (achievement != null)
        {
            achievement.UpdateProgress(totalConfectionerySold);
            
            CheckAndShowNotification(achievement);
        }
    }
    
    private void UpdateBreadAchievement()
    {
        Achievement achievement = GetAchievement(AchievementType.BakerProfessional);
        if (achievement != null)
        {
            achievement.UpdateProgress(totalBreadBaked);
            
            CheckAndShowNotification(achievement);
        }
    }
    
    private Achievement GetAchievement(AchievementType type)
    {
        return achievements.Find(a => a.type == type);
    }
    
    private void CheckAndShowNotification(Achievement achievement)
    {
        if (achievement.isCompleted && !wasNotificationShown[achievement.type])
        {
            ShowAchievementNotification(achievement);
            wasNotificationShown[achievement.type] = true;
        }
    }
    
    // Проверка типа продукта
    private bool IsConfectionery(ResourceType type)
    {
        return type == ResourceType.Cake || 
               type == ResourceType.AppleCake || 
               type == ResourceType.RaspberryCake || 
               type == ResourceType.StrawberryCake || 
               type == ResourceType.Сupcake;
    }
    
    // Показать уведомление о достижении
    private void ShowAchievementNotification(Achievement achievement)
    {
        if (achievementNotification != null && notificationText != null)
        {
            notificationText.text = $"Достижение получено: {achievement.title}\nНаграда: {achievement.rewardCoins} монет";
            achievementNotification.SetActive(true);
            Invoke("HideNotification", 3f);
        }
    }
    
    private void HideNotification()
    {
        if (achievementNotification != null)
        {
            achievementNotification.SetActive(false);
        }
    }
    
    // Открыть/закрыть UI достижений
    public void ToggleAchievementUI()
    {
        if (achievementUI != null)
        {
            achievementUI.SetActive(!achievementUI.activeSelf);
            
            if (achievementUI.activeSelf)
            {
                UpdateAchievementUI();
            }
        }
    }
    
    // Обновить UI достижений
    private void UpdateAchievementUI()
    {
        if (achievementContainer == null || achievementPrefab == null)
            return;
            
        foreach (Transform child in achievementContainer)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Achievement achievement in achievements)
        {
            GameObject achievementObj = Instantiate(achievementPrefab, achievementContainer);
            AchievementDisplay display = achievementObj.GetComponent<AchievementDisplay>();
            
            if (display != null)
            {
                display.SetAchievement(achievement);
            }
        }
    }

    private void OnEnable()
    {
        LoadAchievements();
    }

    private void OnDisable()
    {
        SaveAchievements();
    }

    public void SaveAchievements()
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            Achievement achievement = achievements[i];
            PlayerPrefs.SetInt($"Achievement_{achievement.type}_Current", achievement.currentValue);
            PlayerPrefs.SetInt($"Achievement_{achievement.type}_Completed", achievement.isCompleted ? 1 : 0);
        }
    
        PlayerPrefs.Save();
    }

    public void LoadAchievements()
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            Achievement achievement = achievements[i];
            if (PlayerPrefs.HasKey($"Achievement_{achievement.type}_Current"))
            {
                achievement.currentValue = PlayerPrefs.GetInt($"Achievement_{achievement.type}_Current");
                achievement.isCompleted = PlayerPrefs.GetInt($"Achievement_{achievement.type}_Completed") == 1;
            }
        }
    }
}
