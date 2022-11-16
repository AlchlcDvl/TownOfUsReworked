using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Traitor : Objectifier
    {
        public RoleEnum formerRole = new RoleEnum();
        public Role former;

        public Traitor(PlayerControl player) : base(player)
        {
            Name = "Traitor";
            SymbolName = "♣";
            TaskText = () => "You are <color=#8BFDFDFF>Crew</color>, for now that is....";
            if (CustomGameOptions.CustomImpColors) Color = Colors.Traitor;
            else Color = Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Traitor;
            AddToObjectifierHistory(ObjectifierType);
        }
    }
}