using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldController : MonoBehaviour
{
    public Sprite emptyFieldSprite; // Спрайт пустой грядки
    public Sprite saplingFieldSprite; // Спрайт с маленькими саженцами
    public Sprite grownFieldSprite; // Спрайт с полностью выросшей пшеницей
    public Button plantButton; // Кнопка "Посадить"
    public Button harvestButton; // Кнопка "Собрать урожай"
    public Button upgradeButton; // Кнопка "Улучшение"
    public TMP_Text growthTimerText; // Текст для отображения таймера
    public Image growthProgressBar; // Полоска прогресса

    private bool isPlanted = false;
    private float growthTime = 40f; // Начальное время роста в секундах
    private float growthTimer = 0f;

    private int currentUpgradeLevel = 0; // Текущий уровень улучшения
    private int[] upgradeCosts = { 50, 100, 200 }; // Стоимость улучшений
    private float[] upgradeTimes = { 30f, 15f, 5f }; // Время роста для каждого уровня улучшения

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = emptyFieldSprite;
        plantButton.gameObject.SetActive(false);
        harvestButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);
        growthTimerText.gameObject.SetActive(false); // Скрываем текст таймера в начале
        growthProgressBar.gameObject.SetActive(false); // Скрываем полоску прогресса в начале
    }

    private void OnMouseDown()
    {
        if (!isPlanted)
        {
            ShowPlantingOptions();
        }
        else
        {
            ShowHarvestingOptions();
        }
    }

    private void ShowPlantingOptions()
    {
        plantButton.gameObject.SetActive(true);
        upgradeButton.gameObject.SetActive(true);
    }

    private void ShowHarvestingOptions()
    {
        harvestButton.gameObject.SetActive(true);
        upgradeButton.gameObject.SetActive(true);
    }

    public void PlantCrop()
    {
        isPlanted = true;
        GetComponent<SpriteRenderer>().sprite = saplingFieldSprite;
        StartCoroutine(GrowthTimer());
        plantButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);
        growthTimerText.gameObject.SetActive(true); // Показываем текст таймера
        growthProgressBar.gameObject.SetActive(true); // Показываем полоску прогресса
    }

    private IEnumerator GrowthTimer()
    {
        while (growthTimer < growthTime)
        {
            growthTimer += Time.deltaTime;
            UpdateGrowthTimerText(); // Обновляем текст таймера
            UpdateGrowthProgressBar(); // Обновляем полоску прогресса
            yield return null;
        }

        GetComponent<SpriteRenderer>().sprite = grownFieldSprite;
        ShowHarvestingOptions(); // Показываем кнопку сбора урожая
        growthTimerText.gameObject.SetActive(false); // Скрываем текст таймера после завершения
        growthProgressBar.gameObject.SetActive(false); // Скрываем полоску прогресса после завершения
    }

    private void UpdateGrowthTimerText()
    {
        float remainingTime = growthTime - growthTimer;
        int seconds = Mathf.CeilToInt(remainingTime);
        growthTimerText.text = "Время до сбора: " + seconds + " секунд"; // Обновляем текст с оставшимся временем
    }

    private void UpdateGrowthProgressBar()
    {
        growthProgressBar.fillAmount = growthTimer / growthTime; // Обновляем заполнение полоски
    }

    public void HarvestCrop()
    {
        if (isPlanted && growthTimer >= growthTime) // Проверяем, созрел ли урожай
        {
            isPlanted = false;
            growthTimer = 0f;
            GetComponent<SpriteRenderer>().sprite = emptyFieldSprite;

            harvestButton.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(false);
            
            // Добавляем монеты после сбора урожая
            CoinManager coinManager = FindObjectOfType<CoinManager>();
            if (coinManager != null)
            {
                coinManager.AddCoins(10); // Например, за сбор урожая дается 10 монет
            }
        }
    }

    public void UpgradeField()
    {
        if (currentUpgradeLevel < upgradeCosts.Length)
        {
            CoinManager coinManager = FindObjectOfType<CoinManager>();
            if (coinManager != null && coinManager.SubtractCoins(upgradeCosts[currentUpgradeLevel]))
            {
                currentUpgradeLevel++;
                UpdateGrowthTime();
                Debug.Log("Грядка улучшена! Текущий уровень: " + currentUpgradeLevel);
            }
            else
            {
                Debug.LogWarning("Недостаточно монет для улучшения!");
            }
        }
        else
        {
            Debug.Log("Максимальный уровень улучшения достигнут.");
        }
    }

    private void UpdateGrowthTime()
    {
        if (currentUpgradeLevel < upgradeTimes.Length)
        {
            growthTime = upgradeTimes[currentUpgradeLevel];
            Debug.Log("Новое время роста: " + growthTime + " секунд");
        }
    }
}