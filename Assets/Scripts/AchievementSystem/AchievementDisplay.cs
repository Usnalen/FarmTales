using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementDisplay : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private GameObject completedLabel;
    
    public void SetAchievement(Achievement achievement)
    {
        if (titleText != null)
        {
            titleText.text = achievement.title;
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = achievement.description;
        }
        
        if (progressText != null)
        {
            progressText.text = $"{achievement.currentValue}/{achievement.targetValue}";
        }
        
        if (progressBar != null)
        {
            progressBar.fillAmount = achievement.Progress;
        }
        
        if (rewardText != null)
        {
            rewardText.text = $"Награда: {achievement.rewardCoins} монет";
        }
        
        if (completedLabel != null)
        {
            completedLabel.SetActive(achievement.isCompleted);
        }
        
        if (iconImage != null && achievement.icon != null)
        {
            iconImage.sprite = achievement.icon;
        }
    }
}