using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Allied : Objectifier
    {
        public string Side;

        public Allied(PlayerControl player) : base(player)
        {
            Name = "Allied";
            SymbolName = "ζ";
            TaskText = "You don't feel like a <color=#B3B3B3FF>Neutral</color> today.";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Allied : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Allied;
        }
    }
}