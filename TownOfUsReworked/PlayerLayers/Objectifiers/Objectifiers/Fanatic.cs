using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Fanatic : Objectifier
    {
        public Role former;
        public bool Turned = false;
        public string Objective;
        public Faction Side = Faction.Crew;
        public bool Betray => (Side == Faction.Intruder && Utils.LastImp()) || (Side == Faction.Syndicate && Utils.LastSyn());

        public Fanatic(PlayerControl player) : base(player)
        {
            Name = "Fanatic";
            SymbolName = "♠";
            TaskText = Turned
                ? Objective
                : "- Get attacked by either an <color=#FF0000FF>Intruder</color> or a <color=#008000FF>Syndicate</color> to join their side.";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Fanatic : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Fanatic;
            Hidden = !CustomGameOptions.FanaticKnows && !Turned;
        }

        public static void TurnFanatic(PlayerControl fanatic, Faction faction)
        {
            var fanaticRole = Role.GetRole(fanatic);
            var fanatic2 = GetObjectifier<Fanatic>(fanatic);
            fanaticRole.Faction = faction;
            fanatic2.former = fanaticRole;
            fanatic2.Turned = true;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Mystic));

            if (faction == Faction.Syndicate)
            {
                fanatic2.Objective = Role.SyndicateWinCon;
                fanatic2.Color = Colors.Syndicate;
                fanaticRole.IsSynFanatic = true;
                fanaticRole.FactionColor = Colors.Syndicate;
            }
            else if (faction == Faction.Intruder)
            {
                fanatic2.Objective = Role.IntrudersWinCon;
                fanatic2.Color = Colors.Intruder;
                fanaticRole.IsIntFanatic = true;
                fanaticRole.FactionColor = Colors.Intruder;
            }

            fanatic2.Side = faction;
            fanatic.RegenTask();
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