using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using System;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Anarchist : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }

        public Anarchist(PlayerControl player) : base(player)
        {
            Name = "Anarchist";
            Faction = Faction.Syndicate;
            RoleType = RoleEnum.Anarchist;
            StartText = "Imagine Being Boring Anarchist";
            AbilitiesText = "- None.";
            AttributesText = "- None.";
            Color = Colors.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateUtil;
            AlignmentName = "Syndicate (Utility)";
            IntroText = "Cause chaos";
            Results = InspResults.CrewImpAnMurd;
            CoronerDeadReport = "The body has marked down schematics of the place to plot something sinister. They are an Anarchist";
            CoronerKillerReport = "The body seems to have been killed in a very rough manner, like an inexperienced killer. They were killed by an Anarchist!";
            IntroSound = null;
            Base = true;
            Objectives = "- Kill: <color=#FF0000FF>Intruders</color>, <color=#8BFDFD>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>," +
                " <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.";
            RoleDescription = "You are an Anarchist! Your role is the base role for the Syndicate faction. You have no special abilities aside from being able to kill.";
            FactionDescription = "Your faction is the Syndicate! Your faction has low killing power and is instead geared towards delaying the wins of other factions" +
                " and causing some good old chaos. After a certain number of meeting, one of you will recieve the \"Chaos Drive\" which will enhance your powers and " +
                "give you the ability to kill, if you didn't already.";
            AlignmentDescription = "You are a Syndicate (Utility) role! You usually have no special ability and cannot even appear under natural conditions.";
            AddToRoleHistory(RoleType);
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.PossessCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var synTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Syndicate))
                    synTeam.Add(player);
            }
            __instance.teamToShow = synTeam;
        }

        public override void Wins()
        {
            SyndicateWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (Utils.SyndicateWins())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyndicateWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}