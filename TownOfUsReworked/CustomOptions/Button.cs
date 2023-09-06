namespace TownOfUsReworked.CustomOptions;

public class CustomButtonOption : CustomOption
{
    public Action Do { get; set; }

    public CustomButtonOption(MultiMenu menu, string name, Action toDo = null) : base(-1, menu, name, CustomOptionType.Button, 0) => Do = toDo ?? BaseToDo;

    private static void BaseToDo() {}

    public override void OptionCreated()
    {
        base.OptionCreated();
        Setting.Cast<ToggleOption>().TitleText.text = Name;
    }

    public static ToggleOption CreateButton()
    {
        var togglePrefab = UObject.FindObjectOfType<ToggleOption>();
        var toggle = UObject.Instantiate(togglePrefab, togglePrefab.transform.parent);
        toggle.transform.GetChild(0).localPosition = new(-1.05f, 0f, 0f);
        toggle.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new(5.5f, 0.37f);
        toggle.transform.GetChild(1).localScale = new(1.6f, 1f, 1f);
        toggle.transform.GetChild(2).gameObject.SetActive(false);
        toggle.gameObject.GetComponent<BoxCollider2D>().size = new(7.91f, 0.45f);
        return toggle;
    }
}