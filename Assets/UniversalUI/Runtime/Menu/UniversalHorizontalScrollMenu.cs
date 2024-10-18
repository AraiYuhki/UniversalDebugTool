using UnityEngine;

public class UniversalHorizontalScrollMenu : UniversalScrollMenu
{
    public override void Right() => Move(1);
    public override void Left() => Move(-1);
    public override void Up()
    {
        if (!EnableInput || LockInput) return;
        items[selectedIndex].Up();
    }
    public override void Down()
    {
        if (!EnableInput || LockInput) return;
        items[selectedIndex].Down();
    }

    protected override void FixScroll()
    {
        var targetRect = items[selectedIndex].GetComponent<RectTransform>();
        var contentWidth = scrollView.content.rect.width;
        var viewPortWidth = scrollView.viewport.rect.width;
        // スクロールの必要なし
        if (contentWidth < viewPortWidth) return;

        // pivotによるズレをrect.xで補正
        var positionX = targetRect.localPosition.x - targetRect.rect.x;
        // ローカル座標が、contentWidthの左端を0として正の値で格納されている
        var targetPosition = contentWidth - positionX + targetRect.rect.width * Align;

        // 左端～右端合わせのための調整量
        var gap = viewPortWidth * Align;
        var value = (targetPosition - gap) / (contentWidth - viewPortWidth);
        // 1～0の範囲になっているので逆数にする
        scrollView.horizontalNormalizedPosition = Mathf.Clamp01(1f - value);
    }
}
