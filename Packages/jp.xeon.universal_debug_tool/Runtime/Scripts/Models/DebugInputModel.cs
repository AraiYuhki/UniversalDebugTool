using UnityEngine;
using UnityEngine.Events;
using Xeon.UniversalUI;

public class DebugInputModel : DebugModelBase
{
    private string value;
    private UnityAction<string> onValueChanged;

    private bool isChanging = false;
    private UniversalInputField inputField;

    public string Value
    {
        get => value;
        set
        {
            if (isChanging) return;
            isChanging = true;

            this.value = value;
            inputField.Text = value;

            isChanging = false;
        }
    }

    public DebugInputModel(string text, string value, UnityAction<string> onValueChanged, int priority = 0)
        : base(text, priority)
    {
        this.value = value;
        this.onValueChanged = onValueChanged;
    }

    private void ApplyToInputField(UniversalInputField inputField)
    {
        inputField.Label = Text;
        inputField.Text = value;
        inputField.OnValueChanged -= OnInputFieldValueChanged;
        inputField.OnValueChanged += OnInputFieldValueChanged;
        this.inputField = inputField;
    }

    private void OnInputFieldValueChanged(string text)
    {
        if (isChanging) return;
        isChanging = true;

        this.value = text;

        isChanging = false;
    }

    public override void CreateItem(UniversalMenuBase menu, UniversalDebugToolSetting prefabDictionary)
    {
        var instance = prefabDictionary.Instantiate<UniversalInputField>();
        ApplyToInputField (instance);
        menu.AddItem(instance);
    }
}
