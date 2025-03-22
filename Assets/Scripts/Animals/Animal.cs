using System.Collections;
using UnityEngine;

public abstract class Animal : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float minChangeDirectionTime = 1.5f;
    [SerializeField] protected float maxChangeDirectionTime = 3f;
    
    protected Vector2 moveDirection;
    protected Bounds penBounds;
    protected float productionTimer;
    protected bool hasProduct = false;
    protected Vector3 startScale = Vector3.one;
    
    public bool HasProduct => hasProduct;
    
    public abstract void ProduceProduct();
    public abstract void ResetProductionTimer();
    
    protected virtual void Start()
    {
        startScale = transform.localScale;
        ChangeDirection();
        StartCoroutine(RandomlyChangeDirection());
        ResetProductionTimer();
    }
    
    protected virtual void Update()
    {
        Move();
        UpdateProductionTimer();
    }
    
    protected void Move()
    {
        Vector2 newPosition = (Vector2)transform.position + moveDirection * moveSpeed * Time.deltaTime;
        
        // Проверка границ загона
        if (newPosition.x < penBounds.min.x + 0.5f)
        {
            newPosition.x = penBounds.min.x + 0.5f;
            moveDirection.x = Mathf.Abs(moveDirection.x);
        }
        else if (newPosition.x > penBounds.max.x - 0.5f)
        {
            newPosition.x = penBounds.max.x - 0.5f;
            moveDirection.x = -Mathf.Abs(moveDirection.x);
        }
        
        if (newPosition.y < penBounds.min.y + 0.5f)
        {
            newPosition.y = penBounds.min.y + 0.5f;
            moveDirection.y = Mathf.Abs(moveDirection.y);
        }
        else if (newPosition.y > penBounds.max.y - 0.5f)
        {
            newPosition.y = penBounds.max.y - 0.5f;
            moveDirection.y = -Mathf.Abs(moveDirection.y);
        }
        
        transform.position = newPosition;
        
        // Отражение спрайта по направлению движения
        if (moveDirection.x < 0)
            transform.localScale =  new Vector3(-startScale.x, startScale.y, startScale.z);
        else if (moveDirection.x > 0)
            transform.localScale = startScale;
    }
    
    protected void ChangeDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
    }
    
    protected IEnumerator RandomlyChangeDirection()
    {
        while (true)
        {
            float waitTime = Random.Range(minChangeDirectionTime, maxChangeDirectionTime);
            yield return new WaitForSeconds(waitTime);
            ChangeDirection();
        }
    }
    
    protected virtual void UpdateProductionTimer()
    {
        if (!hasProduct)
        {
            productionTimer -= Time.deltaTime;
            if (productionTimer <= 0)
            {
                ProduceProduct();
            }
        }
    }
    
    public void SetPenBounds(Bounds bounds)
    {
        penBounds = bounds;
    }
    
    public virtual void CollectProduct()
    {
        hasProduct = false;
        ResetProductionTimer();
    }
}
