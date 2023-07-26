namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Professional : Modifier
    {
        public bool LifeUsed;

        public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Professional : Colors.Modifier;
        public override string Name => "Professional";
        public override LayerEnum Type => LayerEnum.Professional;
        public override ModifierEnum ModifierType => ModifierEnum.Professional;
        public override Func<string> TaskText => () => "- You have an extra life when assassinating";

        public Professional(PlayerControl player) : base(player)
        {
            Hidden = !CustomGameOptions.ProfessionalKnows && !LifeUsed;
            LifeUsed = false;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Hidden && (IsDead || LifeUsed))
                Hidden = false;
        }
    }
}