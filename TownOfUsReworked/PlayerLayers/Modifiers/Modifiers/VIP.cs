namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class VIP : Modifier
    {
        public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.VIP : Colors.Modifier;
        public override string Name => "VIP";
        public override LayerEnum Type => LayerEnum.VIP;
        public override ModifierEnum ModifierType => ModifierEnum.VIP;
        public override Func<string> TaskText => () => "- Your death will alert everyone and will have an arrow pointing at your body";
        public override bool Hidden => !CustomGameOptions.VIPKnows && !IsDead;

        public VIP(PlayerControl player) : base(player) {}
    }
}