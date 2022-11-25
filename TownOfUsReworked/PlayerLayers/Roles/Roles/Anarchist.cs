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
            ImpostorText = () => "Imagine Being Boring Anarchist";
            TaskText = () => "Imagine Being Boring Anarchist";
            Color = Colors.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateUtil;
            AlignmentName = () => "Syndicate (Utility)";
            IntroText = "Cause choas and kill your opposition";
            Results = InspResults.CrewImpAnMurd;
            SubFaction = SubFaction.None;
            CoronerDeadReport = "The body has marked down schematics of the place to plot something sinister. They are an Anarchist";
            CoronerKillerReport = "The body seems to have been killed in a very rough manner, like an inexperienced killer. They were killed by an Anarchist!";
            IntroSound = null;
            Base = true;
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

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Crew) |
                x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Intruders) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros))) == 0))
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