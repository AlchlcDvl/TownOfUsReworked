namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Mafia : Objectifier
    {
        public override Color32 Color => ClientGameOptions.CustomObjColors ? Colors.Mafia : Colors.Objectifier;
        public override string Name => "Mafia";
        public override string Symbol => "ω";
        public override LayerEnum Type => LayerEnum.Mafia;
        public override ObjectifierEnum ObjectifierType => ObjectifierEnum.Mafia;
        public override Func<string> Description => () => "- Eliminate anyone who opposes the Mafia";

        public Mafia(PlayerControl player) : base(player) => Role.GetRole(Player).RoleAlignment = Role.GetRole(Player).RoleAlignment.GetNewAlignment(Faction.Neutral);
    }
}