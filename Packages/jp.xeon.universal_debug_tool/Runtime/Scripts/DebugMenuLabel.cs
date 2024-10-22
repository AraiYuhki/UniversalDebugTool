using TMPro;
using UnityEngine;

namespace Xeon.UniversalDebugTool
{
    public class DebugMenuLabel : MonoBehaviour
    {
        [SerializeField]
        protected TMP_Text label;

        public virtual string Label
        {
            get => label.text;
            set => label.text = value;
        }
    }
}
