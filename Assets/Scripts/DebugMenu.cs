using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    private const float OpenTapLimitTime = 0.2f;

    private static DebugMenu instance = null;

    public static DebugMenu Instance => instance;

    [SerializeField]
    private PrefabDictionary prefabDictionary;

    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Transform root;
    [SerializeField]
    private bool isShow = true;
    [SerializeField]
    private bool isSingleton = true;
    [SerializeField]
    private TMP_Text titleLabel;
    [SerializeField]
    private Button openButton;
    [SerializeField]
    private Button backButton;

    private DebugPage initialPage;
    private DebugPageModel initialPageModel;
    private DebugInputSystemActions input;

    private int tapCount = 0;
    private float limitTime = 0f;

    private List<DebugPage> pageStack = new();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (isSingleton) DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        input = new DebugInputSystemActions();
        input.Disable();
        input.UI.Enable();
    }

    public DebugPageModel CreateOrGetInitialPageModel(string title = "Initial Page")
    {
        initialPageModel ??= new DebugPageModel(title);
        return initialPageModel;
    }

    private void OpenInitialPage()
    {
        if (pageStack.Count >= 1) return;
        OpenPage(initialPageModel);
    }

    public void OnClickCloseButton()
    {
        canvasGroup.gameObject.SetActive(false);
        openButton.gameObject.SetActive(true);
    }

    public void OnClickBackButton()
    {
        CloseCurrentPage();
    }

    public void OnClickOpenButton()
    {
        tapCount++;
        if (tapCount >= 3)
        {
            openButton.gameObject.SetActive(false);
            canvasGroup.gameObject.SetActive(true);
            OpenInitialPage();
            tapCount = 0;
            return;
        }
        limitTime = OpenTapLimitTime;
    }

    private void FixedUpdate()
    {
        if (limitTime > 0)
            limitTime -= Time.fixedDeltaTime;
        if (limitTime <= 0f)
            tapCount = 0;
    }

    private void Update()
    {
        if (pageStack.Count <= 0) return;
        var menu = pageStack.Last().Menu;
        if (input.UI.Down.WasPressedThisFrame())
            menu.Down();
        else if (input.UI.Up.WasPressedThisFrame())
            menu.Up();
        if (input.UI.Right.WasPressedThisFrame())
            menu.Right();
        else if (input.UI.Left.WasPressedThisFrame())
            menu.Left();

        if (input.UI.Submit.WasPressedThisFrame())
            menu.Submit();
        else if (input.UI.Cancel.WasPressedThisFrame())
            CloseCurrentPage();
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        openButton.gameObject.SetActive(!isShow);
        canvasGroup.gameObject.SetActive(isShow);
    }

    private void OnDestroy()
    {
        input.Dispose();
    }

    public async void OpenPage(DebugPageModel pageModel)
    {
        if (pageStack.Count > 0)
            await pageStack.Last().PushPage();
        var page = prefabDictionary.Instantiate<DebugPage>(root);
        page.OpenPage(pageModel, prefabDictionary);
        pageStack.Add(page);
        titleLabel.text = pageModel.Title;
        backButton.gameObject.SetActive(pageStack.Count > 1);
    }

    protected async void CloseCurrentPage()
    {
        if (pageStack.Count <= 1) return;
        var page = pageStack.Last();
        await page.PushPage();
        pageStack.Remove(page);
        titleLabel.text = await pageStack.Last().PopPageAsync();
        backButton.gameObject.SetActive(pageStack.Count > 1);
        Destroy(page.gameObject);
    }

    protected void CloseAllPage()
    {
        foreach (var page in pageStack)
            Destroy(page);
        pageStack.Clear();
        backButton.gameObject.SetActive(pageStack.Count > 1);
    }
}
