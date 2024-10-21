using UnityEngine.Events;
using Xeon.UniversalUI;

public class DebugToggleModel : DebugModelBase
{
    private bool value = false;
    private UnityAction<bool> onValueChanged;

    private UniversalToggle toggle = null;
    private bool isChanging = false;

    public bool Value
    {
        get => value;
        set
        {
            if (isChanging) return;
            isChanging = true;

            toggle.Value = value;
            this.value = value;

            isChanging = false;
        }
    }

    public DebugToggleModel(string text, bool value, UnityAction<bool> onValueChanged, int priority = 0)
        : base(text, priority)
    {
        this.value = value;
        this.onValueChanged = onValueChanged;
    }

    private void ApplyToToggle(UniversalToggle toggle)
    {
        toggle.Label = Text;
        toggle.Value = value;
        toggle.OnValueChanged -= OnValueChanged;
        toggle.OnValueChanged += OnValueChanged;
        this.toggle = toggle;
    }

    private void OnValueChanged(bool value)
    {
        if (isChanging) return;
        isChanging = true;

        this.value = value;
        onValueChanged?.Invoke(value);

        isChanging = false;
    }


    public override void CreateItem(UniversalMenuBase menu, UniversalDebugToolSetting prefabDictionary)
    {
        var instance = prefabDictionary.Instantiate<UniversalToggle>();
        ApplyToToggle(instance);
        menu.AddItem(instance);
    }
}
