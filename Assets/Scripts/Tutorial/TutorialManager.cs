using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string title;
        [TextArea(3, 10)]
        public string content;
        public Transform targetPosition; // Точка привязки в мировом пространстве
        public Vector3 offset = Vector3.zero; // Смещение относительно точки привязки
        public float canvasScale = 1f; // Масштаб canvas для каждого шага
    }

    [Header("Настройки обучения")]
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    [SerializeField] private bool showTutorialOnStart = true;
    [SerializeField] private string playerPrefsKey = "TutorialCompleted";
    [SerializeField] private bool debugMode;

    [Header("Элементы интерфейса")]
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private Button okButton;
    [SerializeField] private Button skipButton;
    
    [Header("Настройки анимации")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private int currentStepIndex = 0;
    private static TutorialManager _instance;
    private Coroutine moveCoroutine;
    
    public static TutorialManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (okButton != null)
        {
            okButton.onClick.AddListener(NextStep);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);
        }

        bool tutorialCompleted = PlayerPrefs.GetInt(playerPrefsKey, 0) == 1;

        if (debugMode || showTutorialOnStart && !tutorialCompleted)
        {
            StartTutorial();
        }
        else
        {
            HideTutorialPanel();
        }
    }

    public void StartTutorial()
    {
        currentStepIndex = 0;
        ShowCurrentStep();
    }

    public void NextStep()
    {
        currentStepIndex++;

        if (currentStepIndex < tutorialSteps.Count)
        {
            ShowCurrentStep();
        }
        else
        {
            CompleteTutorial();
        }
    }

    private void ShowCurrentStep()
    {
        if (currentStepIndex < 0 || currentStepIndex >= tutorialSteps.Count)
        {
            CompleteTutorial();
            return;
        }

        TutorialStep step = tutorialSteps[currentStepIndex];

        // Обновляем текст
        if (titleText != null)
        {
            titleText.text = step.title;
        }

        if (contentText != null)
        {
            contentText.text = step.content;
        }

        // Показываем панель
        ShowTutorialPanel();
        
        // Перемещаем Canvas к позиции
        MovePanelToTargetPosition(step);
    }
    
    private void MovePanelToTargetPosition(TutorialStep step)
    {
        if (tutorialCanvas == null || step.targetPosition == null)
            return;
            
        // Останавливаем предыдущую анимацию
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        
        // Запускаем новую анимацию
        moveCoroutine = StartCoroutine(MovePanelToPosition(step));
    }
    
    private IEnumerator MovePanelToPosition(TutorialStep step)
    {
        Vector3 startPosition = tutorialCanvas.transform.position;
        Vector3 targetPosition = step.targetPosition.position + step.offset;
        Vector3 startScale = tutorialCanvas.transform.localScale;
        Vector3 targetScale = Vector3.one * step.canvasScale;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            float curveValue = moveCurve.Evaluate(t);
            
            tutorialCanvas.transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);
            tutorialCanvas.transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
            
            yield return null;
        }
        
        // Устанавливаем точную финальную позицию
        tutorialCanvas.transform.position = targetPosition;
        tutorialCanvas.transform.localScale = targetScale;
        
        moveCoroutine = null;
    }

    private void ShowTutorialPanel()
    {
        if (tutorialCanvas != null)
        {
            tutorialCanvas.SetActive(true);
        }
    }

    private void HideTutorialPanel()
    {
        if (tutorialCanvas != null)
        {
            tutorialCanvas.SetActive(false);
        }
    }

    private void CompleteTutorial()
    {
        HideTutorialPanel();
        PlayerPrefs.SetInt(playerPrefsKey, 1);
        PlayerPrefs.Save();
    }

    public void SkipTutorial()
    {
        CompleteTutorial();
    }

    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey(playerPrefsKey);
    }
}
