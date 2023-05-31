namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Coward : Modifier
    {
        public Coward(PlayerControl player) : base(player)
        {
            Name = "Coward";
            TaskText = () => "- You can't report bodies";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Coward : Colors.Modifier;
            ModifierType = ModifierEnum.Coward;
            Type = LayerEnum.Coward;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            __instance.ReportButton.SetActive(false);
        }
    }
}