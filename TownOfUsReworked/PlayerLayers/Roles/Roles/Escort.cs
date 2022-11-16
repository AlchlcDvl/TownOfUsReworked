using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Lobby.CustomOption;
using System;
using TownOfUsReworked.Extensions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Escort : Role
    {
        public bool EscWin;
        public PlayerControl ClosestPlayer;
        public DateTime LastBlock { get; set; }
        public float TimeRemaining;

        public Escort(PlayerControl player) : base(player)
        {
            Name = "Escort";
            Faction = Faction.Crew;
            RoleType = RoleEnum.Escort;
            ImpostorText = () => "Roleblock Players And Stop Them From Harming Others";
            TaskText = () => "Block people from using their abilities";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Escort : Colors.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = () => "Crew (Support)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.EscConsGliPois;
            SubFaction = SubFaction.None;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public float RoleblockTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlock;
            var num = CustomGameOptions.MimicCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Roleblock()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Block(Player, ClosestPlayer);

            if (Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void Unroleblock()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Block(Player, ClosestPlayer);

            if (Player.Data.IsDead)
                TimeRemaining = 0f;
        }
    }
}