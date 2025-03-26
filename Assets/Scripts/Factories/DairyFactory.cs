using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DairyFactory : Factory
{
    protected override List<ResourceType> GetAllowedStorageResourceTypes()
    {
        // Молочная фабрика использует только молоко
        return new List<ResourceType> { ResourceType.Milk };
    }
}