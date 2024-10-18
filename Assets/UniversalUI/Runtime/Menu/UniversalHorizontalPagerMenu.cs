public abstract class UniversalHorizontalPagerMenu<TItem, TData>
    : UniversalPagerMenu<TItem, TData>
    where TItem : UniversalItemBase
{

    public override void Right() => Move(1);
    public override void Left() => Move(-1);
    public override void Up() => MovePage(-1);
    public override void Down() => MovePage(1);
}
