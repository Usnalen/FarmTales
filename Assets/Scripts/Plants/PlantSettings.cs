using UnityEngine;

[CreateAssetMenu(fileName = "New Plant Settings", menuName = "Farm/Plant Settings", order = 1)]
public class PlantSettings : ScriptableObject
{
    [Header("Основные настройки")]
    public string productName = "Растение";
    public ResourceType resourceType = ResourceType.None;
    public float initialGrowthTime = 40f;
    public int timeReductionPerUpgrade = 5;
    public int maxUpgrades = 7;
    public int minGrowthTime = 5;
}