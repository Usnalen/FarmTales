using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenController : MonoBehaviour
{
    public float moveSpeed = 2f; // Скорость движения курицы
    private Vector3 targetPosition; // Целевая позиция для движения

    // Границы забора
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    void Start()
    {
        // Установка границ на основе позиции и масштаба забора
        Vector3 fencePosition = new Vector3(-0.419499993f, 106.8713f, 0.186739147f);
        Vector3 fenceScale = new Vector3(33.7172432f, 31.7059441f, 1);

        minX = fencePosition.x - (fenceScale.x / 2);
        maxX = fencePosition.x + (fenceScale.x / 2);
        minY = fencePosition.y - (fenceScale.y / 2);
        maxY = fencePosition.y + (fenceScale.y / 2);

        SetNewTargetPosition();
    }

    void Update()
    {
        MoveChicken();
    }

    private void MoveChicken()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTargetPosition();
        }
    }

    private void SetNewTargetPosition()
    {
        // Генерация случайной позиции внутри области загона
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        targetPosition = new Vector3(x, y, 0);
    }
}