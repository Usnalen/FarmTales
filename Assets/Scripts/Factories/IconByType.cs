using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IconByType : MonoBehaviour
{
    [SerializeField] private ResourceType type;

    private void Start()
    {
        if (type == ResourceType.None)
            return;
        GetComponent<Image>().sprite = ResourceIconManager.Instance.GetIcon(type);
    }
}
