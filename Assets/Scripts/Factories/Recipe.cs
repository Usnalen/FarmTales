using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public string recipeName;
    public ResourceType resultType;
    public Sprite resultIcon;
    public Dictionary<ResourceType, int> ingredients = new Dictionary<ResourceType, int>();

    // Конструктор для программного создания рецептов
    public Recipe(string name, ResourceType result, Dictionary<ResourceType, int> requiredIngredients)
    {
        recipeName = name;
        resultType = result;
        ingredients = requiredIngredients;
    }

    // Проверка, все ли необходимые ингредиенты присутствуют
    public bool MatchesIngredients(Dictionary<ResourceType, int> providedIngredients)
    {
        // Проверяем каждый требуемый ингредиент
        foreach (var ingredient in ingredients)
        {
            // Если ингредиент отсутствует или его количество недостаточно
            if (!providedIngredients.ContainsKey(ingredient.Key) || 
                providedIngredients[ingredient.Key] < ingredient.Value)
            {
                return false;
            }
        }
        
        return true;
    }
}

[System.Serializable]
public class SerializableRecipe
{
    public string recipeName;
    public ResourceType resultType;
    public Sprite resultIcon;
    public List<RecipeIngredient> ingredients = new List<RecipeIngredient>();
    
    [System.Serializable]
    public class RecipeIngredient
    {
        public ResourceType resourceType;
        public int amount;
    }
    
    public Recipe ToRecipe()
    {
        Dictionary<ResourceType, int> ingredientsDict = new Dictionary<ResourceType, int>();
        
        foreach (RecipeIngredient ingredient in ingredients)
        {
            ingredientsDict[ingredient.resourceType] = ingredient.amount;
        }
        
        Recipe recipe = new Recipe(recipeName, resultType, ingredientsDict);
        recipe.resultIcon = resultIcon;
        return recipe;
    }
}
