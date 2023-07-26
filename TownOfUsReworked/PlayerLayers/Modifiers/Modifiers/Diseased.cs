namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Diseased : Modifier
    {
        public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Diseased : Colors.Modifier;
        public override string Name => "Diseased";
        public override LayerEnum Type => LayerEnum.Diseased;
        public override ModifierEnum ModifierType => ModifierEnum.Diseased;
        public override Func<string> TaskText => () => $"- Your killer's cooldown increases by {CustomGameOptions.DiseasedMultiplier} times";

        public Diseased(PlayerControl player) : base(player) => Hidden = !CustomGameOptions.DiseasedKnows && !IsDead;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Hidden && IsDead)
                Hidden = false;
        }
    }
}