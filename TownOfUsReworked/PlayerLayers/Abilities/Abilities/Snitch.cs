namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Snitch : Ability
    {
        public override Color32 Color => ClientGameOptions.CustomAbColors ? Colors.Snitch : Colors.Ability;
        public override string Name => "Snitch";
        public override LayerEnum Type => LayerEnum.Snitch;
        public override AbilityEnum AbilityType => AbilityEnum.Snitch;
        public override Func<string> TaskText => () => "- You can finish your tasks to get information on who's evil";

        public Snitch(PlayerControl player) : base(player) => Hidden = !CustomGameOptions.SnitchKnows && !TasksDone;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Hidden && TasksDone)
                Hidden = false;
        }
    }
}