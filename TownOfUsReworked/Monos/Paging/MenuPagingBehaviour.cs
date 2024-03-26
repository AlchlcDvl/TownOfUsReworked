namespace TownOfUsReworked.Monos;

public class MenuPagingBehaviour : BasePagingBehaviour
{
    [HideFromIl2Cpp]
    public IEnumerable<ShapeshifterPanel> Targets => Menu.potentialVictims.ToArray();

    public override int MaxPageIndex => (Targets.Count() - 1) / 15;
    private TextMeshPro PageText;
    public ShapeshifterMinigame Menu;

    public override void Start()
    {
        PageText = Instantiate(HUD.KillButton.cooldownTimerText, Menu.transform);
        PageText.name = "MenuPageCount";
        PageText.enableWordWrapping = false;
        PageText.gameObject.SetActive(true);
        PageText.transform.localPosition = new(4.1f, -2.36f, -1f);
        PageText.transform.localScale *= 0.5f;
        OnPageChanged();
    }

    public override void OnPageChanged()
    {
        var i = 0;
        PageText.text = $"({PageIndex + 1}/{MaxPageIndex + 1})";

        foreach (var panel in Targets)
        {
            if (i >= PageIndex * 15 && i < (PageIndex + 1) * 15)
            {
                panel.gameObject.SetActive(true);
                var relativeIndex = i % 15;
                var row = relativeIndex / 3;
                var col = relativeIndex % 3;
                panel.transform.localPosition = new(Menu.XStart + (Menu.XOffset * col), Menu.YStart + (row * Menu.YOffset), panel.transform.localPosition.z);
            }
            else
                panel.gameObject.SetActive(false);

            i++;
        }
    }
}