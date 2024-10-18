using UnityEngine;

public class UniversalVerticalScrollMenu : UniversalScrollMenu
{
    public override void Up() => Move(-1);
    public override void Down() => Move(1);
    public override void Right()
    {
        if (!EnableInput || LockInput) return;
        items[selectedIndex].Right();
    }
    public override void Left()
    {
        if (!EnableInput || LockInput) return;
        items[selectedIndex].Left();
    }

    protected override void FixScroll()
    {
        var targetRect = items[selectedIndex].GetComponent<RectTransform>();
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
}
