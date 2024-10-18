using UnityEngine;
using UnityEngine.UI;

public abstract class UniversalScrollMenu : UniversalMenuBase
{
    [SerializeField]
    protected ScrollRect scrollView;
    [SerializeField, Range(0f, 1f)]
    private float align = 1f;
    protected float Align
    {
        get => align;
        set => align = Mathf.Clamp01(value);
    }

    protected virtual void Move(int move)
    {
        if (!EnableInput || LockInput) return;

        items[selectedIndex].UnSelect();
        selectedIndex += move;
        FixIndex();
        items[selectedIndex].Select();
        ReselectCurrentItem();
        FixScroll();
    }

    protected abstract void FixScroll();
}
