using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldController : MonoBehaviour
{
   public Sprite emptyFieldSprite; // Спрайт пустой грядки
    public Sprite saplingFieldSprite; // Спрайт с маленькими саженцами
    public Sprite grownFieldSprite; // Спрайт с полностью выросшей пшеницей
    public Button plantButton; // Кнопка "Посадить"
    public Button harvestButton; // Кнопка "Собрать урожай"
    public Button upgradeButton; // Кнопка "Улучшение"

    private bool isPlanted = false;
    private float growthTime = 5f; // Время роста в секундах
    private float growthTimer = 0f;

    private void Start()
    {
        // Устанавливаем начальный спрайт
        GetComponent<SpriteRenderer>().sprite = emptyFieldSprite;

        // Скрываем кнопки при старте
        plantButton.gameObject.SetActive(false);
        harvestButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);
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

        // Начинаем таймер роста
        StartCoroutine(GrowthTimer());

        // Скрываем кнопки после посадки
        plantButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);
    }

    private IEnumerator GrowthTimer()
    {while (growthTimer < growthTime)
        {
            growthTimer += Time.deltaTime;
            yield return null;
        }

        // После завершения роста заменяем спрайт на полностью выросшую пшеницу
        GetComponent<SpriteRenderer>().sprite = grownFieldSprite; 
        ShowHarvestingOptions(); // Показываем кнопку сбора урожая
    }

    public void HarvestCrop()
    {
        if (isPlanted)
        {
            isPlanted = false;
            growthTimer = 0f;
            GetComponent<SpriteRenderer>().sprite = emptyFieldSprite;

            // Скрываем кнопки после сбора урожая
            harvestButton.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(false);
        }
    }

    public void UpgradeField()
    {
        // Логика улучшения грядки (например, уменьшение времени роста или увеличение урожайности)
        Debug.Log("Грядка улучшена!");
        // Здесь можно добавить логику для улучшения, например, уменьшение времени роста.
    }
}
