namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Tunneler : Ability
    {
        public override Color32 Color => ClientGameOptions.CustomAbColors ? Colors.Tunneler : Colors.Ability;
        public override string Name => "Tunneler";
        public override LayerEnum Type => LayerEnum.Tunneler;
        public override AbilityEnum AbilityType => AbilityEnum.Tunneler;
        public override Func<string> TaskText => () => "- You can finish tasks to be able to vent";

        public Tunneler(PlayerControl player) : base(player) => Hidden = !CustomGameOptions.TunnelerKnows && !TasksDone;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Hidden && TasksDone)
                Hidden = false;
        }
    }
}