using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class TimeMaster : Role
    {
        public KillButton _freezeButton;
        public bool Enabled;
        public float TimeRemaining;
        public DateTime LastFrozen { get; set; }
        public bool Frozen => TimeRemaining > 0f;

        public TimeMaster(PlayerControl player) : base(player)
        {
            Name = "Time Master";
            ImpostorText = () => "Freeze Time To Stop The <color=#8BFDFDFF>Crew</color>";
            TaskText = () => "Freeze time to stop the <color=#8BFDFDFF>Crew</color> from moving";
            Color = CustomGameOptions.CustomImpColors ? Colors.TimeMaster : Colors.Intruder;
            SubFaction = SubFaction.None;
            LastFrozen = DateTime.UtcNow;
            RoleType = RoleEnum.TimeMaster;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = () => "Intruder (Support)";
            IntroText = "Kill those who oppose you";
            Results = InspResults.TrackAltTLTM;
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

        public System.Collections.Generic.List<PlayerControl> Freeze()
        {
            var FrozenPlayers = new System.Collections.Generic.List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!(player.Data.IsDead | player.Data.Disconnected | (player.Is(RoleEnum.TimeLord) && CustomGameOptions.TLImmunity) |
                    player.Is(Faction.Intruders)))
                    FrozenPlayers.Add(player);
            }

            return FrozenPlayers;
        }

        public void TimeFreeze()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void Unfreeze()
        {
            Enabled = false;
            LastFrozen = DateTime.UtcNow;
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
    }
}