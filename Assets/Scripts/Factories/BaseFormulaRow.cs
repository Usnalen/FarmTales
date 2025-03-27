using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class BaseFormulaRow : MonoBehaviour
{
    [Header("UI элементы")]
    [SerializeField] protected List<IngredientSlot> ingredientSlots = new List<IngredientSlot>();
    [SerializeField] protected IngredientSlot resultSlot;
    [SerializeField] protected Button createButton; // Кнопка "="
    [SerializeField] protected TextMeshProUGUI equationText; // Отображение формулы
    
    protected Factory parentFactory;

    protected virtual void Start()
    {
        parentFactory = GetComponentInParent<Factory>();
        
        if (createButton != null)
        {
            createButton.onClick.AddListener(OnCreateButtonClicked);
        }
        
        InitializeRecipe();
    }
    
    // Абстрактные методы для реализации в дочерних классах
    protected abstract void InitializeRecipe();
    protected abstract void UpdateFormulaText();
    public abstract void OnCreateButtonClicked();
    
    public bool ContainsSlot(IngredientSlot slot)
    {
        return ingredientSlots.Contains(slot) || slot == resultSlot;
    }
}