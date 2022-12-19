using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class TimeMaster : Role
    {
        private KillButton _freezeButton;
        public bool Enabled;
        public float TimeRemaining;
        public DateTime LastFrozen { get; set; }
        public bool Frozen => TimeRemaining > 0f;

        public TimeMaster(PlayerControl player) : base(player)
        {
            Name = "Time Master";
            StartText = "Freeze Time To Stop The <color=#8BFDFDFF>Crew</color>";
            AbilitiesText = "Freeze time to stop the <color=#8BFDFDFF>Crew</color> from moving";
            Color = CustomGameOptions.CustomImpColors ? Colors.TimeMaster : Colors.Intruder;
            SubFaction = SubFaction.None;
            LastFrozen = DateTime.UtcNow;
            Base = false;
            IsRecruit = false;
            RoleType = RoleEnum.TimeMaster;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            IntroText = "Kill those who oppose you";
            Results = InspResults.TrackAltTLTM;
            IntroSound = null;
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            AddToRoleHistory(RoleType);
        }
        
        public KillButton FreezeButton
        {
            get => _freezeButton;
            set
            {
                _freezeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public float FreezeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFrozen;
            var num = CustomGameOptions.FreezeDuration * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void TimeFreeze()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Freeze.PlayerPhysics_FixedUpdate.FreezeAll();
        }

        public void Unfreeze()
        {
            Enabled = false;
            LastFrozen = DateTime.UtcNow;
            Freeze.PlayerPhysics_FixedUpdate.UnfreezeAll();
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var intTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
        }

        public override void Wins()
        {
            IntruderWin = true;
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
                x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros))) == 0) |
                Utils.Sabotaged())
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