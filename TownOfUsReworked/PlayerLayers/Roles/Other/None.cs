namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Roleless : Role // In an ideal world, this never gets assigned
{
    public override Faction BaseFaction => Faction.None;
    public override Alignment Alignment => Alignment.None;
}