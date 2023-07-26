namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Insider : Ability
    {
        public override Color32 Color => ClientGameOptions.CustomAbColors ? Colors.Insider : Colors.Ability;
        public override string Name => "Insider";
        public override LayerEnum Type => LayerEnum.Insider;
        public override AbilityEnum AbilityType => AbilityEnum.Insider;
        public override Func<string> TaskText => () => "- You can finish your tasks to see the votes of others";

        public Insider(PlayerControl player) : base(player) => Hidden = !CustomGameOptions.InsiderKnows && !TasksDone;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Hidden && TasksDone)
                Hidden = false;
        }
    }
}