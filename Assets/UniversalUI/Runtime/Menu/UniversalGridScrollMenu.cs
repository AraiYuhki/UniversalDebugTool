using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UniversalGridScrollMenu : UniversalScrollMenu
{
    [SerializeField]
    private GridLayoutGroup grid;
    [SerializeField]
    private int columnCount = 4;

    private Vector2Int cursorPosition = Vector2Int.zero;
    private List<List<UniversalItemBase>> itemGrid = new();

    protected override int selectedIndex
    {
        get => cursorPosition.x + cursorPosition.y * columnCount;
        set
        {
            cursorPosition.x = value % columnCount;
            cursorPosition.y = Mathf.CeilToInt((float)value / columnCount);
            FixIndex(true);
        }
    }

    protected override void FixIndex()
    {
    }

    private void FixIndex(bool clampXPosition)
    {
        if (cursorPosition.y < 0)
            cursorPosition.y += itemGrid.Count;
        else if (cursorPosition.y >= itemGrid.Count)
            cursorPosition.y %= itemGrid.Count;

        var columnNum = itemGrid[cursorPosition.y].Count;
        if (cursorPosition.x < 0)
            cursorPosition.x += columnNum;
        else if (cursorPosition.x >= columnNum)
        {
            if (clampXPosition)
                cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0, columnNum - 1);
            else
                cursorPosition.x %= columnNum;
        }
    }

    protected override void FixScroll()
    {
        var targetRect = itemGrid[cursorPosition.y][cursorPosition.x].GetComponent<RectTransform>();
        var contentHeight = scrollView.content.rect.height;
        var viewPortHeight = scrollView.viewport.rect.height;
        // スクロールの必要なし
        if (contentHeight < viewPortHeight) return;

        // pivotによるズレをrect.yで補正
        var positionY = targetRect.localPosition.y + targetRect.rect.y;
        // ローカル座標が、contentHeightの上辺を0として負の値で格納されている
        var targetPosition = contentHeight + positionY + targetRect.rect.height * Align;

        // 上端～下端合わせのための調整量
        var gap = viewPortHeight * Align;
        scrollView.verticalNormalizedPosition = Mathf.Clamp01((targetPosition - gap) / (contentHeight - viewPortHeight));
    }

    public override void Initialize()
    {
        base.Initialize();
        foreach (var (item, index) in items.Select((item, index) => (item, index)))
            item.transform.SetSiblingIndex(index);
        itemGrid.Clear();
        itemGrid.Add(new());
        foreach (var item in items)
        {
            if (itemGrid.Last().Count >= columnCount)
                itemGrid.Add(new());
            itemGrid.Last().Add(item);
        }
    }

    protected override void Move(int move)
    {
    }

    private void Move(Vector2Int move)
    {
        if (!EnableInput || LockInput) return;
        itemGrid[cursorPosition.y][cursorPosition.x].UnSelect();
        cursorPosition += move;
        FixIndex(move.y != 0);
        itemGrid[cursorPosition.y][cursorPosition.x].Select();
        FixScroll();
    }

    public override void Down() => Move(Vector2Int.up);
    public override void Up() => Move(Vector2Int.down);
    public override void Right() => Move(Vector2Int.right);
    public override void Left() => Move(Vector2Int.left);

    private void SetupGrid()
    {
        if (grid == null) return;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columnCount;
    }

    private void OnEnable()
    {
        SetupGrid();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        SetupGrid();
    }
#endif
}
