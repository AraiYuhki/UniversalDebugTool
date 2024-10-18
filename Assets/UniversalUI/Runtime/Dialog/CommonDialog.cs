using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CommonDialog : DialogBase
{
    [SerializeField]
    private TMP_Text message;
    [SerializeField]
    private UniversalButton original;
    [SerializeField]
    private UniversalHorizontalMenu menu;

    public override DialogType Type => DialogType.Common;

    public string Message
    {
        get => message.text;
        set => message.text = value;
    }

    public void Initialize(string title, string message, params (string label, UnityAction onClick)[] data)
    {
        Title = title;
        Message = message;
        menu.Clear();

        foreach ((var label, var onClick) in data)
        {
            var instance = Instantiate(original);
            instance.gameObject.SetActive(true);
            instance.Label = label;
            instance.AddSubmitEvent(onClick);
            menu.AddItem(instance);
        }
        menu.Initialize();
        menu.EnableInput = true;
        original.gameObject.SetActive(false);
    }

    public override void Left() => menu.Left();

    public override void Right() => menu.Right();

    public override void Submit() => menu.Submit();
}
