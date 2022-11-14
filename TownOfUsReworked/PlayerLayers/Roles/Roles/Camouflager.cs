using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Camouflager : Role
    {
        public KillButton _camouflageButton;
        public bool Enabled;
        public DateTime LastCamouflaged { get; set; }
        public float TimeRemaining;
        public bool Camouflaged => TimeRemaining > 0f;

        public Camouflager(PlayerControl player) : base(player)
        {
            Name = "Camouflager";
            ImpostorText = () => "Throw paint everywhere and blend in with the <color=#8BFDFDFF>Crew</color>";
            TaskText = () => "Camouflage among everyone and kill in front of them";
            Color = CustomGameOptions.CustomImpColors ? Colors.Camouflager : Colors.Intruder;
            RoleType = RoleEnum.Camouflager;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = () => "Intruder (Concealing)";
            IntroText = "Kill those who oppose you";
            CoronerDeadReport = $"The camouflage suit indicate that this body is a Camouflager!";
            CoronerKillerReport = "There are marks of grey paint on the body. They were killed by a Camouflager!";
            Results = InspResults.DisgMorphCamoAgent;
            SubFaction = SubFaction.None;
            AddToRoleHistory(RoleType);
        }

        public KillButton CamouflageButton
        {
            get => _camouflageButton;
            set
            {
                _camouflageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Camouflage()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Camouflage();
        }

        public void UnCamouflage()
        {
            Enabled = false;
            LastCamouflaged = DateTime.UtcNow;
            Utils.UnCamouflage();
        }

        public float CamouflageTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCamouflaged;
            var num = CustomGameOptions.CamouflagerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
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