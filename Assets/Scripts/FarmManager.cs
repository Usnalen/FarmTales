using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class FarmManager : MonoBehaviour
{
    public GameObject chickenPrefab; // Префаб курицы
    public Transform chickenArea; // Область, где могут находиться курицы
    public CoinManager coinManager; // Ссылка на CoinManager
    public GameObject upgradeButton; // Кнопка улучшения
    public GameObject collectButton; // Кнопка сбора яиц
    public GameObject startCollectingButton; // Кнопка для начала сбора яиц

    public GameObject addOneChickenButton;
    public GameObject addTwoChickensButton;
    public GameObject addFiveChickensButton;

    public Text insufficientCoinsMessage;

    public EggCollectionTimer eggCollectionTimer; // Ссылка на EggCollectionTimer

    private int chickenCount = 2; // Начальное количество куриц

    void Start()
    {
        SpawnChickens();
        InitializeUI(); // Инициализируем интерфейс
        HideUpgradeOptions(); // Скрываем кнопки добавления куриц при старте
        insufficientCoinsMessage.gameObject.SetActive(false); // Скрываем сообщение о нехватке монет
    }

    private void InitializeUI()
    {
        collectButton.SetActive(false); // Скрываем кнопку сбора яиц
        startCollectingButton.SetActive(false); // Скрываем кнопку начала сбора яиц
        upgradeButton.SetActive(true); // Кнопка "Улучшить" видима изначально
    }

    private void SpawnChickens()
    {
        if (chickenPrefab == null)
        {
            Debug.LogError("chickenPrefab не назначен! Пожалуйста, назначьте его в инспекторе.");
            return;
        }

        if (chickenArea == null)
        {
            Debug.LogError("chickenArea не назначен! Пожалуйста, назначьте его в инспекторе.");
            return;
        }

        for (int i = 0; i < chickenCount; i++)
        {
            Instantiate(chickenPrefab, chickenArea.position, Quaternion.identity);
        }
    }

    public void ShowUpgradeOptions()
    {
        startCollectingButton.SetActive(true); // Показываем кнопку "Начать сбор яиц"
        upgradeButton.SetActive(true);
        addOneChickenButton.SetActive(true);
        addTwoChickensButton.SetActive(true);
        addFiveChickensButton.SetActive(true);
        collectButton.SetActive(false); // Скрываем кнопку "Собрать"
    }

    public void HideUpgradeOptions()
    {
        addOneChickenButton.SetActive(false);
        addTwoChickensButton.SetActive(false);
        addFiveChickensButton.SetActive(false);
        collectButton.SetActive(false); // Скрываем кнопку "Собрать"
        startCollectingButton.SetActive(false); // Скрываем кнопку "Начать сбор яиц"
        upgradeButton.SetActive(true);
        insufficientCoinsMessage.gameObject.SetActive(false); // Скрываем сообщение о нехватке монет
    }

    public void AddChickens(int amount)
    {
        int cost = GetCost(amount);
        
        if (!coinManager.HasEnoughCoins(cost))
        {
            insufficientCoinsMessage.gameObject.SetActive(true); // Показываем сообщение о нехватке монет
            HideUpgradeOptions(); // Скрываем кнопки
            return; // Выходим из метода
        }

        if (coinManager.SubtractCoins(cost))
        {
            chickenCount += amount; // Увеличиваем количество куриц
            SpawnChickens(); // Спавним новых куриц
            HideUpgradeOptions(); // Скрываем кнопки после успешной покупки
        }
    }

    private int GetCost(int amount)
    {
        switch (amount)
        {
            case 1: return 20;
            case 2: return 60;
            case 5: return 100;
            default: return 0;
        }
    }

    public void StartEggCollection()
    {
        eggCollectionTimer.StartTimer(); // Запускаем таймер
        collectButton.SetActive(false); // Скрываем кнопку сбора яиц
        startCollectingButton.SetActive(false); // Скрываем кнопку "Начать сбор яиц"
    }

    public void EnableCollectButton()
    {
        collectButton.SetActive(true); // Активируем кнопку сбора яиц
    }

    public void CollectEggs()
    {
        int eggsCollected = chickenCount; // Каждая курица дает одно яйцо
        coinManager.AddCoins(eggsCollected); // Добавляем монеты за яйца
    }

    public void OnStartCollectingButtonClicked()
    {
        StartEggCollection(); // Запускаем сбор яиц
    }

    public void OnAddOneChickenButtonClicked() => AddChickens(1);
    
    public void OnAddTwoChickensButtonClicked() => AddChickens(2);
    
    public void OnAddFiveChickensButtonClicked() => AddChickens(5);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FenceZone")) // Убедитесь, что у зоны забора установлен тег "FenceZone"
        {
            ShowUpgradeOptions(); // Показываем кнопки при входе в зону забора
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FenceZone")) // Убедитесь, что у зоны забора установлен тег "FenceZone"
        {
            HideUpgradeOptions(); // Скрываем кнопки при выходе из зоны забора
        }
    }
    public void ShowInsufficientCoinsMessage()
    {
        insufficientCoinsMessage.gameObject.SetActive(true);
        StartCoroutine(HideInsufficientCoinsMessage());
    }

    private IEnumerator HideInsufficientCoinsMessage()
    {
        yield return new WaitForSeconds(2f); // Скрыть через 2 секунды
        insufficientCoinsMessage.gameObject.SetActive(false);
    }
}