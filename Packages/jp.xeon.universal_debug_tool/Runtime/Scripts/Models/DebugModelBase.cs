using Xeon.UniversalUI;

public abstract class DebugModelBase
{
    protected bool isActive = true;
    public string Text { get; protected set; } = string.Empty;
    public int Priority { get; protected set; } = 0;

    public DebugModelBase(string text, int priority = 0)
    {
        this.Text = text;
        this.Priority = priority;
    }

    public abstract void CreateItem(UniversalMenuBase menu, UniversalDebugToolSetting prefabDictionary);
}
