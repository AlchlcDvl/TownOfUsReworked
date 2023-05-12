namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Insider : Ability
    {
        public Insider(PlayerControl player) : base(player)
        {
            Name = "Insider";
            TaskText = "- You can finish your tasks to see the votes of others";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Insider : Colors.Ability;
            AbilityType = AbilityEnum.Insider;
            Hidden = !CustomGameOptions.InsiderKnows && !TasksDone;
            Type = LayerEnum.Insider;
        }
    }
}