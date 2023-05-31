namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Drunk : Modifier
    {
        private static float _time;
        public int Modify = -1;

        public Drunk(PlayerControl player) : base(player)
        {
            Name = "Drunk";
            TaskText = () => CustomGameOptions.DrunkControlsSwap ? "- Your controls swap over time" : "- Your controls are inverted";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Drunk : Colors.Modifier;
            ModifierType = ModifierEnum.Drunk;
            Type = LayerEnum.Drunk;
            Modify = -1;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (CustomGameOptions.DrunkControlsSwap)
            {
                _time += Time.deltaTime;

                if (_time > CustomGameOptions.DrunkInterval)
                {
                    _time -= CustomGameOptions.DrunkInterval;
                    Modify *= -1;
                }
            }
        }
    }
}