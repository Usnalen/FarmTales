using System.Collections.Generic;
using UnityEngine;

public class MultiFormulaRow : BaseFormulaRow
{
    [Header("Варианты рецептов")]
    [SerializeField] private List<SerializableRecipe> variantRecipes = new List<SerializableRecipe>();
    
    protected List<Recipe> recipes = new List<Recipe>();
    protected Recipe currentDisplayedRecipe; // Для отображения в UI
    
    protected override void InitializeRecipe()
    {
        recipes.Clear();
        
        foreach (var variantRecipe in variantRecipes)
        {
            recipes.Add(variantRecipe.ToRecipe());
        }
        
        // Выбираем первый рецепт для отображения
        if (recipes.Count > 0)
        {
            currentDisplayedRecipe = recipes[0];
            
            if (equationText != null)
            {
                UpdateFormulaText();
            }
        }
    }
    
    protected override void UpdateFormulaText()
    {
        if (currentDisplayedRecipe == null || equationText == null) return;
        
        string formula = "";
        foreach (var ingredient in currentDisplayedRecipe.ingredients)
        {
            if (formula.Length > 0) formula += " + ";
            formula += $"{ingredient.Value} {ingredient.Key}";
        }
        
        formula += " = " + currentDisplayedRecipe.recipeName;
        
        // Если есть несколько рецептов, показываем это
        if (recipes.Count > 1)
        {
            formula += $" (1/{recipes.Count})";
        }
        
        equationText.text = formula;
    }
    
    public override void OnCreateButtonClicked()
    {
        if (recipes.Count == 0) return;
        
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

        // Проверяем каждый рецепт на соответствие
        foreach (var recipe in recipes)
        {
            if (recipe.MatchesIngredients(providedIngredients))
            {
                // Расходуем ингредиенты
                ConsumeIngredients(recipe);
                // Создаем результат
                CreateResult(recipe);
                return; // Выходим после первого подходящего рецепта
            }
        }
        
        Debug.Log("Недостаточно ингредиентов для создания продукта");
    }
    
    protected void ConsumeIngredients(Recipe recipe)
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
    
    protected void CreateResult(Recipe recipe)
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
    
    // Метод для циклического переключения между рецептами (можно привязать к кнопке)
    public void CycleDisplayedRecipe()
    {
        if (recipes.Count <= 1) return;
        
        int currentIndex = recipes.IndexOf(currentDisplayedRecipe);
        int nextIndex = (currentIndex + 1) % recipes.Count;
        
        currentDisplayedRecipe = recipes[nextIndex];
        UpdateFormulaText();
    }
    
    public List<Recipe> GetRecipes()
    {
        return recipes;
    }
    
    public Recipe GetCurrentRecipe()
    {
        return currentDisplayedRecipe;
    }
}
