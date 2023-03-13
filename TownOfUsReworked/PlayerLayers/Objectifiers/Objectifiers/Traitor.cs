using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Traitor : Objectifier
    {
        public Role former;
        public string Side;
        public bool Turned = false;
        public string Objective;
        public Faction Side2;
        public bool Betray => PlayerControl.AllPlayerControls.ToArray().Where(x => Turned && x.Is(Side2) && x != Player).Count() == 0;

        public Traitor(PlayerControl player) : base(player)
        {
            Name = "Traitor";
            SymbolName = "♣";
            TaskText = Turned
                ? Objective
                : "- Finish your tasks to switch sides to either <color=#FF0000FF>Intruders</color> or the <color=#008000FF>Syndicate</color>.";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Traitor : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Traitor;
            Hidden = !CustomGameOptions.TraitorKnows && !Turned;
            ObjectifierDescription = "You are a Traitor! You are an indifferent Crew who just wants to be done with this mission, one way or another!" + (Turned
                ? $" You care currently siding with the {Side}."
                : "");
        }

        public void TurnBetrayer()
        {
            var role = Role.GetRole(Player);

            if (role.RoleType == RoleEnum.Betrayer)
                return;

            var betrayer = new Betrayer(Player);
            betrayer.RoleUpdate(role);
            betrayer.Objectives = role.Objectives;
            Player.RegenTask();
        }
    }
}