using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfectioneryFactory : Factory
{ protected override List<ResourceType> GetAllowedStorageResourceTypes()
    {
        // Кондитерская фабрика может использовать все продукты кроме яиц и пшеницы
        return new List<ResourceType> 
        { 
            ResourceType.Milk,
            ResourceType.Bread,
            ResourceType.Butter,
            ResourceType.Cream,
            ResourceType.Apple,
            ResourceType.Raspberry,
            ResourceType.Strawberry
        };
    }
}