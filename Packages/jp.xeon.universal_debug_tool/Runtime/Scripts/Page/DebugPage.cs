using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Xeon.UniversalUI;
using Xeon.XTween;
using Xeon.UniversalDebugTool.Model;

namespace Xeon.UniversalDebugTool
{
    public class DebugPage : MonoBehaviour, IUniversalControllable
    {
        [SerializeField]
        private UniversalVerticalScrollMenu menu;
        [SerializeField]
        private CanvasGroup canvasGroup;

        private DebugPageModel model;
        private CancellationTokenSource cts;
        private UniversalDebugToolSetting prefabDictionary;

        public UniversalVerticalScrollMenu Menu => menu;
        public string Title => model?.Title;

        public void Right() => menu.Right();
        public void Left() => menu.Left();
        public void Up() => menu.Up();
        public void Down() => menu.Down();
        public void Submit() => menu.Submit();
        public void Cancel() => menu.Cancel();

        public async void OpenPage(DebugPageModel model, UniversalDebugToolSetting prefabDictionary)
        {
            this.model = model;
            this.prefabDictionary = prefabDictionary;
            model.OpenPage(menu, prefabDictionary);
            try
            {
                cts?.Cancel();
                cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
                canvasGroup.alpha = 0f;
                await canvasGroup.TweenFade(1f, 0.2f).ToUniTask(cts.Token);
                menu.EnableInput = true;
            }
            finally
            {
                cts = null;
            }
        }

        public async UniTask PushPage()
        {
            try
            {
                cts?.Cancel();
                cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
                await canvasGroup.TweenFade(0f, 0.2f).ToUniTask(cts.Token);
            }
            finally
            {
                cts = null;
                gameObject.SetActive(false);
                menu.EnableInput = false;
            }
        }

        public async UniTask<string> PopPageAsync()
        {
            try
            {
                cts?.Cancel();
                cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
                canvasGroup.alpha = 0f;
                Reopen();
                await canvasGroup.TweenFade(1f, 0.2f).ToUniTask(cts.Token);
                return Title;
            }
            finally
            {
                cts = null;
                gameObject.SetActive(true);
                menu.EnableInput = true;
            }
        }

        public void Reopen()
        {
            var currentSelectIndex = menu.SelectedIndex;
            menu.Clear();
            model.OpenPage(menu, prefabDictionary);
            menu.Select(currentSelectIndex);
        }
    }
}
