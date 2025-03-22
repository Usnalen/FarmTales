using UnityEngine;

public class Chicken : Animal
{
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private float eggProductionTime = 30f; // Время производства яйца
    [SerializeField] private Transform eggSpawnPoint;
    
    private GameObject currentEgg;
    
    public override void ProduceProduct()
    {
        hasProduct = true;
        
        // Создаем яйцо над курицей
        if (eggPrefab != null && currentEgg == null)
        {
            Vector3 spawnPosition = (eggSpawnPoint != null) 
                ? eggSpawnPoint.position 
                : transform.position + new Vector3(0, 0.3f, 0);
                
            currentEgg = Instantiate(eggPrefab, spawnPosition, Quaternion.identity, transform.parent);
        }
    }
    
    public override void ResetProductionTimer()
    {
        productionTimer = eggProductionTime + Random.Range(-5f, 5f); // Небольшая случайность
    }
    
    public override void CollectProduct()
    {
        if (currentEgg != null)
        {
            Destroy(currentEgg);
            currentEgg = null;
        }
        
        base.CollectProduct();
    }
}