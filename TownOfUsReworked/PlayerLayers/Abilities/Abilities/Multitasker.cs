namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Multitasker : Ability
    {
        public override Color32 Color => ClientGameOptions.CustomAbColors ? Colors.Multitasker : Colors.Ability;
        public override string Name => "Multitasker";
        public override LayerEnum Type => LayerEnum.Multitasker;
        public override AbilityEnum AbilityType => AbilityEnum.Multitasker;
        public override Func<string> Description => () => "- Your task windows are transparent";

        public Multitasker(PlayerControl player) : base(player) {}
    }
}