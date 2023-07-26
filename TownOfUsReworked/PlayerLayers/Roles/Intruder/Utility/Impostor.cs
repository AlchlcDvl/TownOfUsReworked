namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Impostor : Intruder
    {
        public override string Name => "Impostor";
        public override LayerEnum Type => LayerEnum.Impostor;
        public override RoleEnum RoleType => RoleEnum.Impostor;
        public override Func<string> StartText => () => "Sabotage And Kill Everyone";
        public override Func<string> AbilitiesText => () => CommonAbilities;
        public override InspectorResults InspectorResults => InspectorResults.IsBasic;

        public Impostor(PlayerControl player) : base(player) => RoleAlignment = RoleAlignment.IntruderUtil;
    }
}