namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Ninja : Ability
    {
        public override Color32 Color => ClientGameOptions.CustomAbColors ? Colors.Ninja : Colors.Ability;
        public override string Name => "Ninja";
        public override LayerEnum Type => LayerEnum.Ninja;
        public override AbilityEnum AbilityType => AbilityEnum.Ninja;
        public override Func<string> TaskText => () => "- You do not lunge when killing";

        public Ninja(PlayerControl player) : base(player) {}
    }
}