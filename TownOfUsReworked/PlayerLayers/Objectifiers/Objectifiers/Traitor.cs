using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Traitor : Objectifier
    {
        public Role former;
        public bool Turned;
        public string Objective;
        public Faction Side = Faction.Crew;
        public bool Betray => (Side == Faction.Intruder && Utils.LastImp()) || (Side == Faction.Syndicate && Utils.LastSyn());

        public Traitor(PlayerControl player) : base(player)
        {
            Name = "Traitor";
            SymbolName = "♣";
            TaskText = "- Finish your tasks to switch sides to either <color=#FF0000FF>Intruders</color> or the <color=#008000FF>Syndicate</color>.";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Traitor : Colors.Objectifier;
            Type = ObjectifierEnum.Traitor;
            Hidden = !CustomGameOptions.TraitorKnows && !Turned;
        }

        public void TurnBetrayer()
        {
            var role = Role.GetRole(Player);

            if (role.Type == RoleEnum.Betrayer)
                return;

            var betrayer = new Betrayer(Player);
            betrayer.RoleUpdate(role);
            betrayer.Objectives = role.Objectives;
            Player.RegenTask();

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "Someone changed their identity!");
        }
    }
}