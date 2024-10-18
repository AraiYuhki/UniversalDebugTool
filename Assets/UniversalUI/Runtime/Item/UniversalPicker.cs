using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UniversalPicker : UniversalItem
{
    [SerializeField]
    private TMP_Text selectedLabel;
    [SerializeField]
    private int selectedIndex = 0;
    [SerializeField]
    private List<string> values = new List<string>();

    public Action<int> OnValueChanged { get; set; }

    public int SelectedIndex => selectedIndex;
    public void SetItems(List<string> values)
    {
        this.values = values;
        FixIndex();
        UpdateView();
    }

    public void Select(int index)
    {
        selectedIndex = index;
        FixIndex();
        UpdateView();
    }

    public void UpdateView() => selectedLabel.text = values[selectedIndex];

    public override void Right() => Move(1);
    public override void Left() => Move(-1);

    private void FixIndex()
    {
        if (selectedIndex < 0)
            selectedIndex += values.Count;
        else if (selectedIndex >= values.Count)
            selectedIndex -= values.Count;
    }

    private void Move(int index)
    {
        selectedIndex += index;
        FixIndex();
        UpdateView();
        OnValueChanged?.Invoke(selectedIndex);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        FixIndex();
        UpdateView();
    }
#endif
}
