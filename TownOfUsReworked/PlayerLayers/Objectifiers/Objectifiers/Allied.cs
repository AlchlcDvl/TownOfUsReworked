using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Allied : Objectifier
    {
        public Role former;
        public bool Turned;
        public string Side;

        public Allied(PlayerControl player) : base(player)
        {
            Name = "Allied";
            SymbolName = "♣";
            TaskText = Turned
                ? "You now side with the <color=#" + Role.GetRole(Player).ColorString + $"{Side}</color>!"
                : "You are <color=#8BFDFDFF>Crew</color>, for now that is....";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Traitor : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Traitor;
            Hidden = !CustomGameOptions.TraitorKnows;
        }
    }
}