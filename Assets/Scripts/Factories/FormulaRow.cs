using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FormulaRow : MonoBehaviour
{
    [Header("UI элементы")]
    [SerializeField] private List<IngredientSlot> ingredientSlots = new List<IngredientSlot>();
    [SerializeField] private IngredientSlot resultSlot;
    [SerializeField] private Button createButton; // Кнопка "="
    [SerializeField] private TextMeshProUGUI equationText; // Отображение формулы
    
    [Header("Рецепт")]
    [SerializeField] private SerializableRecipe recipeData;
    
    private Recipe recipe;
    private Factory parentFactory;
    
    private void Awake()
    {
        parentFactory = GetComponentInParent<Factory>();
        
        if (recipeData != null)
        {
            recipe = recipeData.ToRecipe();
        }
    }
    
    private void Start()
    {
        if (createButton != null)
        {
            createButton.onClick.AddListener(OnCreateButtonClicked);
        }
        
        // Настройка текста формулы
        if (equationText != null && recipe != null)
        {
            UpdateFormulaText();
        }
    }
    
    private void UpdateFormulaText()
    {
        string formula = "";
        foreach (var ingredient in recipe.ingredients)
        {
            if (formula.Length > 0) formula += " + ";
            formula += $"{ingredient.Value} {ingredient.Key}";
        }
        
        formula += " = " + recipe.recipeName;
        equationText.text = formula;
    }
    
    public void OnCreateButtonClicked()
    {
        if (recipe == null) return;
        
        Dictionary<ResourceType, int> providedIngredients = new Dictionary<ResourceType, int>();
        
        // Собираем ингредиенты из слотов
        foreach (IngredientSlot slot in ingredientSlots)
        {
            ResourceType type = slot.GetResourceType();
            if (type != ResourceType.None)
            {
                int slotCount = slot.GetCount();
                if (providedIngredients.ContainsKey(type))
                {
                    providedIngredients[type] += slotCount;
                }
                else
                {
                    providedIngredients[type] = slotCount;
                }
            }
        }
        
        // Проверяем соответствие ингредиентов рецепту
        if (recipe.MatchesIngredients(providedIngredients))
        {
            // Расходуем ингредиенты
            ConsumeIngredients();
            
            // Создаем результат
            CreateResult();
        }
        else
        {
            Debug.Log("Недостаточно ингредиентов для создания продукта");
        }
    }
    
    private void ConsumeIngredients()
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int remainingAmount = ingredient.Value;
            
            foreach (IngredientSlot slot in ingredientSlots)
            {
                if (slot.GetResourceType() == ingredient.Key && remainingAmount > 0)
                {
                    int amountInSlot = slot.GetCount();
                    int amountToRemove = Mathf.Min(amountInSlot, remainingAmount);
                    
                    slot.RemoveAmount(amountToRemove);
                    remainingAmount -= amountToRemove;
                    
                    if (remainingAmount <= 0)
                        break;
                }
            }
        }
    }
    
    private void CreateResult()
    {
        if (resultSlot == null) return;
        
        ResourceType currentResultType = resultSlot.GetResourceType();
        
        // Если слот результата содержит такой же тип продукта, стакируем
        if (currentResultType == recipe.resultType)
        {
            resultSlot.AddAmount(1);
        }
        // Если слот результата пуст или содержит другой тип продукта
        else
        {
            // Если в слоте есть что-то, возвращаем в хранилище
            if (currentResultType != ResourceType.None && GameManager.Instance != null)
            {
                GameManager.Instance.AddResource(currentResultType, resultSlot.GetCount());
            }
            
            // Получаем иконку для результата
            Sprite resultIcon = recipe.resultIcon;
            if (resultIcon == null && parentFactory != null)
            {
                resultIcon = parentFactory.GetResourceIcon(recipe.resultType);
            }
            
            // Устанавливаем новый результат
            resultSlot.SetResource(recipe.resultType, 1, resultIcon);
        }
    }
    
    public Recipe GetRecipe()
    {
        return recipe;
    }
    
    public bool ContainsSlot(IngredientSlot slot)
    {
        return ingredientSlots.Contains(slot) || slot == resultSlot;
    }
}
