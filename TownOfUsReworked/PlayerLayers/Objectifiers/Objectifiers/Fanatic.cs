using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Fanatic : Objectifier
    {
        public Role former;
        public bool Turned;
        public string Side;

        public Fanatic(PlayerControl player) : base(player)
        {
            Name = "Fanatic";
            SymbolName = "♠";
            TaskText = Turned
                ? "You now side with the <color=#" + Role.GetRole(Player).ColorString + $"{Side}</color>!"
                : "You are <color=#8BFDFDFF>Crew</color>, for now, that is....";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Fanatic : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Fanatic;
            AddToObjectifierHistory(ObjectifierType);
        }
    }
}