using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Xeon.XTween;

public class DialogManager : MonoSingleton<DialogManager>
{
    [SerializeField]
    private Image basePanel;

    [SerializeField]
    private List<DialogBase> dialogs = new List<DialogBase>();

    private Dictionary<Type, DialogBase> dialogDict = new();

    private List<DialogBase> dialogQueue = new List<DialogBase>();

    private CancellationTokenSource cts;

    public bool OpeningDialog => dialogQueue.Any();
    public DialogBase Current => OpeningDialog ? dialogQueue.First() : null;

    public void Start()
    {
        basePanel.color = new Color(0f, 0f, 0f, 0f);
        basePanel.enabled = false;
        dialogDict = dialogs.ToDictionary(dialog => dialog.GetType(), dialog => dialog);
    }

    public T Create<T>() where T : DialogBase
    {
        if (!dialogDict.TryGetValue(typeof(T), out var original))
        {
            Debug.LogError($"{typeof(T)}'s dialog is not found");
            return null;
        }

        if (dialogQueue.Any())
        {
            var oldDialog = dialogQueue.First();
            oldDialog.CloseAsync(oldDialog.destroyCancellationToken).Forget();
        }
        else
        {
            cts?.Cancel();
            cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            basePanel.enabled = true;
            basePanel.TweenFade(0.5f, 0.2f).ToUniTask(cts.Token).ContinueWith(() => cts = null).Forget();
        }
        var dialog = Instantiate(original, transform) as T;
        dialogQueue.Insert(0, dialog); // キューの先頭に追加
        return dialog;
    }

    public async UniTask CloseAsync(DialogBase dialog)
    {
        var isFirst = dialogQueue.First() == dialog;
        if (isFirst)
        {
            dialogQueue.Remove(dialog);
            dialog.CloseAsync(dialog.destroyCancellationToken).ContinueWith(() => Destroy(dialog.gameObject)).Forget();
            // キューにダイアログが残っているなら新しく開く
            if (dialogQueue.Any())
            {
                var newDialog = dialogQueue.First();
                if (newDialog != null)
                    await newDialog.OpenAsync(newDialog.destroyCancellationToken);
            }
            if (!dialogQueue.Any())
            {
                cts?.Cancel();
                cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
                await basePanel.TweenFade(0f, 0.2f).ToUniTask(cts.Token);
                cts = null;
                basePanel.enabled = false;
            }
            return;
        }
        // 現在開いているものではないのでクローズ処理をせずに削除
        dialogQueue.Remove(dialog);
        Destroy(dialog.gameObject);
    }

    public T Swap<T>() where T : DialogBase
    {
        if (dialogQueue.Count > 0)
        {
            var prevDialog = dialogQueue.First();
            prevDialog.CloseAsync(prevDialog.destroyCancellationToken).ContinueWith(() => Destroy(prevDialog.gameObject)).Forget();
            dialogQueue.Remove(prevDialog);
        }
        else
        {
            cts?.Cancel();
            cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            basePanel.enabled = true;
            basePanel.TweenFade(0.5f, 0.2f).ToUniTask(cts.Token).ContinueWith(() => cts = null).Forget();
        }

        var original = dialogs.FirstOrDefault(dialog => dialog.GetType() == typeof(T));
        if (original == null)
        {
            UnityEngine.Debug.LogError($"{typeof(T)}'s dialog is not found");
            return null;
        }
        var dialog = Instantiate(original, transform) as T;
        dialogQueue.Insert(0, dialog);
        return dialog;
    }

    private void Update()
    {
        dialogQueue.FirstOrDefault()?.Control();
    }
}
