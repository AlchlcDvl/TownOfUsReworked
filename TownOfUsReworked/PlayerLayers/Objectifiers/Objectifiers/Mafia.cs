namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Mafia : Objectifier
{
    public override UColor Color => ClientGameOptions.CustomObjColors ? CustomColorManager.Mafia : CustomColorManager.Objectifier;
    public override string Name => "Mafia";
    public override string Symbol => "ω";
    public override LayerEnum Type => LayerEnum.Mafia;
    public override Func<string> Description => () => "- Eliminate anyone who opposes the Mafia";

    public Mafia(PlayerControl player) : base(player) => Role.GetRole(Player).Alignment = Role.GetRole(Player).Alignment.GetNewAlignment(Faction.Neutral);
}