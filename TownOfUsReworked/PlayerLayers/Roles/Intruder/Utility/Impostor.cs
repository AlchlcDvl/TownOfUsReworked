namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Impostor : IUtility
{
    public override Layer Type => Layer.Impostor;
    public override string StartText => "Sabotage And Kill Everyone";
    public override string Description => CommonAbilities;
}