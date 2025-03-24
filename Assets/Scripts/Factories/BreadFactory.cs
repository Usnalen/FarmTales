using System.Collections.Generic;
using UnityEngine;

public class BreadFactory : Factory
{
    [Header("Bread Factory Settings")]
    [SerializeField] private IngredientSlot wheatSlot;
    [SerializeField] private IngredientSlot eggSlot;
    
    [Header("Recipes")]
    [SerializeField] private List<SerializableRecipe> serializableRecipes = new List<SerializableRecipe>();

    protected override void Start()
    {
        base.Start();
        
        if (factoryTitle != null)
        {
            factoryTitle.text = "Хлебная фабрика";
        }
        
        // Конвертация сериализуемых рецептов в рабочие рецепты
        foreach (SerializableRecipe serializableRecipe in serializableRecipes)
        {
            recipes.Add(serializableRecipe.ToRecipe());
        }
        
        // Если нет сериализуемых рецептов, создаем стандартный рецепт программно
        if (recipes.Count == 0)
        {
            Dictionary<ResourceType, int> breadIngredients = new Dictionary<ResourceType, int>
            {
                { ResourceType.Wheat, 1 },
                { ResourceType.Eggs, 1 }
            };
            Recipe breadRecipe = new Recipe("Хлеб", ResourceType.Bread, breadIngredients);
            recipes.Add(breadRecipe);
        }
    }
    
    public override bool IsValidIngredientForSlot(IngredientSlot slot, ResourceType resourceType)
    {
        // Проверяем тип ресурса для конкретного слота
        if (slot == wheatSlot && resourceType == ResourceType.Wheat)
            return true;
        
        if (slot == eggSlot && resourceType == ResourceType.Eggs)
            return true;
        
        return false;
    }
    
    protected override List<ResourceType> GetAllowedStorageResourceTypes()
    {
        // Хлебная фабрика может использовать только пшеницу и яйца
        return new List<ResourceType> { ResourceType.Wheat, ResourceType.Eggs };
    }
    
    protected override Recipe FindMatchingRecipe(Dictionary<ResourceType, int> providedIngredients)
    {
        foreach (Recipe recipe in recipes)
        {
            if (recipe.MatchesIngredients(providedIngredients))
            {
                return recipe;
            }
        }
        
        return null;
    }
}