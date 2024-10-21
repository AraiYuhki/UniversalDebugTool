using System;
using System.Collections.Generic;
using UnityEngine.Events;
using Xeon.UniversalUI;

public class InvalidArgmentException : Exception
{
    public InvalidArgmentException(string message) : base(message) { }
}

public class DataNotFoundException : Exception
{
    public DataNotFoundException(string message) : base(message) { }
}

public class DebugPickerModel<T> : DebugModelBase
{
    protected List<string> labels = new();
    protected List<T> values = new();
    protected int selectedIndex = 0;
    protected T selectedValue = default;
    protected UnityAction<T> onValueChanged = null;

    protected UniversalPicker picker;
    protected bool isChanging = false;

    public T Value
    {
        get => selectedValue;
        set
        {
            if (isChanging) return;
            isChanging = true;

            selectedValue = value;
            selectedIndex = IndexOf(value);
            picker.Select(selectedIndex);

            isChanging = false;
        }
    }

    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            if (isChanging) return;
            isChanging = true;

            selectedIndex = value;
            selectedValue = values[value];
            picker.Select(value);

            isChanging = false;
        }
    }

    public DebugPickerModel(string text, List<string> labels, List<T> values, T value, UnityAction<T> onValueChanged, int priority = 0)
        : base(text, priority)
    {
        if (labels.Count != values.Count)
            throw new InvalidArgmentException("Labels count and values count is not equals");
        this.labels = labels;
        this.values = values;
        this.selectedValue = value;
        this.selectedIndex = IndexOf(value);
        this.onValueChanged = onValueChanged;
    }

    protected int IndexOf(T value)
    {
        for (var index = 0; index < values.Count; index++)
        {
            if (values[index].Equals(value))
                return index;
        }

        throw new DataNotFoundException($"{value} is not found in picker's list");
    }

    protected void OnValueChanged(int index)
    {
        if (index < 0 || index >= values.Count)
            throw new IndexOutOfRangeException();
        if (isChanging) return;
        isChanging = true;

        selectedValue = values[index];
        selectedIndex = index;
        onValueChanged?.Invoke(selectedValue);

        isChanging = false;
    }

    protected void ApplyToPicker(UniversalPicker picker)
    {
        picker.Label = Text;
        picker.SetItems(labels);
        picker.Select(selectedIndex);
        picker.OnValueChanged -= OnValueChanged;
        picker.OnValueChanged += OnValueChanged;
        this.picker = picker;
    }

    public override void CreateItem(UniversalMenuBase menu, UniversalDebugToolSetting prefabDictionary)
    {
        var instance = prefabDictionary.Instantiate<UniversalPicker>();
        ApplyToPicker(instance);
        menu.AddItem(instance);
    }
}
