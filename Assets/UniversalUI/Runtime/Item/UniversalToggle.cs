using System;
using UnityEngine;
using UnityEngine.UI;

public class UniversalToggle : UniversalItem
{
    [SerializeField]
    private Toggle toggle;

    public Action<bool> OnValueChanged { get; set; }
    public bool Value
    {
        get => toggle.isOn;
        set => toggle.isOn = value;
    }

    private void Awake()
    {
        toggle.onValueChanged.AddListener(value => OnValueChanged?.Invoke(value));
    }
    public override void Submit()
    {
        Value = !Value;
        base.Submit();
    }
}
