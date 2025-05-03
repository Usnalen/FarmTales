using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarnCapacityWarning : MonoBehaviour
{
    [Header("UI элементы")]
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private Button okButton;
    
    [Header("Настройки")]
    [SerializeField] private float checkInterval = 1f;
    
    private Barn barn;
    private float lastCheckTime;
    private bool _warningShown = false;
    
    private void Start()
    {
        barn = Barn.Instance;
        
        if (barn == null)
        {
            Debug.LogError("Не найден экземпляр Barn!");
            return;
        }
        
        barn.OnBarnUpdated += CheckBarnCapacity;
        
        if (okButton != null)
        {
            okButton.onClick.AddListener(CloseWarning);
        }
        
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
        
        CheckBarnCapacity();
    }
    
    private void Update()
    {
        if (Time.time - lastCheckTime >= checkInterval)
        {
            lastCheckTime = Time.time;
            CheckBarnCapacity();
        }
    }
    
    private void OnDestroy()
    {
        if (barn != null)
        {
            barn.OnBarnUpdated -= CheckBarnCapacity;
        }
    }
    
    // Проверяем, заполнен ли амбар полностью
    private void CheckBarnCapacity()
    {
        if (barn == null || warningPanel == null)
            return;
        
        bool isFull = !barn.CanAddResources(1);
        
        if (isFull && !_warningShown)
        {
            ShowWarning();
            _warningShown = true;
        }
        else if (!isFull)
        {
            _warningShown = false;
        }
    }
    
    // Показываем предупреждение
    public void ShowWarning()
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
        }
    }
    
    // Закрываем предупреждение
    public void CloseWarning()
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }
}
