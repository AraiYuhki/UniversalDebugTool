using Xeon.UniversalUI;

namespace Xeon.UniversalDebugTool.Model
{
    public class DebugLabelModel : DebugModelBase
    {
        protected DebugMenuLabel label;
        public DebugLabelModel(string text, int priority = 0) : base(text, priority)
        {
        }

        public override void CreateItem(UniversalMenuBase menu, UniversalDebugToolSetting prefabDictionary)
        {
            label = prefabDictionary.InstantiateLabel();
            label.Label = Text;
            menu.AddUnselectableItem(label.gameObject);
        }
    }
}
