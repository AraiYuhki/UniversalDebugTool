using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xeon.XTween;

[RequireComponent(typeof(EventTrigger))]
public abstract class UniversalItemBase : MonoBehaviour, IUniversalControllable
{
    [SerializeField]
    protected Color normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField]
    protected Color selectedColor = new Color(0f, 1f, 1f, 0.5f);
    [SerializeField]
    protected Image targetImage;

    protected bool isSelected = false;
    protected Tweener tween;
    protected Action onSelect;

    public virtual bool LockInput => false;

    protected abstract float fadeDuration { get; }
    public abstract void AddSubmitEvent(UnityAction action);
    public abstract void RemoveSubmitEvent(UnityAction action);

    public virtual void Select()
    {
        if (this.isSelected) return;
        this.isSelected = true;
        ChangeColor();
    }

    public virtual void UnSelect()
    {
        if (!isSelected) return;
        isSelected = false;
        ChangeColor();
    }

    protected virtual void OnEnable()
    {
        targetImage.color = isSelected ? selectedColor : normalColor;
    }

    protected virtual void ChangeColor()
    {
        if (tween != null)
        {
            tween?.Kill();
            tween = null;
        }
        var destinationColor = isSelected ? selectedColor : normalColor;
        tween = targetImage.TweenColor(destinationColor, fadeDuration).OnComplete(() => tween = null);
    }

    public virtual void Initialize(Action onSelect = null, Action onSubmit = null)
    {
        this.onSelect = () => onSelect?.Invoke();
    }

    public virtual void OnHover() => onSelect?.Invoke();
    public void OnMouseEnter() => onSelect?.Invoke();
    public virtual void Right() { }
    public virtual void Left() { }
    public virtual void Up() { }
    public virtual void Down() { }

    public abstract void Submit();

    protected void OnDestroy()
    {
        tween?.Kill();
        tween = null;
    }
}
