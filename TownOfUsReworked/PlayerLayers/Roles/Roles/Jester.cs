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
        public KillButton _hauntButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastHaunted;

        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            StartText = "It Was Jest A Prank Bro";
            AbilitiesText = "Get ejected!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jester : Colors.Neutral;
            RoleType = RoleEnum.Jester;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            Results = InspResults.JestJuggWWInv;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            VotedOut = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
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
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
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