using UnityEngine;
using UnityEngine.Events;

public class DebugButtonModel : DebugModelBase
{
    private UnityAction onClick = null;
    private Sprite icon = null;

    private UniversalButtonWithIcon button = null;

    public DebugButtonModel(string text, UnityAction onClick, Sprite icon = null, int priority = 0)
        : base(text, priority)
    {
        this.onClick = onClick;
        this.icon = icon;
    }

    protected void ApplyToButton(UniversalButtonWithIcon button)
    {
        button.RemoveSubmitEvent(onClick);
        button.AddSubmitEvent(onClick);
        button.Label = Text;
        button.SetIcon(icon);
        this.button = button;
    }

    public override void CreateItem(UniversalMenuBase menu, PrefabDictionary prefabDictionary)
    {
        var instance = prefabDictionary.Instantiate<UniversalButtonWithIcon>();
        ApplyToButton(instance);
        menu.AddItem(instance);
    }
}
