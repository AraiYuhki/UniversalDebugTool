using UnityEngine;
using Xeon.UniversalDebugTool;
using Xeon.UniversalDebugTool.Model;

public class DebugController : MonoBehaviour
{
    private enum TestEnum
    {
        One,
        Two,
        Three,
        Four,
    }

    [SerializeField]
    private DebugMenu menu;

    [SerializeField]
    private int intValueTest = 0;
    [SerializeField]
    private float floatValueTest = 0f;
    [SerializeField]
    private TestEnum testEnum = TestEnum.One;
    [SerializeField]
    private int pickerIndex = 0;
    [SerializeField]
    private string selectedPickerValue = string.Empty;
    [SerializeField]
    private bool toggleTest = false;
    [SerializeField]
    private string inputFieldTest;


    private DebugSliderModel intSliderModel, floatSliderModel;
    private DebugPickerModel<string> testPickerModel;
    private DebugEnumPickerModel<TestEnum> testEnumPickerModel;
    private DebugToggleModel testToggleModel;
    private DebugInputModel testInputModel;

    private void Start()
    {
        var initialPage = menu.CreateOrGetInitialPageModel();

        intSliderModel = new DebugSliderModel("Debug int slider", -10, 10, intValueTest, value => intValueTest = value, 2);
        floatSliderModel = new DebugSliderModel("Debug float slider", -10.5f, 10.5f, floatValueTest, value => floatValueTest = value, 0.1f);
        selectedPickerValue = "2nd";
        testPickerModel = new DebugPickerModel<string>("String Picker", new() { "First", "Second", "Third", "Fourth" }, new() { "1st", "2nd", "3rd", "4th" }, selectedPickerValue, value =>
        {
            selectedPickerValue = value;
            pickerIndex = testPickerModel.SelectedIndex;
        });
        testEnumPickerModel = new DebugEnumPickerModel<TestEnum>("Enum picker", testEnum, value => testEnum = value);
        testToggleModel = new DebugToggleModel("Toggle Test", toggleTest, value => toggleTest = value);
        testInputModel = new DebugInputModel("Test input", inputFieldTest, value => inputFieldTest = value);

        initialPage.AddSlider(floatSliderModel);
        initialPage.AddSlider(intSliderModel);
        initialPage.AddPicker(testPickerModel);
        initialPage.AddEnumPicker(testEnumPickerModel);
        initialPage.AddToggle(testToggleModel);
        initialPage.AddInputField(testInputModel);

        initialPage.AddButton("Open dialog", async () =>
        {
            var result = await menu.OpenYesNoDialog("Test", "Test message");
            if (result)
            {
                await menu.OpenConfirmDialog("Confirm", "Prease press OK button");
            }
        });

        initialPage.AddPageLinkButton<GraphyDebugPageModel>("Graphy");
        initialPage.AddPageLinkButton<InGameDebugConsolePageModel>("In game debug console");

        initialPage.AddLabel("Other pages");

        for (var count = 0; count < 4; count++)
        {
            var pageName = $"Page{count}";
            initialPage.AddPageLinkButton(CreateOtherPage(pageName, 4 * count, 4), pageName);
        }
    }

    private DebugPageModel CreateOtherPage(string title, int startIndex, int bottonCount)
    {
        var model = new DebugPageModel(title);
        for (var count = 0; count < bottonCount; count++)
        {
            var index = count + startIndex;
            model.AddButton($"Button{index}", () => Debug.Log(index));
        }
        return model;
    }
}
