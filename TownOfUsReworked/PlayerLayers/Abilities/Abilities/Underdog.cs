namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Underdog : Ability
    {
        public override Color32 Color => ClientGameOptions.CustomAbColors ? Colors.Underdog : Colors.Ability;
        public override string Name => "Underdog";
        public override LayerEnum Type => LayerEnum.Underdog;
        public override AbilityEnum AbilityType => AbilityEnum.Underdog;
        public override Func<string> TaskText => () => Last(Player) ? "- You have shortened cooldowns" : (CustomGameOptions.UnderdogIncreasedKC ? ("- You have long cooldowns until you're "
            + "not alone") : "- You have short cooldowns when you're alone");

        public Underdog(PlayerControl player) : base(player) => Hidden = !CustomGameOptions.UnderdogKnows && !Last(Player);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Hidden && Last(Player))
                Hidden = false;
        }
    }
}