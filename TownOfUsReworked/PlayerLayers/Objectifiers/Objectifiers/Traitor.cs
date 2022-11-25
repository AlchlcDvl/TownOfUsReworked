using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Traitor : Objectifier
    {
        public RoleEnum formerRole;
        public Role former;
        public bool Turned => TasksLeft == 0;
        public Faction Side;

        public Traitor(PlayerControl player) : base(player)
        {
            Name = "Traitor";
            SymbolName = "♣";
            TaskText = () => "You are <color=#8BFDFDFF>Crew</color>, for now, that is....";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Traitor : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Traitor;
            AddToObjectifierHistory(ObjectifierType);
        }
    }
}