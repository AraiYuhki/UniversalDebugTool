using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public static class AnimatorExtension
{
    public static async void Play(this Animator self, int hash, int layer = 0, Action onComplete = null)
    {
        self.Play(hash, layer);
        await UniTask.Yield(self.GetCancellationTokenOnDestroy());
        var stateInfo = self.GetCurrentAnimatorStateInfo(layer);
        await UniTask.Delay(TimeSpan.FromSeconds(stateInfo.length), cancellationToken: self.GetCancellationTokenOnDestroy());
        onComplete?.Invoke();
    }

    public static async UniTask PlayAsync(this Animator self, int hash, int layer = 0, CancellationToken token = default)
    {
        self.Play(hash, layer);
        await UniTask.Yield(token);
        var stateInfo = self.GetCurrentAnimatorStateInfo(layer);
        await UniTask.Delay(TimeSpan.FromSeconds(stateInfo.length), cancellationToken: token);
    }
}
