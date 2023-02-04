using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Jester : Role
    {
        public bool VotedOut;
        public List<byte> ToHaunt = new List<byte>();
        public bool HasHaunted = false;
        private KillButton _hauntButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastHaunted;

        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            StartText = "It Was Jest A Prank Bro";
            AbilitiesText = "- After you get ejected, you can haunt a player.";
            AttributesText = "- Haunted players will die.";
            Objectives = "- Get ejected.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jester : Colors.Neutral;
            RoleType = RoleEnum.Jester;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            Results = InspResults.JestMafiSideDamp;
            RoleDescription = "You are a Jester! You are a suicidal lunatic who wants to be thrown out of the airlock. Get yourself ejected at all costs!";
            FactionDescription = NeutralFactionDescription;
            AlignmentDescription = NEDescription;
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
            VotedOut = true;
        }

        public void SetHaunted(MeetingHud __instance)
        {
            ToHaunt.Clear();

            if (!VotedOut)
                return;

            foreach (var state in __instance.playerStates)
            {
                if (state.AmDead || Utils.PlayerById(state.TargetPlayerId).Data.Disconnected || state.VotedFor != Player.PlayerId || state.TargetPlayerId == Player.PlayerId)
                    continue;
                
                ToHaunt.Add(state.TargetPlayerId);
            }
        }

        public KillButton HauntButton
        {
            get => _hauntButton;
            set
            {
                _hauntButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float HauntTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastHaunted;
            var num = CustomGameOptions.DampBiteCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}