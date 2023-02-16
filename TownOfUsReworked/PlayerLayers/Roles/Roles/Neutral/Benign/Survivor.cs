using System;
using UnityEngine;
using TMPro;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Survivor : Role
    {
        public bool Enabled;
        public DateTime LastVested;
        public float TimeRemaining;
        public int UsesLeft;
        public TextMeshPro UsesText;
        public bool ButtonUsable => UsesLeft != 0;
        public bool Vesting => TimeRemaining > 0f;
        public bool Alive => !Player.Data.Disconnected && !Player.Data.IsDead;
        private KillButton _vestButton;

        public Survivor(PlayerControl player) : base(player)
        {
            Name = "Survivor";
            StartText = "Do Whatever It Takes To Live";
            AbilitiesText = "Stay alive to win";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Survivor : Colors.Neutral;
            LastVested = DateTime.UtcNow;
            RoleType = RoleEnum.Survivor;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            UsesLeft = CustomGameOptions.MaxVests;
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = "Neutral (Benign)";
            Objectives = "Stay alive";
        }

        public KillButton VestButton
        {
            get => _vestButton;
            set
            {
                _vestButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float VestTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastVested;
            var num = CustomGameOptions.VestCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Vest()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void UnVest()
        {
            Enabled = false;
            LastVested = DateTime.UtcNow;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else if (IsIntAlly)
                IntruderWin = true;
            else if (IsSynAlly)
                SyndicateWin = true;
            else if (IsCrewAlly)
                CrewWin = true;
            else if (IsPersuaded)
                SectWin = true;
            else if (IsBitten)
                UndeadWin = true;
            else if (IsResurrected)
                ReanimatedWin = true;
            else if (CustomGameOptions.NoSolo == NoSolo.AllNeutrals)
                AllNeutralsWin = true;
        }
    }
}