using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private float dragAlpha = 0.7f;
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private ResourceType resourceType;
    private int count;
    private Vector2 originalPosition;
    private Camera worldCamera;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Находим камеру для World Canvas
        worldCamera = Camera.main;
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.WorldSpace)
        {
            worldCamera = parentCanvas.worldCamera != null ? parentCanvas.worldCamera : worldCamera;
        }
    }
    
    public void SetItem(Sprite icon, int itemCount, ResourceType type)
    {
        resourceType = type;
        count = itemCount;
        
        if (iconImage != null && icon != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = true;
        }
        
        if (countText != null)
        {
            countText.text = itemCount > 1 ? itemCount.ToString() : "";
            countText.gameObject.SetActive(itemCount > 1);
        }
    }
    
    public ResourceType GetResourceType()
    {
        return resourceType;
    }
    
    public int GetCount()
    {
        return count;
    }
    
    public Sprite GetIconSprite()
    {
        return iconImage != null ? iconImage.sprite : null;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Сохраняем позицию перед перетаскиванием
        originalPosition = rectTransform.anchoredPosition;
        
        canvasGroup.alpha = dragAlpha;
        canvasGroup.blocksRaycasts = false;
        
        transform.SetAsLastSibling();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (worldCamera == null)
            return;
            
        // Если это World Canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            // Преобразуем экранные координаты в мировые
            Vector3 worldPos = worldCamera.ScreenToWorldPoint(new Vector3(
                eventData.position.x, 
                eventData.position.y, 
                transform.position.z - worldCamera.transform.position.z));
                
            transform.position = worldPos;
        }
        else
        {
            // Для Overlay или Camera Space Canvas
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, 
                eventData.position, 
                canvas.worldCamera, 
                out mousePos);
                
            rectTransform.anchoredPosition = mousePos;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        // Возвращаем в исходную позицию
        rectTransform.anchoredPosition = originalPosition;
    }
}
