namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Snitch : Ability
    {
        public Snitch(PlayerControl player) : base(player)
        {
            Name = "Snitch";
            TaskText = "- You can finish your tasks to get information on who's evil";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Snitch : Colors.Ability;
            AbilityType = AbilityEnum.Snitch;
            Hidden = !CustomGameOptions.SnitchKnows && !TasksDone;
            Type = LayerEnum.Snitch;
        }
    }
}