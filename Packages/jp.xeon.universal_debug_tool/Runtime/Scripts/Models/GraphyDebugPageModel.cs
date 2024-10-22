#if UNIVERSAL_DEBUG_SUPPORT_GRAPHY
using System;
using Tayx.Graphy;

namespace Xeon.UniversalDebugTool.Model
{
    [Serializable]
    public sealed class GraphyDebugPageModel : DebugPageModel
    {
        private GraphyManager graphyManager;
        private DebugEnumPickerModel<GraphyManager.ModuleState> fpsState;
        private DebugEnumPickerModel<GraphyManager.ModuleState> ramState;
        private DebugEnumPickerModel<GraphyManager.ModuleState> audioState;
        private DebugEnumPickerModel<GraphyManager.ModuleState> advancedState;

        public GraphyDebugPageModel()
        {
            this.title = "Graphy";
        }

        protected override void Initialize()
        {
            graphyManager = GraphyManager.Instance;
            fpsState = new("FPS", graphyManager.FpsModuleState, value => graphyManager.FpsModuleState = value);
            ramState = new("RAM", graphyManager.RamModuleState, value => graphyManager.RamModuleState = value);
            audioState = new("Audio", graphyManager.AudioModuleState, value => graphyManager.AudioModuleState = value);
            advancedState = new("Advanced", graphyManager.AdvancedModuleState, value => graphyManager.AdvancedModuleState = value);
            AddEnumPicker(fpsState);
            AddEnumPicker(ramState);
            AddEnumPicker(audioState);
            AddEnumPicker(advancedState);
        }
    }
}
#endif
