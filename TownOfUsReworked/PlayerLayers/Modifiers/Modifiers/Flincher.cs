namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Flincher : Modifier
    {
        public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Flincher : Colors.Modifier;
        public override string Name => "Flincher";
        public override LayerEnum Type => LayerEnum.Flincher;
        public override ModifierEnum ModifierType => ModifierEnum.Flincher;
        public override Func<string> TaskText => () => "- You are quick to flinch";

        public Flincher(PlayerControl player) : base(player) {}
    }
}