using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Fanatic : Objectifier
    {
        public RoleEnum formerRole = new RoleEnum();
        public Role former;
        public bool Turned = false;

        public Fanatic(PlayerControl player) : base(player)
        {
            Name = "Fanatic";
            SymbolName = "«";
            TaskText = "You are <color=#8BFDFDFF>Crew</color>, for now that is....";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Fanatic : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Fanatic;
            AddToObjectifierHistory(ObjectifierType);
        }
    }
}