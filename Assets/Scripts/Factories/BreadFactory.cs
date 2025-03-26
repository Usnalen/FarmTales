using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BreadFactory : Factory
{
    protected override List<ResourceType> GetAllowedStorageResourceTypes()
    {
        // Хлебная фабрика использует только пшеницу и яйца
        return new List<ResourceType> { ResourceType.Wheat, ResourceType.Eggs };
    }
}