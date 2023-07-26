namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Torch : Ability
    {
        public override Color32 Color => ClientGameOptions.CustomAbColors ? Colors.Torch : Colors.Ability;
        public override string Name => "Torch";
        public override LayerEnum Type => LayerEnum.Torch;
        public override AbilityEnum AbilityType => AbilityEnum.Torch;
        public override Func<string> TaskText => () => "- You can see in the dark";

        public Torch(PlayerControl player) : base(player) {}
    }
}