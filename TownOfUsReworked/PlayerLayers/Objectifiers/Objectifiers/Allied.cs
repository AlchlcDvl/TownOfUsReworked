using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Allied : Objectifier
    {
        public string Objective = "- None";
        public Faction Side = Faction.Neutral;

        public Allied(PlayerControl player) : base(player)
        {
            Name = "Allied";
            SymbolName = "ζ";
            TaskText = Objective;
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Allied : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Allied;
            Type = LayerEnum.Allied;
        }
    }
}