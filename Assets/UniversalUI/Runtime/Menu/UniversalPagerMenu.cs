using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public interface IUniversalPagerMenu : IUniversalMenu
{
    void UpdateView();
}

public interface IDataManageMenu<TData>
{
    void Initialize(List<TData> data);
    void AddData(TData data);
    void RemoveData(TData data);
}

public abstract class UniversalPagerMenu<TItem, TData>
    : MonoBehaviour, IUniversalPagerMenu, IItemAddable<TItem>, IITemRemovable<TItem>, IDataManageMenu<TData>
    where TItem : UniversalItemBase
{
    [SerializeField]
    private Transform container;
    [SerializeField]
    protected TMP_Text pageLabel;
    [SerializeField, Tooltip("1ページに何個の要素を入れるか")]
    protected int itemCountPerPage = 10;
    [SerializeField]
    protected TItem itemPrefab;

    protected int selectedIndex = 0;
    protected int currentPage = 0;
    protected int totalPages = 0;

    protected virtual int selectedDataIndex => itemCountPerPage * currentPage + selectedIndex;

    protected List<TItem> items = new();
    protected List<TItem> activeItems = new();
    protected List<TData> data = new();

    public bool LockInput => items[selectedIndex].LockInput;

    public UnityAction<TItem, TData> OnSubmit { get; set; }
    public bool EnableInput { get; set; }
    public List<TItem> Items => items;

    protected virtual void OnEnable()
    {
        Clear();
        for (var count = 0; count < itemCountPerPage; count++)
        {
            var instance = Instantiate(itemPrefab, container);
            instance.Initialize(() => OnSelected(instance), Submit);
            items.Add(instance);
        }
    }

    protected virtual void OnSelected(TItem item)
    {
        if (!EnableInput || LockInput) return;

        items[selectedIndex].UnSelect();
        selectedIndex = activeItems.IndexOf(item);
        items[selectedIndex].Select();
    }

    public void AddItem(TItem item)
    {
        items.Add(item);
        item.transform.SetParent(container, false);
        item.transform.localScale = Vector3.one;
    }

    public void RemoveItem(TItem item, bool destroy = true)
    {
        items.Remove(item);
        if (destroy) Destroy(item.gameObject);
        ReselectCurrentItem();
    }

    public void AddData(TData data)
    {
        this.data.Add(data);
        UpdateView();
    }

    public void RemoveData(TData data)
    {
        this.data.Remove(data);
        UpdateView();
    }

    public virtual void ReselectCurrentItem()
    {
        if (activeItems.Count <= 0) return;
        FixIndex();
        foreach (var item in activeItems)
            item.UnSelect();
        items[selectedIndex].Select();
    }

    public void Clear()
    {
        foreach (var item in items)
            Destroy(item.gameObject);
        items.Clear();
        activeItems.Clear();
    }

    public virtual void Initialize(List<TData> data)
    {
        this.data = data;
        Initialize();
    }

    public virtual void Initialize()
    {
        currentPage = 0;
        selectedIndex = 0;
        UpdateView();
        ReselectCurrentItem();
    }

    public virtual void UpdateView()
    {
        foreach (var item in items)
        {
            item.UnSelect();
            item.gameObject.SetActive(false);
        }
        activeItems.Clear();

        totalPages = Mathf.CeilToInt((float)data.Count / itemCountPerPage);
        FixPageNumber();

        var pageEndIndex = Mathf.Min(data.Count, itemCountPerPage * (currentPage + 1));
        var itemIndex = 0;
        for (var index = itemCountPerPage * currentPage; index < pageEndIndex; index++, itemIndex++)
        {
            var instance = items[itemIndex];
            Setup(instance, data[index], index);
        }

        UpdatePageLabel();
    }

    protected virtual void UpdatePageLabel()
        => pageLabel.text = $"{currentPage + 1}/ {totalPages}";

    protected virtual void FixIndex()
    {
        if (selectedIndex < 0) selectedIndex += activeItems.Count;
        else if (selectedIndex >= activeItems.Count) selectedIndex -= activeItems.Count;
    }

    protected virtual void FixPageNumber()
    {
        if (currentPage >= totalPages) currentPage -= totalPages;
        else if (currentPage < 0) currentPage += totalPages;
    }

    protected virtual void Move(int move)
    {
        if (!EnableInput || LockInput) return;
        items[selectedIndex].UnSelect();
        selectedIndex += move;
        FixIndex();
        items[selectedIndex].Select();
    }

    protected virtual void MovePage(int move)
    {
        if (!EnableInput || LockInput) return;
        currentPage += move;
        FixPageNumber();
        UpdateView();
        FixIndex();
        ReselectCurrentItem();
    }

    public virtual void Submit()
    {
        if (!EnableInput || LockInput) return;
        OnSubmit?.Invoke(activeItems[selectedIndex], data[selectedDataIndex]);
    }

    protected virtual void Setup(TItem instance, TData data, int index)
    {
        instance.gameObject.SetActive(true);
        activeItems.Add(instance);
    }

    public abstract void Right();

    public abstract void Left();

    public abstract void Up();

    public abstract void Down();
}
