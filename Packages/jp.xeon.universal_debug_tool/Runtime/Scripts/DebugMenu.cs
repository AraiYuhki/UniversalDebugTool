using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xeon.UniversalDebugTool.Model;

namespace Xeon.UniversalDebugTool
{
    public class DebugMenu : MonoBehaviour
    {
        private static DebugMenu instance = null;

        public static DebugMenu Instance => instance;

        [SerializeField]
        private UniversalDebugToolSetting setting;

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
        [SerializeField, Tooltip("Number of taps to open the debug menu")]
        private int openTapCount = 3;
        [SerializeField, Tooltip("The time from the last time the open button was pressed until the tap count is reset")]
        private float tapCountResetTime = 0.2f;
        [SerializeField]
        private KeyboardShortcut keyboardShortcut = new();
        [SerializeField]
        private GamepadShortcut gamepadShortcut = new();

        private DebugPageModel initialPageModel;
        private DebugInputSystemActions input;
        private List<IInputActionCollection> inputsToDisable = new();
        private Dictionary<InputAction, bool> actionsEnableStates = new();

        private int tappedCount = 0;
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
            SceneManager.sceneUnloaded += ClearInputsToDisable;
            if (isSingleton) DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            input = new DebugInputSystemActions();
            input.Disable();
            input.UI.Enable();
        }

        private void ClearInputsToDisable(Scene arg0)
        {
            inputsToDisable.Clear();
            actionsEnableStates.Clear();
        }

        private void DisableInputs()
        {
            foreach (var input in inputsToDisable)
            {
                foreach (var action in input)
                {
                    actionsEnableStates.Add(action, action.enabled);
                    action.Disable();
                }
            }
        }

        private void RestoreInput()
        {
            foreach (var (action, flag) in actionsEnableStates)
            {
                if (flag)
                    action.Enable();
            }
            actionsEnableStates.Clear();
        }

        public void AddInputToDisable(IInputActionCollection input)
        {
            inputsToDisable.Add(input);
        }

        public void RemoveInputToDisable(IInputActionCollection input)
        {
            inputsToDisable.Remove(input);
        }

        public DebugPageModel CreateOrGetInitialPageModel(string title = "Initial Page")
        {
            initialPageModel ??= new DebugPageModel(title);
            return initialPageModel;
        }

        private void OpenInitialPage()
        {
            if (pageStack.Count >= 1) return;
            if (initialPageModel == null)
            {
                Debug.LogError("Initial page's model is not created.\n You must create instance.");
                initialPageModel = CreateOrGetInitialPageModel();
            }
            OpenPage(initialPageModel);
        }

        public void OnClickCloseButton()
        {
            Hide();
        }

        public void OnClickBackButton()
        {
            CloseCurrentPage();
        }

        public void OnClickOpenButton()
        {
            tappedCount++;
            if (tappedCount >= openTapCount)
            {
                Show();
                return;
            }
            limitTime = tapCountResetTime;
        }

        public void Show()
        {
            if (isShow) return;
            isShow = true;
            pageStack.FirstOrDefault()?.Reopen();
            openButton.gameObject.SetActive(false);
            canvasGroup.gameObject.SetActive(true);
            OpenInitialPage();
            DisableInputs();
            tappedCount = 0;
        }

        public void Hide()
        {
            if (!isShow) return;
            isShow = false;
            openButton.gameObject.SetActive(true);
            canvasGroup.gameObject.SetActive(false);
            RestoreInput();
            tappedCount = 0;
        }

        private void FixedUpdate()
        {
            if (limitTime > 0)
                limitTime -= Time.fixedDeltaTime;
            if (limitTime <= 0f)
                tappedCount = 0;
        }

        private void Update()
        {
            if (keyboardShortcut.Judge() || gamepadShortcut.Judge())
            {
                if (!isShow)
                    Show();
                else
                    Hide();
            }
            if (pageStack.Count <= 0 || !isShow) return;
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
            var page = setting.Instantiate<DebugPage>(root);
            page.OpenPage(pageModel, setting);
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
}
