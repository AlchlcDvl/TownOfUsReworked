using System.Linq;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Sidekick : Role
    {
        public Role FormerRole = null;
        public bool CanPromote => PlayerControl.AllPlayerControls.ToArray().ToList().Where(x => x.Is(RoleEnum.Rebel)).Count() == 0;
        public DateTime LastKilled { get; set; }
        private KillButton _killButton;
        public PlayerControl ClosestPlayer = null;

        public Sidekick(PlayerControl player) : base(player)
        {
            Name = "Sidekick";
            Faction = Faction.Syndicate;
            RoleType = RoleEnum.Sidekick;
            StartText = "Succeed The <color=#FFFCCEFF>Rebel</color>";
            AbilitiesText = "- When the <color=#FFFCCEFF>Rebel</color> dies, you will become the new <color=#FFFCCEFF>Rebel</color> with boosted abilities of your former role.";
            AttributesText = "- None.";
            Color = CustomGameOptions.CustomSynColors ? Colors.Sidekick : Colors.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateUtil;
            AlignmentName = "Syndicate (Utility)";
            Results = InspResults.JestMafiSideDamp;
            FactionDescription = "Your faction is the Syndicate! Your faction has low killing power and is instead geared towards delaying the wins of other factions" +
                " and causing some good old chaos. After a certain number of meeting, one of you will recieve the \"Chaos Drive\" which will enhance your powers and " +
                "give you the ability to kill, if you didn't already.";
            Objectives = "- Kill: <color=#FF0000FF>Intruders</color>, <color=#8BFDFD>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>," +
                " <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.";
            AlignmentDescription = "You are a Syndicate (Utility) role! You usually have no special ability and cannot even appear under natural conditions.";
            RoleDescription = "You have become a Sidekick! You are the successor to the leader of the Intruders. When the Rebel dies, you will become the new" +
                " Rebel and will inherit stronger variations of your former role.";
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.ChaosDriveKillCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                AddToExtraButtons(value);
            }
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else
                SyndicateWin = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.SyndicateWins())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            
            return false;
        }

        public void TurnRebel()
        {
            var sidekick = Role.GetRole<Sidekick>(Player);
            var formerRole = sidekick.FormerRole;
            var role = new Rebel(Player);
            role.WasSidekick = true;
            role.HasDeclared = !CustomGameOptions.PromotedSidekickCanPromote;
            role.FormerRole = formerRole;
            role.RoleHistory.Add(sidekick);
            role.RoleHistory.AddRange(sidekick.RoleHistory);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer)
                    role.RegenTask();
            }
        }
    }
}