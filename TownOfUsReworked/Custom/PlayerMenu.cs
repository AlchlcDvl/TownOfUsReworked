namespace TownOfUsReworked.Custom;

public class CustomPlayerMenu : CustomMenu
{
    public PlayerSelect Click { get; }
    public PlayerMultiSelect MultiClick { get; }
    public PlayerBodyExclusion Exception { get; }
    public MenuType SelectType { get; }
    public int MaxSelected { get; }
    public Dictionary<byte, ShapeshifterPanel> PlayerToPanel { get; } = [];
    public List<byte> Selected { get; } = [];

    public PlayerControl[] Targets() => [ .. AllPlayers().Where(x => !Exception(x) && !x.IsPostmortal() && !x.Data.Disconnected) ];

    private CustomPlayerMenu(PlayerControl owner, MenuType type, PlayerBodyExclusion exception) : base(owner, "Player")
    {
        Exception = exception ?? BlankFalse;
        SelectType = type;
    }

    public CustomPlayerMenu(PlayerControl owner, PlayerMultiSelect multiClick, PlayerBodyExclusion exception = null, int max = 2) : this(owner, MenuType.MultiSelect, exception)
    {
        Click = BlankVoid;
        MultiClick = multiClick ?? BlankFalse;
        MaxSelected = max;
    }

    public CustomPlayerMenu(PlayerControl owner, PlayerSelect click, PlayerBodyExclusion exception = null) : this(owner, MenuType.Single, exception)
    {
        Click = click ?? BlankVoid;
        MultiClick = BlankFalse;
        MaxSelected = 1;
    }

    public void Clicked(PlayerControl player)
    {
        var shouldClose = false;

        if (Selected.Contains(player.PlayerId))
            Selected.RemoveAll(x => x == player.PlayerId);
        else if (SelectType == MenuType.MultiSelect && MultiClick(player, out shouldClose))
            Selected.Add(player.PlayerId);
        else
            Click(player);

        while (Selected.Count > MaxSelected)
            Selected.TakeFirst();

        PlayerToPanel.ForEach((x, y) => y.Background.color = Selected.Contains(x) ? UColor.red : UColor.white);

        if (SelectType == MenuType.Single || shouldClose || Selected.Count >= MaxSelected)
        {
            Menu.Close();
            IsActive = false;
        }
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
            panel.Background.color = Selected.Contains(player.PlayerId) ? UColor.red : UColor.white;
            (panel.NameText.text, panel.NameText.color) = (player.NameText().text, player.NameText().color);
            __instance.potentialVictims.Add(panel);
            list2.Add(panel.Button);
            PlayerToPanel[player.PlayerId] = panel;
        }

        return list2;
    }
}