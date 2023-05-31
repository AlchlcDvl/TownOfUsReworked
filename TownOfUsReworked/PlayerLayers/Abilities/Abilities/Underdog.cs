namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Underdog : Ability
    {
        public Underdog(PlayerControl player) : base(player)
        {
            Name = "Underdog";
            TaskText = () => LayerExtentions.Last(Player)
                ? "- You have shortened cooldowns"
                : (CustomGameOptions.UnderdogIncreasedKC
                    ? "- You have long cooldowns until you're not alone"
                    : "- You have short cooldowns when you're alone");
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Underdog : Colors.Ability;
            AbilityType = AbilityEnum.Underdog;
            Hidden = !CustomGameOptions.UnderdogKnows && !LayerExtentions.Last(Player);
            Type = LayerEnum.Underdog;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Hidden && LayerExtentions.Last(Player))
                Hidden = false;
        }
    }
}