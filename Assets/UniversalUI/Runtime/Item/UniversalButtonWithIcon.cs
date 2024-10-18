using UnityEngine;
using UnityEngine.UI;

public class UniversalButtonWithIcon : UniversalButton
{
    [SerializeField]
    protected Image icon;

    private void Awake()
    {
        icon.gameObject.SetActive(icon.sprite != null);
    }

    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
        icon.gameObject.SetActive(sprite != null);
    }
}
