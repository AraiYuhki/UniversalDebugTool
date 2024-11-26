using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Xeon.UniversalDebugTool
{
    public class DebugDialogManager : MonoBehaviour
    {
        [SerializeField]
        private Image mask;
        [SerializeField]
        private DebugDialog dialogPrefab;

        private List<DebugDialog> dialogStack = new();
        private DebugDialog currentDialog = null;

        public DebugDialog CreateDialog(string title, string message, params (string label, UnityAction onClick)[] data)
        {
            var dialog = Instantiate(dialogPrefab, transform);
            dialog.gameObject.SetActive(false);
            dialog.Setup(title, message, data);
            return dialog;
        }

        public void OpenDialog(DebugDialog dialog)
        {
            mask.gameObject.SetActive(true);
            dialog.gameObject.SetActive(true);
            if (currentDialog != null)
            {
                currentDialog.CloseAsync().Forget();
                dialogStack.Add(currentDialog);
            }
            currentDialog = dialog;
            currentDialog.OpenAsync().Forget();
        }

        public void CloseDialog(DebugDialog dialog)
        {
            if (dialog == null) return;
            if (dialog != currentDialog)
            {
                dialogStack.Remove(dialog);
                Destroy(dialog.gameObject);
                return;
            }
            dialog.CloseAsync().ContinueWith(() => Destroy(dialog.gameObject)).Forget();
            currentDialog = dialogStack.LastOrDefault();
            if (currentDialog == null)
            {
                mask.gameObject.SetActive(false);
                return;
            }
            dialogStack.Remove(currentDialog);
            currentDialog.OpenAsync().Forget();
        }
    }
}
