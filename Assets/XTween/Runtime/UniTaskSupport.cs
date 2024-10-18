#if XTWEEN_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Xeon.XTween
{
    public static class UniTaskSupport
    {
        public static UniTask ToUniTask(this Tweener self, CancellationToken cancellationToken = default)
        {
            if (self == null) throw new NullReferenceException();
            return new UniTask(TweenConfiguredSource.Create(self, cancellationToken, out var token), token);
        }

        /// <summary>
        /// Tween用のAwaiter
        /// </summary>
        private struct TweenAwaiter : ICriticalNotifyCompletion
        {
            private readonly Tweener tween;
            public bool IsCompleted => tween.IsCompleted;
            public TweenAwaiter(Tweener tween) => this.tween = tween;
            public TweenAwaiter GetAwaiter() => this;
            public void GetResult() { }

            public void UnsafeOnCompleted(Action continuation) => tween.OnComplete(PooledTweenCallback.Create(continuation));

            public void OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);
        }

        /// <summary>
        /// Tween用の非同期処理クラス
        /// </summary>
        private sealed class TweenConfiguredSource : IUniTaskSource, ITaskPoolNode<TweenConfiguredSource>
        {
            private static TaskPool<TweenConfiguredSource> pool;
            private readonly Action onCompleteCallbackDelegate;
            private Tweener tween;
            private bool canceled = false;
            private CancellationToken token;
            private Action originalCompleteAcion;
            private TweenConfiguredSource nextNode;
            private CancellationTokenRegistration cancellationRegistration;
            private UniTaskCompletionSourceCore<AsyncUnit> core;
            public ref TweenConfiguredSource NextNode => ref nextNode;

            static TweenConfiguredSource() => TaskPool.RegisterSizeGetter(typeof(TweenConfiguredSource), () => pool.Size);

            public TweenConfiguredSource() => onCompleteCallbackDelegate = OnCompleteCallbackDelegate;

            public static IUniTaskSource Create(Tweener tween, CancellationToken cancellationToken, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    DoCancelBeforeCreate(tween);
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                    result = new TweenConfiguredSource();

                result.tween = tween;
                result.token = cancellationToken;
                result.canceled = false;
                result.originalCompleteAcion = tween.GetOnComplete();
                tween.OnComplete(result.onCompleteCallbackDelegate);

                if (result.originalCompleteAcion == result.onCompleteCallbackDelegate)
                    result.originalCompleteAcion = null;

                if (cancellationToken.CanBeCanceled)
                    result.cancellationRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(Cancel, result);

                TaskTracker.TrackActiveTask(result, 3);
                token = result.core.Version;
                return result;
            }

            /// <summary>
            /// キャンセル処理
            /// </summary>
            /// <param name="obj"></param>
            private static void Cancel(object obj)
            {
                var source = (TweenConfiguredSource)obj;
                source.canceled = true;
                source.tween.Kill();
            }

            private void OnCompleteCallbackDelegate()
            {
                if (token.IsCancellationRequested)
                    canceled = true;
                if (canceled)
                {
                    core.TrySetCanceled(token);
                    return;
                }
                originalCompleteAcion?.Invoke();
                core.TrySetResult(AsyncUnit.Default);
            }

            private static void DoCancelBeforeCreate(Tweener tween) => tween.Kill();

            public void GetResult(short token)
            {
                try
                {
                    core.GetResult(token);
                }
                finally
                {
                    TryReturn();
                }
            }

            public UniTaskStatus GetStatus(short token) => core.GetStatus(token);
            public UniTaskStatus UnsafeGetStatus() => core.UnsafeGetStatus();
            public void OnCompleted(Action<object> continuation, object state, short token) => core.OnCompleted(continuation, state, token);

            private bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                cancellationRegistration.Dispose();
                RestoreOriginalCallback();

                tween = null;
                token = default;
                originalCompleteAcion = null;
                return pool.TryPush(this);
            }

            private void RestoreOriginalCallback()
            {
                tween.OnComplete(originalCompleteAcion);
            }
        }

        /// <summary>
        /// Tweenのコールバックをプールするクラス
        /// </summary>
        private sealed class PooledTweenCallback
        {
            private static readonly ConcurrentQueue<PooledTweenCallback> pool = new();
            private readonly Action runDelegate;
            private Action continuation;

            public PooledTweenCallback() => runDelegate = Run;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Action Create(Action continuation)
            {
                if (!pool.TryDequeue(out var item))
                    item = new PooledTweenCallback();
                item.continuation = continuation;
                return item.runDelegate;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Run()
            {
                var call = continuation;
                continuation = null;
                if (call != null)
                {
                    pool.Enqueue(this);
                    call.Invoke();
                }
            }
        }
    }
}
#endif