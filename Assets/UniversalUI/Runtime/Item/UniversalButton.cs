using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UniversalButton : UniversalItemBase
{
    [SerializeField]
    protected Button button;
    [SerializeField]
    protected TMP_Text label;
    [SerializeField]
    protected bool enableSubmitSE = true;

    public override void AddSubmitEvent(UnityAction action)
    {
        RemoveSubmitEvent(action);
        button.onClick.AddListener(action);
    }

    public override void RemoveSubmitEvent(UnityAction action)
    {
        button.onClick.RemoveListener(action);
    }

    protected override float fadeDuration => button.colors.fadeDuration;
    public bool Interactable
    {
        get => button.interactable;
        set => button.interactable = value;
    }

    public string Label
    {
        get => label.text;
        set => label.text = value;
    }

    protected override void OnEnable()
    {
        targetImage.color = isSelected ? selectedColor : normalColor;
    }

    public override void Initialize(Action onSelect = null, Action onSubmit = null)
    {
        button.onClick.AddListener(() => onSubmit?.Invoke());
        base.Initialize(onSelect);
    }

    public override void Submit()
    {
        if (!button.interactable) return;
        button.onClick?.Invoke();
    }
}
