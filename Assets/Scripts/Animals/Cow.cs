using UnityEngine;

public class Cow : Animal
{
    [SerializeField] private GameObject milkPrefab;
    [SerializeField] private float milkProductionTime = 35f; // Время производства молока
    [SerializeField] private Transform milkSpawnPoint;
    
    private GameObject currentMilk;
    
    public override void ProduceProduct()
    {
        hasProduct = true;
        
        if (milkPrefab != null && currentMilk == null)
        {
            Vector3 spawnPosition = (milkSpawnPoint != null) 
                ? milkSpawnPoint.position 
                : transform.position + new Vector3(0, 0.7f, 0);
                
            currentMilk = Instantiate(milkPrefab, spawnPosition, Quaternion.identity, transform.parent);
        }
    }
    
    public override void ResetProductionTimer()
    {
        productionTimer = milkProductionTime + Random.Range(-5f, 5f); // Небольшая случайность
    }
    
    public override void CollectProduct()
    {
        if (currentMilk != null)
        {
            Destroy(currentMilk);
            currentMilk = null;
        }
        
        base.CollectProduct();
    }
}