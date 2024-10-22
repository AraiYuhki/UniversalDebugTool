using System;
using System.Linq;
using UnityEngine.Events;

namespace Xeon.UniversalDebugTool.Model
{
    public class DebugEnumPickerModel<T> : DebugPickerModel<T>
        where T : Enum
    {
        public DebugEnumPickerModel(string text, T value, UnityAction<T> onValueChanged, int priority = 0)
            : base(
                text,
                Enum.GetNames(typeof(T)).ToList(),
                Enum.GetValues(typeof(T)).Cast<T>().ToList(),
                value,
                onValueChanged,
                priority)
        {
        }
    }
}
