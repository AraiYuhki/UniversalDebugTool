using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UniversalInputField : UniversalItemBase
{
    [SerializeField]
    protected TMP_InputField input;
    [SerializeField]
    protected TMP_Text label;

    private bool isEditing = false;
    private bool isChanging = false;

    protected override float fadeDuration => input.colors.fadeDuration;
    public override bool LockInput => isEditing;
    public Action<string> OnValueChanged { get; set; }

    public string Label
    {
        get => label.text;
        set => label.text = value;
    }

    public string Text
    {
        get => input.text;
        set
        {
            if (isChanging) return;
            isChanging = true;

            input.text = value;

            isChanging = false;
        }
    }

    private void Awake()
    {
        input.onValueChanged.RemoveListener(OnInputValueChanged);
        input.onValueChanged.AddListener(OnInputValueChanged);
        input.onDeselect.AddListener(_ =>
        {
            if (isEditing)
                isEditing = false;
        });
    }

    public override void AddSubmitEvent(UnityAction action)
    {
    }

    public override void RemoveSubmitEvent(UnityAction action)
    {
    }

    private void OnInputValueChanged(string text)
    {
        if (isChanging) return;
        isChanging = true;

        OnValueChanged?.Invoke(text);

        isChanging = false;
    }

    public override void Submit()
    {
        isEditing = !isEditing;
        if (isEditing)
            input.ActivateInputField();
        else
            input.DeactivateInputField();
    }
}
