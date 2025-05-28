namespace TownOfUsReworked.Custom;

public sealed class CustomPlayerMenu : CustomMenu
{
    public List<byte> Selected { get; } = [];
    private PlayerSelect Click { get; }
    private PlayerMultiSelect MultiClick { get; }
    private PlayerBodyExclusion Exception { get; }
    private PlayerMenuType SelectType { get; }
    private int MaxSelected { get; }
    private UColor SelectionColor { get; }
    private Dictionary<byte, ShapeshifterPanel> PlayerToPanel { get; } = [];

    private CustomPlayerMenu(PlayerControl owner, PlayerMenuType type, PlayerBodyExclusion exception, UColor selectionColor) : base(owner, MenuType.Player)
    {
        Exception = exception ?? BlankFalse;
        SelectionColor = selectionColor;
        SelectType = type;
    }

    public CustomPlayerMenu(PlayerControl owner, PlayerMultiSelect multiClick, UColor selectionColor, PlayerBodyExclusion exception = null, int max = 2) : this(owner, PlayerMenuType.MultiSelect,
        exception, selectionColor)
    {
        Click = BlankVoid;
        MultiClick = multiClick ?? BlankFalse;
        MaxSelected = max;
    }

    public CustomPlayerMenu(PlayerControl owner, PlayerSelect click, UColor selectionColor, PlayerBodyExclusion exception = null) : this(owner, PlayerMenuType.Single, exception, selectionColor)
    {
        Click = click ?? BlankVoid;
        MultiClick = BlankFalse;
        MaxSelected = 1;
    }

    private PlayerControl[] Targets() => [ .. AllPlayers().Where(x => !Exception(x) && !x.Is<IGhosty>() && !x.Data.Disconnected) ];

    private void Clicked(PlayerControl player)
    {
        var shouldClose = false;

        if (Selected.Contains(player.PlayerId))
            Selected.RemoveAll(x => x == player.PlayerId);
        else if (SelectType == PlayerMenuType.MultiSelect && MultiClick(player, out shouldClose))
            Selected.Add(player.PlayerId);
        else
            Click(player);

        while (Selected.Count > MaxSelected)
            Selected.TakeFirst();

        PlayerToPanel.ForEach((x, y) => y.Background.color = Selected.Contains(x) ? SelectionColor : UColor.white);

        if (SelectType != PlayerMenuType.Single && !shouldClose && Selected.Count < MaxSelected)
            return;

        Menu.Close();
        IsActive = false;
    }

    public override ISystem.List<UiElement> CreateMenu(ShapeshifterMinigame __instance)
    {
        __instance.potentialVictims = new();
        var list2 = new ISystem.List<UiElement>();
        var targets = Targets();

        for (var i = 0; i < targets.Length; i++)
        {
            var player = targets[i];
            var panel = UObject.Instantiate(__instance.PanelPrefab, __instance.transform);
            panel.SetPlayer(i, player.Data, (Action)(() => Clicked(player)));
            panel.Background.color = Selected.Contains(player.PlayerId) ? SelectionColor : UColor.white;
            var button = panel.transform.GetChild(5).GetComponent<PassiveButton>();
            var highlight = button.transform.GetChild(0);
            var rend = highlight.GetComponent<SpriteRenderer>();
            highlight.GetChild(0).gameObject.SetActive(false); // TODO: Implement role icons into this whenever possible
            button.OverrideOnMouseOverListeners(() => rend.color = SelectionColor);
            button.OverrideOnMouseOutListeners(() => rend.color = UColor.clear);
            (panel.NameText.text, panel.NameText.color) = (player.NameText().text, player.NameText().color);
            __instance.potentialVictims.Add(panel);
            list2.Add(panel.Button);
            PlayerToPanel[player.PlayerId] = panel;
        }

        return list2;
    }
}