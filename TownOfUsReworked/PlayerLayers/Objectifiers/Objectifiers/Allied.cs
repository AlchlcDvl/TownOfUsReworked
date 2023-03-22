using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Allied : Objectifier
    {
        public string Side;
        public string Objective;
        public Faction Side2;

        public Allied(PlayerControl player) : base(player)
        {
            Name = "Allied";
            SymbolName = "ζ";
            TaskText = Objective;
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Allied : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Allied;
        }
    }
}