namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Indomitable : Modifier
    {
        public bool AttemptedGuess;

        public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Indomitable : Colors.Modifier;
        public override string Name => "Indomitable";
        public override LayerEnum Type => LayerEnum.Indomitable;
        public override ModifierEnum ModifierType => ModifierEnum.Indomitable;
        public override Func<string> TaskText => () => "- You cannot be guessed";

        public Indomitable(PlayerControl player) : base(player)
        {
            Hidden = !CustomGameOptions.IndomitableKnows && !AttemptedGuess;
            AttemptedGuess = false;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Hidden && (IsDead || AttemptedGuess))
                Hidden = false;
        }
    }
}