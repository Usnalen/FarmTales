using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class IngredientSlot : MonoBehaviour, IDropHandler
{
    [Header("UI элементы")] 
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Button slotButton;

    [Header("Настройки")] 
    [SerializeField] private int maxStackSize = 10;

    private ResourceType currentResourceType = ResourceType.None;
    private int count = 0;
    private Factory parentFactory;

    public delegate void SlotChangedDelegate(IngredientSlot slot);
    public event SlotChangedDelegate OnSlotChanged;

    private void Awake()
    {
        parentFactory = GetComponentInParent<Factory>();
    }

    private void Start()
    {
        if (slotButton != null)
        {
            slotButton.onClick.AddListener(OnSlotClicked);
        }

        UpdateVisual();
    }

    // Обработчик нажатия на слот
    private void OnSlotClicked()
    {
        if (currentResourceType != ResourceType.None && count > 0 && GameManager.Instance != null)
        {
            GameManager.Instance.AddResource(currentResourceType, count);
            Debug.Log($"Возвращено в хранилище: {count} x {currentResourceType}");
            Clear();
        }
    }

    // Обработчик события перетаскивания ресурса в слот
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop вызван на слоте: " + gameObject.name);

        if (parentFactory == null)
        {
            Debug.LogError("Не найдена родительская фабрика");
            return;
        }

        DraggableItem draggableItem = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (draggableItem == null)
        {
            Debug.LogError("Не найден компонент DraggableItem");
            return;
        }

        ResourceType droppedType = draggableItem.GetResourceType();
        Debug.Log($"Тип ресурса для добавления: {droppedType}");

        if (parentFactory.IsValidIngredientForSlot(this, droppedType))
        {
            if (currentResourceType == ResourceType.None || currentResourceType == droppedType)
            {
                if (count < maxStackSize)
                {
                    if (GameManager.Instance != null && GameManager.Instance.GetResourceCount(droppedType) > 0)
                    {
                        if (GameManager.Instance.UseResource(droppedType, 1))
                        {
                            if (currentResourceType == ResourceType.None)
                            {
                                SetResource(droppedType, 1, draggableItem.GetIconSprite());
                            }
                            else
                            {
                                AddAmount(1);
                            }

                            StartCoroutine(UpdateDisplayWait(droppedType));
                        }
                    }
                    else
                    {
                        Debug.Log("Недостаточно ресурсов в хранилище");
                    }
                }
                else
                {
                    Debug.Log($"Достигнут максимальный размер стека: {maxStackSize}");
                }
            }
            else
            {
                Debug.Log($"В слоте уже есть другой ресурс: {currentResourceType}");
            }
        }
        else
        {
            Debug.Log("Этот тип ресурса не подходит для данного слота");
        }
    }

    // Метод для отложенного обновления отображения
    private IEnumerator UpdateDisplayWait(ResourceType resourceType)
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.UpdateAllActiveFactories(resourceType);
    }

    // Установка ресурса в слот
    public void SetResource(ResourceType type, int amount, Sprite icon)
    {
        currentResourceType = type;
        count = amount;

        if (iconImage != null && icon != null)
        {
            iconImage.sprite = icon;
            iconImage.gameObject.SetActive(true);
        }

        UpdateVisual();

        if (OnSlotChanged != null)
        {
            OnSlotChanged.Invoke(this);
        }
    }

    // Добавление количества ресурса
    public void AddAmount(int amountToAdd)
    {
        if (currentResourceType != ResourceType.None)
        {
            count += amountToAdd;
            UpdateVisual();

            if (OnSlotChanged != null)
            {
                OnSlotChanged.Invoke(this);
            }
        }
    }

    // Уменьшение количества ресурса
    public void RemoveAmount(int amountToRemove)
    {
        if (currentResourceType != ResourceType.None && count >= amountToRemove)
        {
            count -= amountToRemove;

            if (count <= 0)
            {
                Clear();
            }
            else
            {
                UpdateVisual();

                if (OnSlotChanged != null)
                {
                    OnSlotChanged.Invoke(this);
                }
            }
        }
    }

    // Очистка слота
    public void Clear()
    {
        currentResourceType = ResourceType.None;
        count = 0;

        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(false);
        }

        UpdateVisual();

        if (OnSlotChanged != null)
        {
            OnSlotChanged.Invoke(this);
        }
    }

    // Получение типа ресурса в слоте
    public ResourceType GetResourceType()
    {
        return currentResourceType;
    }

    // Получение количества ресурса в слоте
    public int GetCount()
    {
        return count;
    }

    // Обновление визуального отображения слота
    private void UpdateVisual()
    {
        if (countText != null)
        {
            countText.text = count > 0 ? count.ToString() : "";
            countText.gameObject.SetActive(count > 1);
        }

        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(currentResourceType != ResourceType.None && count > 0);
        }
    }
}
