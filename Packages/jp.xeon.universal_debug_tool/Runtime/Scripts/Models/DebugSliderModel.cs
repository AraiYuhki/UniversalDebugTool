using UnityEngine.Events;
using Xeon.UniversalUI;

namespace Xeon.UniversalDebugTool.Model
{
    public class DebugSliderModel : DebugModelBase
    {
        private float value, min, max, step;
        private bool isInterger = false;
        private UnityAction<float> onValueChanged;
        private UnityAction<int> onValueChangedForInt;
        private UniversalSlider slider = null;

        private bool isChanging = false;

        public float Value
        {
            get => value;
            set
            {
                if (isChanging) return;
                isChanging = true;
                this.value = value;
                slider.Value = value;
                isChanging = false;
            }
        }

        public DebugSliderModel(string text, float min, float max, float value, UnityAction<float> onValueChanged, float step = 1f, int priority = 0)
            : base(text, priority)
        {
            this.min = min;
            this.max = max;
            this.step = step;
            this.value = value;
            this.onValueChanged = onValueChanged;
            isInterger = false;
        }

        public DebugSliderModel(string text, int min, int max, int value, UnityAction<int> onValueChanged, int step = 1, int priority = 0)
            : base(text, priority)
        {
            this.min = min;
            this.max = max;
            this.step = step;
            this.value = value;
            this.onValueChangedForInt = onValueChanged;
            isInterger = true;
        }

        private void ApplyToSlider(UniversalSlider slider)
        {
            slider.Label = Text;
            slider.SetMin(min);
            slider.SetData(value, max);
            slider.SetStep(step);
            slider.SetInterger(isInterger);
            slider.OnValueChanged -= OnSliderValueChanged;
            slider.OnValueChanged += OnSliderValueChanged;
            this.slider = slider;
        }

        private void OnSliderValueChanged(float value)
        {
            if (isChanging) return;
            isChanging = true;
            if (isInterger)
                onValueChangedForInt?.Invoke(slider.IntValue);
            else
                onValueChanged?.Invoke(value);
            this.value = value;
            isChanging = false;
        }

        public override void CreateItem(UniversalMenuBase menu, UniversalDebugToolSetting prefabDictionary)
        {
            var instance = prefabDictionary.Instantiate<UniversalSlider>();
            ApplyToSlider(instance);
            menu.AddItem(instance);
        }

    }
}
