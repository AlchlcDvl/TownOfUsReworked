using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using Hazel;
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
        public bool Betray => PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Side2) && x != Player).Count() == 0;

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
                ? $" You care currently siding with the {Side}"
                : "");
        }

        public void TurnBetrayer()
        {
            var role = Role.GetRole(Player);

            if (role.RoleType == RoleEnum.Betrayer)
                return;

            var betrayer = new Betrayer(Player);
            betrayer.Faction = role.Faction;
            betrayer.SubFaction = role.SubFaction;
            betrayer.FactionColor = role.FactionColor;
            betrayer.RoleHistory.Add(role);
            betrayer.RoleHistory.AddRange(role.RoleHistory);
            betrayer.FactionName = role.FactionName;
            betrayer.Objectives = role.Objectives;
            Player.RegenTask();
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Side == "Intruder" && Utils.IntrudersWin())
            {
                Role.IntruderWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Side == "Syndicate" && Utils.SyndicateWins())
            {
                Role.SyndicateWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return !Turned;
        }
    }
}