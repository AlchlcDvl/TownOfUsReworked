namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Impostor : IUtility
{
    public override LayerEnum Type => LayerEnum.Impostor;
    public override string StartText => "Sabotage And Kill Everyone";
    public override string Description => CommonAbilities;
}