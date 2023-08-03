namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Insider : Ability
    {
        public override Color32 Color => ClientGameOptions.CustomAbColors ? Colors.Insider : Colors.Ability;
        public override string Name => "Insider";
        public override LayerEnum Type => LayerEnum.Insider;
        public override AbilityEnum AbilityType => AbilityEnum.Insider;
        public override Func<string> Description => () => "- You can finish your tasks to see the votes of others";
        public override bool Hidden => !CustomGameOptions.InsiderKnows && !TasksDone && !IsDead;

        public Insider(PlayerControl player) : base(player) {}
    }
}