using TMPro;
using UnityEngine;
using UnityEngine.Events;

public abstract class UniversalItem : UniversalItemBase
{
    [SerializeField]
    protected TMP_Text titleLabel;
    [SerializeField]
    protected float duration;
    [SerializeField]
    protected UnityAction onSubmit;

    protected override float fadeDuration => duration;

    public override void AddSubmitEvent(UnityAction action)
    {
        RemoveSubmitEvent(action);
        onSubmit += action;
    }

    public override void RemoveSubmitEvent(UnityAction action)
    {
        onSubmit -= action;
    }

    public virtual string Label
    {
        get => titleLabel.text;
        set => titleLabel.text = value;
    }

    public override void Submit() => onSubmit?.Invoke();
}
