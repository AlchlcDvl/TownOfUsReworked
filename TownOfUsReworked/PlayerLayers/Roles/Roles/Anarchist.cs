using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using System;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using Hazel;

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
            StartText = "Wreck Everyone With A Passion";
            Color = IsRecruit ? Colors.Cabal : Colors.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateUtil;
            AlignmentName = "Syndicate (Utility)";
            Results = InspResults.CrewImpAnMurd;
            Base = true;
            Objectives = IsRecruit ? JackalWinCon : SyndicateWinCon;
            RoleDescription = "You are an Anarchist! Your role is the base role for the Syndicate faction. You have no special abilities aside from being able to kill.";
            FactionDescription = SyndicateFactionDescription;
            AlignmentDescription = SUDescription;
            Attack = AttackEnum.Basic;
            AttackString = "Basic";
            AddToRoleHistory(RoleType);
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.ChaosDriveKillCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (!IsRecruit)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                        team.Add(player);
                }
            }
            else
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else
                SyndicateWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CabalWin,
                        SendOption.Reliable, -1);
                    writer.Write(Player.PlayerId);
                    Wins();
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.SyndicateWins())
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