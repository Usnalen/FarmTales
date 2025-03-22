using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BarnItemDisplay : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject selectionHighlight;
    
    private ResourceType resourceType;
    private int count;
    
    private void Start()
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(false);
        }
    }
    
    public void SetItem(Sprite icon, int itemCount, ResourceType type)
    {
        if (iconImage != null)
        {
            iconImage.sprite = icon;
        }
        
        if (countText != null)
        {
            countText.text = itemCount.ToString();
        }
        
        resourceType = type;
        count = itemCount;
    }
    
    // Обработка клика по предмету
    public void OnPointerClick(PointerEventData eventData)
    {
        // Можно добавить логику выбора предмета для его использования
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(!selectionHighlight.activeSelf);
        }
    }
    
    // Метод для использования предмета
    public void UseItem(int amount = 1)
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.UseResource(resourceType, amount))
            {
                // Обновляем UI амбара после использования
                if (Barn.Instance != null)
                {
                    Barn.Instance.UpdateBarnUI();
                }
            }
        }
    }
}