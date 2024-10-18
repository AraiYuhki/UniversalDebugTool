using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DebugPageModel
{
    private UniversalMenuBase menu;
    private DebugPage page;
    private List<DebugModelBase> pageItemModels = new();
    private string title = string.Empty;

    public string Title => title;

    public ReadOnlyCollection<DebugModelBase> PageItemModels 
    {
        get
        {
            return new ReadOnlyCollection<DebugModelBase>(pageItemModels.OrderBy(model => model.Priority).ToList());
        }
    }
    public DebugModelBase this[int index] => pageItemModels[index];
    public int Count => pageItemModels.Count;

    public DebugPageModel() { }

    public DebugPageModel(string title)
    {
        this.title = title;
    }

    public DebugPageModel(string title, DebugPageModel other)
    {
        this.title = title;
        var copyItemModels = new DebugModelBase[other.pageItemModels.Count];
        other.pageItemModels.CopyTo(copyItemModels);
        pageItemModels = copyItemModels.ToList();
    }

    protected virtual void Initialize()
    {
    }

    public void OpenPage(UniversalMenuBase menu, PrefabDictionary prefabDictionary)
    {
        menu.Clear(true);
        Initialize();
        foreach (var model in pageItemModels.OrderBy(model => model.Priority))
            model.CreateItem(menu, prefabDictionary);
        menu.Initialize();
        menu.EnableInput = true;
    }

    public void AddButton(string text, UnityAction onClick, Sprite icon = null, int priority = 0)
        => pageItemModels.Add(new DebugButtonModel(text, onClick, icon, priority));
    public void AddButton(DebugButtonModel model) => pageItemModels.Add(model);

    public void AddPageLinkButton<T>() where T : DebugPageModel, new()
        => DebugMenu.Instance.OpenPage(new T());
    public void AddPageLinkButton<T>(T model) where T : DebugPageModel, new()
        => DebugMenu.Instance.OpenPage(model);

    public void AddSlider(string text, float min, float max, float value, UnityAction<float> onValueChanged, float step = 1f, int priority = 0)
        => pageItemModels.Add(new DebugSliderModel(text, min, max, value, onValueChanged, step, priority));
    public void AddIntSlider(string text, int min, int max, int value, UnityAction<int> onValueChanged, int step = 1, int priority = 0)
        => pageItemModels.Add(new DebugSliderModel(text, min, max, value, onValueChanged, step, priority));
    public void AddSlider(DebugSliderModel model)
        => pageItemModels.Add(model);

    public void AddPicker<T>(string text, List<string> labels, List<T> values, T value, UnityAction<T> onValueChanged, int priority = 0)
        => pageItemModels.Add(new DebugPickerModel<T>(text, labels, values, value, onValueChanged, priority));
    public void AddPicker<T>(DebugPickerModel<T> model)
        => pageItemModels.Add(model);
    public void AddEnumPicker<T>(string text, T value, UnityAction<T> onValueChanged, int priority = 0) where T : Enum
        => pageItemModels.Add(new DebugEnumPickerModel<T>(text, value, onValueChanged, priority));
    public void AddEnumPicker<T>(DebugEnumPickerModel<T> model) where T : Enum
        => pageItemModels.Add(model);

    public void AddToggle(string text, bool value, UnityAction<bool> onValueChanged, int priority = 0)
        => pageItemModels.Add(new DebugToggleModel(text, value, onValueChanged, priority));
    public void AddToggle(DebugToggleModel model)
        => pageItemModels.Add(model);

    public void AddInputField(string text, string value, UnityAction<string> onValueChanged, int priority = 0)
        => pageItemModels.Add(new DebugInputModel(text, value, onValueChanged, priority));
    public void AddInputField(DebugInputModel model)
        => pageItemModels.Add(model);
}
