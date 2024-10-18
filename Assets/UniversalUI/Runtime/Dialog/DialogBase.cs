using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xeon.XTween;

public enum DialogType
{
    Unknown,
    Common,
    Config,
    MainMenu,
    Inventory,
    Reinforce,
    FunctionShop,
    ParameterAdjust,
}

public class DialogBase : MonoBehaviour, IControllable
{
    private static readonly int OpenId = Animator.StringToHash("Open");
    private static readonly int CloseId = Animator.StringToHash("Close");

    [SerializeField]
    protected Image window;
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected CanvasGroup canvasGroup;
    [SerializeField]
    private TMP_Text title;

    public virtual DialogType Type => throw new NotImplementedException();

    protected int currentSelected = 0;

    public Action OnDestroyed { get; set; }
    protected bool lockInput = true;
    protected Sequence tween;
    public string Title
    {
        get => title.text;
        set => title.text = value;
    }

    public virtual async UniTask OpenAsync(CancellationToken token)
    {
        gameObject.SetActive(true);
        Initialize();
        await animator.PlayAsync(OpenId, token: token);
        OnOpened();
        lockInput = false;
    }

    protected virtual void Initialize()
    {
    }

    protected virtual void OnOpened() { }

    public virtual async UniTask CloseAsync(CancellationToken token)
    {
        lockInput = true;
        await animator.PlayAsync(CloseId, token: token);
    }

    public virtual void Control()
    {
        if (lockInput) return;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Right();
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            Left();
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Up();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            Down();
        if (Input.GetKeyDown(KeyCode.Return))
            Submit();
        else if (Input.GetKeyDown(KeyCode.Tab))
            Cancel();
    }

    public virtual void Left() { }
    public virtual void Right() { }
    public virtual void Up() { }
    public virtual void Down() { }
    public virtual void Submit() { }
    public virtual void Cancel() { }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
        tween?.Kill();
        tween = null;
    }
}
