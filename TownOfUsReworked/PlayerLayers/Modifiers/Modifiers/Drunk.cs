namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Drunk : Modifier
    {
        private static float _time;
        public int Modify = -1;

        public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Drunk : Colors.Modifier;
        public override string Name => "Drunk";
        public override LayerEnum Type => LayerEnum.Drunk;
        public override ModifierEnum ModifierType => ModifierEnum.Drunk;
        public override Func<string> TaskText => () => CustomGameOptions.DrunkControlsSwap ? "- Your controls swap over time" : "- Your controls are inverted";

        public Drunk(PlayerControl player) : base(player) => Modify = -1;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (CustomGameOptions.DrunkControlsSwap)
            {
                _time += Time.deltaTime;

                if (_time >= CustomGameOptions.DrunkInterval)
                {
                    _time -= CustomGameOptions.DrunkInterval;
                    Modify *= -1;
                }
            }
        }
    }
}