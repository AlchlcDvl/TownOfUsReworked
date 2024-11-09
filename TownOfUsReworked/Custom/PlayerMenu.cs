namespace TownOfUsReworked.Custom;

public class CustomPlayerMenu(PlayerControl owner, PlayerSelect click, PlayerBodyExclusion exception = null) : CustomMenu(owner, "Player")
{
    public PlayerSelect Click { get; } = click;
    public PlayerBodyExclusion Exception { get; } = exception ?? BlankFalse;

    public List<PlayerControl> Targets() => [ .. AllPlayers().Where(x => !Exception(x) && !x.IsPostmortal() && !x.Data.Disconnected) ];

    public void Clicked(PlayerControl player)
    {
        Click(player);
        Menu.Close();
        IsActive = false;
    }

    public override ISystem.List<UiElement> CreateMenu(ShapeshifterMinigame __instance)
    {
        __instance.potentialVictims = new();
        var list2 = new ISystem.List<UiElement>();
        var targets = Targets();

        for (var i = 0; i < targets.Count; i++)
        {
            var player = targets[i];
            var panel = UObject.Instantiate(__instance.PanelPrefab, __instance.transform);
            panel.SetPlayer(i, player.Data, (Action)(() => Clicked(player)));
            (panel.NameText.text, panel.NameText.color) = PlayerHandler.UpdateGameName(player);
            __instance.potentialVictims.Add(panel);
            list2.Add(panel.Button);
        }

        return list2;
    }
}