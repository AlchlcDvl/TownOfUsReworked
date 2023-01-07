using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Overlord : Objectifier
    {
        public bool Turned = false;

        public Overlord(PlayerControl player) : base(player)
        {
            Name = "Overlord";
            SymbolName = "♠";
            TaskText = "You are <color=#8BFDFDFF>Crew</color>, for now that is....";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Overlord : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Overlord;
            AddToObjectifierHistory(ObjectifierType);
        }
    }
}