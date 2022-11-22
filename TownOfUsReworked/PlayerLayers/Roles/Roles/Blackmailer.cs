using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using Il2CppSystem.Collections.Generic;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Blackmailer : Role
    {
        public KillButton _blackmailButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl Blackmailed;
        public DateTime LastBlackmailed { get; set; }
        public bool IntruderWin;

        public Blackmailer(PlayerControl player) : base(player)
        {
            Name = "Blackmailer";
            ImpostorText = () => "You Know Their Dirty Little Secrets";
            TaskText = () => "Use your information to silence players for the next meeting";
            Color = CustomGameOptions.CustomImpColors ? Colors.Blackmailer : Colors.Intruder;
            LastBlackmailed = DateTime.UtcNow;
            RoleType = RoleEnum.Blackmailer;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = () => "Intruder (Concealing)";
            IntroText = "Kill anyone who opposes you";
            CoronerDeadReport = "This body has a ledger containing everyone's secrets! They must be a Blackmailer!";
            CoronerKillerReport = "The hurried and tucked away letter on the body indicates blackmail. They were killed by a Blackmailer!";
            Results = InspResults.SherConsigInspBm;
            SubFaction = SubFaction.None;
            IntroSound = null;
            AddToRoleHistory(RoleType);
        }

        public KillButton BlackmailButton
        {
            get => _blackmailButton;
            set
            {
                _blackmailButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public float BlackmailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlackmailed;
            var num = CustomGameOptions.BlackmailCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var intTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
        }

        public void Wins()
        {
            IntruderWin = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                (x.Is(Faction.Crew) | x.Is(RoleAlignment.NeutralKill) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.IntruderWin,
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