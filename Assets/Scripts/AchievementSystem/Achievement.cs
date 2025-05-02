using UnityEngine;

[System.Serializable]
public class Achievement
{
    public AchievementType type;
    public string title;
    public string description;
    public int targetValue;
    public int currentValue;
    public int rewardCoins;
    public bool isCompleted;
    public Sprite icon;

    public float Progress => (float)currentValue / targetValue;

    public void UpdateProgress(int value)
    {
        if (isCompleted) return;
        
        currentValue = value;
        if (currentValue >= targetValue && !isCompleted)
        {
            isCompleted = true;
            if (CoinManager.instance != null)
            {
                CoinManager.instance.AddCoins(rewardCoins);
            }
        }
    }
}