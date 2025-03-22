using System.Collections;
using UnityEngine;
using TMPro;

public class EggCollectionTimer : MonoBehaviour
{
    public TMP_Text timerText; // Текстовое поле для отображения таймера
    public FarmManager farmManager; // Ссылка на FarmManager
    private float timerDuration = 20f; // Длительность таймера
    private float timeRemaining;
    private bool isTimerRunning = false;

    void Start()
    {
        timeRemaining = timerDuration;
        timerText.gameObject.SetActive(false); // Скрываем таймер в начале
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timeRemaining).ToString(); // Обновляем текст таймера

            if (timeRemaining <= 0)
            {
                isTimerRunning = false;
                timerText.gameObject.SetActive(false); // Скрываем таймер
                farmManager.EnableCollectButton(); // Активируем кнопку сбора яиц
            }
        }
    }

    public void StartTimer()
    {
        timeRemaining = timerDuration;
        isTimerRunning = true;
        timerText.gameObject.SetActive(true); // Показываем таймер
    }
}
