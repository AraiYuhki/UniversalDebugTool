public abstract class UniversalVerticalPagerMenu<TItem, TData> : UniversalPagerMenu<TItem, TData>
    where TItem : UniversalItemBase
{
    public override void Right() => MovePage(1);

    public override void Left() => MovePage(-1);

    public override void Up() => Move(-1);
    public override void Down() => Move(1);
}
