namespace TownOfUsReworked.PlayerLayers.Objectifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Mafia : Objectifier
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MafiaRoles { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MafVent { get; set; } = false;

    public override UColor Color => ClientOptions.CustomObjColors ? CustomColorManager.Mafia : CustomColorManager.Objectifier;
    public override string Name => "Mafia";
    public override string Symbol => "ω";
    public override LayerEnum Type => LayerEnum.Mafia;
    public override Func<string> Description => () => "- Eliminate anyone who opposes the Mafia";

    public override void Init()
    {
        base.Init();
        Player.GetRole().Alignment = Player.GetRole().Alignment.GetNewAlignment(Faction.Neutral);
    }
}