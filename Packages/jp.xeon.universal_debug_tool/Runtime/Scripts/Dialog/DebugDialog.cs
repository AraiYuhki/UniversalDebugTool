using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Xeon.UniversalUI;

namespace Xeon.UniversalDebugTool
{
    public class DebugDialog : MonoBehaviour
    {
        private static readonly int OpenID = Animator.StringToHash("Open");
        private static readonly int CloseID = Animator.StringToHash("Close");

        [SerializeField]
        private TMP_Text title, message;
        [SerializeField]
        private UniversalHorizontalMenu menu;
        [SerializeField]
        private UniversalButton buttonPrefab;
        [SerializeField]
        private Animator animator;

        public event UnityAction OnOpen;
        public event UnityAction OnClose;
        public event UnityAction OnCancel;

        private CancellationTokenSource cts;

        public void Setup(string title, string message, params (string label, UnityAction onClick)[] data)
        {
            this.title.text = title;
            this.message.text = message;

            menu.Clear();
            foreach (var (label, onClick) in data)
            {
                var button = Instantiate(buttonPrefab);
                button.Label = label;
                button.AddSubmitEvent(onClick);
                menu.AddItem(button);
            }
            menu.Initialize();
            menu.EnableInput = false;
        }

        public async UniTask OpenAsync()
        {
            cts?.Cancel();
            try
            {
                cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
                await animator.PlayAsync(OpenID, token: cts.Token);
                menu.EnableInput = true;
                OnOpen?.Invoke();
            }
            finally
            {
                cts = null;
            }
        }

        public async UniTask CloseAsync()
        {
            cts?.Cancel();
            try
            {
                cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
                menu.EnableInput = false;
                await animator.PlayAsync(CloseID, token: cts.Token);
                OnClose?.Invoke();
            }
            finally
            {
                cts = null;
            }
        }

        public void Up() => menu.Up();
        public void Down() => menu.Down();
        public void Left() => menu.Left();
        public void Right() => menu.Right();
        public void Submit() => menu.Submit();
        public void Cancel() => OnCancel?.Invoke();
    }
}
