using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Fanatic : Objectifier
    {
        public Role former;
        public bool Turned;
        public string Objective;
        public Faction Side = Faction.Crew;
        public bool Betray => (Side == Faction.Intruder && ConstantVariables.LastImp) || (Side == Faction.Syndicate && ConstantVariables.LastSyn);

        public Fanatic(PlayerControl player) : base(player)
        {
            Name = "Fanatic";
            SymbolName = "♠";
            TaskText = "- Get attacked by either an <color=#FF0000FF>Intruder</color> or a <color=#008000FF>Syndicate</color> to join their side";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Fanatic : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Fanatic;
            Hidden = !CustomGameOptions.FanaticKnows && !Turned;
            Type = LayerEnum.Fanatic;
        }

        public static void TurnFanatic(PlayerControl fanatic, Faction faction)
        {
            var fanaticRole = Role.GetRole(fanatic);
            var fanatic2 = GetObjectifier<Fanatic>(fanatic);
            fanaticRole.Faction = faction;
            fanatic2.former = fanaticRole;
            fanatic2.Turned = true;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                Utils.Flash(Colors.Mystic);

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
            fanatic2.TaskText = "";
            fanatic.RegenTask();
        }

        public void TurnBetrayer()
        {
            var role = Role.GetRole(Player);

            if (role.RoleType == RoleEnum.Betrayer)
                return;

            var betrayer = new Betrayer(Player) { Objectives = role.Objectives };
            betrayer.RoleUpdate(role);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Betray && Turned)
            {
                TurnBetrayer();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnFanaticBetrayer);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}