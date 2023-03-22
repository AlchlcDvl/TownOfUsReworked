using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Traitor : Objectifier
    {
        public Role former;
        public bool Turned = false;
        public string Objective;
        public Faction Side = Faction.Crew;
        public bool Betray => Side == Faction.Intruder ? Utils.LastImp() : (Side == Faction.Syndicate ? Utils.LastSyn() : false);

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

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Seer));
        }
    }
}