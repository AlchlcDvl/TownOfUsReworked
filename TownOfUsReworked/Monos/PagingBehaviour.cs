namespace TownOfUsReworked.Monos;

public abstract class BasePagingBehaviour : MonoBehaviour
{
    protected BasePagingBehaviour(IntPtr ptr) : base(ptr) {}

    private int _page;
    public virtual int MaxPageIndex => 0;
    public virtual int PageIndex
    {
        get => _page;
        set
        {
            _page = value;
            OnPageChanged();
        }
    }

    public virtual void OnPageChanged() {}

    public virtual void Start() => OnPageChanged();

    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f)
            PageIndex = CycleInt(MaxPageIndex, 0, PageIndex, false);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f)
            PageIndex = CycleInt(MaxPageIndex, 0, PageIndex, true);
    }
}

public class MeetingHudPagingBehaviour : BasePagingBehaviour
{
    public MeetingHudPagingBehaviour(IntPtr ptr) : base(ptr) {}

    [HideFromIl2Cpp]
    public IEnumerable<PlayerVoteArea> Targets => Menu.playerStates.OrderBy(p => p.AmDead);
    public override int MaxPageIndex => (Targets.Count() - 1) / 15;
    public MeetingHud Menu;

    public override void Update()
    {
        base.Update();

        if (Menu.state is MeetingHud.VoteStates.Animating or MeetingHud.VoteStates.Proceeding || Menu.TimerText.text.Contains($" ({PageIndex + 1}/{MaxPageIndex + 1})"))
            return;

        Menu.TimerText.text += $" ({PageIndex + 1}/{MaxPageIndex + 1})";
    }

    public override void OnPageChanged()
    {
        var i = 0;

        foreach (var button in Targets)
        {
            if (i >= PageIndex * 15 && i < (PageIndex + 1) * 15 && !Ability.GetAssassins().Any(x => x.Phone) && !Role.GetRoles<Guesser>(LayerEnum.Guesser).Any(x => x.Phone)
                && !Role.GetRoles<Thief>(LayerEnum.Thief).Any(x => x.Phone) && !Ability.GetAssassins().Any(x => x.Phone))
            {
                button.gameObject.SetActive(true);
                var relativeIndex = i % 15;
                var row = relativeIndex / 3;
                var col = relativeIndex % 3;
                button.transform.localPosition = Menu.VoteOrigin + new Vector3(Menu.VoteButtonOffsets.x * col, Menu.VoteButtonOffsets.y * row, button.transform.localPosition.z);
            }
            else
                button.gameObject.SetActive(false);

            i++;
        }
    }
}

public class VitalsPagingBehaviour : BasePagingBehaviour
{
    public VitalsPagingBehaviour(IntPtr ptr) : base(ptr) {}

    [HideFromIl2Cpp]
    public IEnumerable<VitalsPanel> Targets => Menu.vitals.ToArray();
    public override int MaxPageIndex => (Targets.Count() - 1) / 15;
    private TextMeshPro PageText;
    public VitalsMinigame Menu;

    public override void Start()
    {
        PageText = Instantiate(HUD.KillButton.cooldownTimerText, Menu.transform);
        PageText.name = "MenuPageCount";
        PageText.enableWordWrapping = false;
        PageText.gameObject.SetActive(true);
        PageText.transform.localPosition = new(2.7f, -2f, -1f);
        PageText.transform.localScale *= 0.5f;
        OnPageChanged();
    }

    public override void OnPageChanged()
    {
        if (PlayerTask.PlayerHasTaskOfType<HudOverrideTask>(CustomPlayer.Local))
            return;

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
                panel.transform.localPosition = new(Menu.XStart + (Menu.XOffset * col), Menu.YStart + (Menu.YOffset * row), panel.transform.position.z);
            }
            else
                panel.gameObject.SetActive(false);

            i++;
        }
    }
}

public class MenuPagingBehaviour : BasePagingBehaviour
{
    public MenuPagingBehaviour(IntPtr ptr) : base(ptr) {}

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