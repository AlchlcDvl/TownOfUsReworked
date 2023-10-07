namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Colorblind : Modifier
{
    public override Color Color => ClientGameOptions.CustomModColors ? Colors.Colorblind : Colors.Modifier;
    public override string Name => "Colorblind";
    public override LayerEnum Type => LayerEnum.Colorblind;
    public override Func<string> Description => () => "- You can't tell the difference between players";

    public Colorblind(PlayerControl player) : base(player)
    {
        if (Local)
            ColorAll();
    }

    public override void OnLobby()
    {
        base.OnLobby();
        AllToNormal();
    }

    public override void ExitingLayer() => AllToNormal();

    public override void EnteringLayer() => ColorAll();

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (!IsDead)
            ColorAll();
    }

    public override void OnMeetingEnd(MeetingHud __instance)
    {
        base.OnMeetingEnd(__instance);

        if (IsDead)
            AllToNormal();
    }

    private static void ColorChar(PlayerControl player)
    {
        var fit = player.GetCustomOutfitType();

        if (fit is not (CustomPlayerOutfitType.Colorblind or CustomPlayerOutfitType.Invis or CustomPlayerOutfitType.PlayerNameOnly))
        {
            player.SetOutfit(CustomPlayerOutfitType.Colorblind, ColorblindOutfit(player));
            player.cosmetics.SetBodyColor(15);
            player.MyRend().color = Color.grey;
            player.NameText().color = Color.clear;
            player.cosmetics.colorBlindText.color = Color.clear;
        }
    }

    private static void AllToNormal()
    {
        foreach (var p in CustomPlayer.AllPlayers)
        {
            DefaultOutfit(p);
            p.MyRend().color = Color.white;
        }
    }

    private void ColorAll()
    {
        foreach (var player in CustomPlayer.AllPlayers)
        {
            if (player == Player)
                continue;

            ColorChar(player);
        }
    }
}