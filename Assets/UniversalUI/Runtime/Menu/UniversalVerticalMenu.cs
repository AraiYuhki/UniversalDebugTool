public class UniversalVerticalMenu : UniversalMenuBase
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
}
