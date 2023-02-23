using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using TMPro;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class BountyHunter : Role
    {
        public PlayerControl TargetPlayer = null;
        public PlayerControl ClosestPlayer;
        public bool TargetKilled;
        public bool ColorHintGiven;
        public bool LetterHintGiven;
        public bool TargetFound;
        public DateTime LastChecked;
        private KillButton _guessButton;
        public int UsesLeft;
        public TextMeshPro UsesText;

        public BountyHunter(PlayerControl player) : base(player)
        {
            Name = "Bounty Hunter";
            StartText = "Find And Kill Your Target";
            Objectives = "- Find And Kill your target.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.BountyHunter : Colors.Neutral;
            RoleType = RoleEnum.BountyHunter;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
            RoleDescription = "You are a Bounty Hunter! You are an assassin who is in pursuit of a target! Find and kill them at all costs!";
            UsesLeft = CustomGameOptions.BountyHunterGuesses;
        }

        public KillButton GuessButton
        {
            get => _guessButton;
            set
            {
                _guessButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float CheckTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastChecked;
            var num = CustomGameOptions.BountyHunterCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (!Player.Data.IsDead || Player.Data.Disconnected)
                return true;
            
            return !(TargetPlayer.Data.IsDead || TargetPlayer.Data.Disconnected) && !TargetKilled;
        }
    }
}