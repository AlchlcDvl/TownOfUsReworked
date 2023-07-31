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
        public override bool Hidden => !CustomGameOptions.TraitorKnows && !LifeUsed && !IsDead;

        public Professional(PlayerControl player) : base(player) => LifeUsed = false;
    }
}