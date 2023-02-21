using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Tunneler : Ability
    {
        public int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        public bool TasksDone => TasksLeft <= 0;

        public Tunneler(PlayerControl player) : base(player)
        {
            Name = "Tunneler";
            TaskText = "You can dig yourself into the ground";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Tunneler : Colors.Ability;
            AbilityType = AbilityEnum.Tunneler;
            Hidden = !CustomGameOptions.TunnelerKnows;
        }
    }
}