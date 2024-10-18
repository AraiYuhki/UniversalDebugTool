public class UniversalHorizontalMenu : UniversalMenuBase
{
    protected void Move(int move)
    {
        if (!EnableInput || LockInput) return;

        items[selectedIndex].UnSelect();
        selectedIndex += move;
        FixIndex();
        items[selectedIndex].Select();
        ReselectCurrentItem();
    }

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
}
